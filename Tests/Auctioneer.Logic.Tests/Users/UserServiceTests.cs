using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Tests.TestUtils;
using Auctioneer.Logic.Tests.TestUtils.ModelsWithDefaultValues;
using Auctioneer.Logic.Users;

using FakeItEasy;

using Microsoft.AspNet.Identity;

using NUnit.Framework;

namespace Auctioneer.Logic.Tests.Users
{
	internal class UserServiceTests
	{
		private IUserService  mTestedService;
		private DbConnection  mDatabaseConnection;
		private IUserNotifier mUserNotifierMock;

		[SetUp]
		public void SetUp()
		{
			CreateDatabaseWithTestData();

			var context       = new TestAuctioneerDbContext(mDatabaseConnection);
			mUserNotifierMock = A.Fake<IUserNotifier>();

			mTestedService = new UserService(context, mUserNotifierMock);
		}

		private void CreateDatabaseWithTestData()
		{
			mDatabaseConnection = Effort.DbConnectionFactory.CreateTransient();

			var tempService = new UserService(new TestAuctioneerDbContext(mDatabaseConnection), A.Fake<IUserNotifier>());

			tempService.Create(new TestUser { Id = "1", Email = "1@a.com" }, "Password");
			tempService.Create(new TestUser { Id = "2", Email = "2@a.com" }, "Password");
			tempService.Create(new TestUser { Id = "3", Email = "3@a.com" }, "Password");
			tempService.Create(new TestUser { Id = "4", Email = "4@a.com" }, "Password");
		}

		[Test]
		public async Task AfterUserHasBeenAdded_AnActivationTokenIsSentToHim()
		{
			await mTestedService.AddUser(new TestUser(), "Password", new FakeErrorNotifier());

			A.CallTo(() => mUserNotifierMock.SendActivationToken(null, null)).WithAnyArguments().MustHaveHappened();
		}

		[Test]
		public async Task WhenAddingUserFails_AnActivationTokenIsNotSentToHim()
		{
			await mTestedService.AddUser(new TestUser { Email = "1@a.com" }, "Password", new FakeErrorNotifier());

			A.CallTo(() => mUserNotifierMock.SendActivationToken(null, null)).WithAnyArguments().MustNotHaveHappened();
		}

		[Test]
		public async Task WhenValidPasswordHasBeenPassed_UpdateEmailChangesTheEmail()
		{
			var errorNotifier = new FakeErrorNotifier();

			await mTestedService.UpdateUserEmail("1", "Password", "newmail@a.com", errorNotifier);

			var user = ReadUserFromDatabase("1");

			Assert.That(!errorNotifier.IsInErrorState);
			Assert.That(user.Email, Is.EqualTo("newmail@a.com"));
		}

		[Test]
		public async Task WhenValidPasswordHasBeenPassed_NewEmailAlreadyExists_UpdateEmailDoesNotChangeTheEmail()
		{
			var errorNotifier = new FakeErrorNotifier();

			await mTestedService.UpdateUserEmail("1", "Password", "3@a.com", errorNotifier);

			var user = ReadUserFromDatabase("1");

			Assert.That(errorNotifier.IsInErrorState);
			Assert.That(user.Email, Is.EqualTo("1@a.com"));
		}

		[Test]
		public async Task WhenValidPasswordHasBeenPassed_EmailIsTheSameAsBefore_NoErrorIsRaised()
		{
			var errorNotifier = new FakeErrorNotifier();

			await mTestedService.UpdateUserEmail("1", "Password", "1@a.com", errorNotifier);

			Assert.That(!errorNotifier.IsInErrorState);
		}

		[Test]
		public async Task WhenInvalidPasswordHasBeenPassed_UpdateEmailDoesNotChangeTheEmail()
		{
			var errorNotifier = new FakeErrorNotifier();

			await mTestedService.UpdateUserEmail("1", "WrongPassword", "newmail@a.com", errorNotifier);

			var user = ReadUserFromDatabase("1");

			Assert.That(errorNotifier.IsInErrorState);
			Assert.That(user.Email, Is.EqualTo("1@a.com"));
		}

		private User ReadUserFromDatabase(string userId)
		{
			// New context is required because if SaveChanges() fails, the changes are not saved to the database
			// but in memory version of the entity is still in invalid state, changes to it are not rollback.
			var newContext = new TestAuctioneerDbContext(mDatabaseConnection);

			return new UserService(newContext, A.Fake<IUserNotifier>()).FindById(userId);
		}
	}
}
