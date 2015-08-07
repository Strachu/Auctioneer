using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic;
using Auctioneer.Logic.Users;

using Auctioneer.Presentation.Controllers;
using Auctioneer.Presentation.Emails;
using Auctioneer.Presentation.Infrastructure.Security;
using Auctioneer.Presentation.Models.User;

using FakeItEasy;

using Microsoft.AspNet.Identity.Owin;

using NUnit.Framework;

using Postal;

namespace Auctioneer.Presentation.Tests.Controllers
{
	[TestFixture]
	internal class UserControllerTests
	{
		private UserController            mTestedController;
		private IUserService              mUserServiceMock;
		private IEmailService             mMailServiceMock;
		private AuthenticationManagerMock mAuthManagerMock;

		[SetUp]
		public void SetUp()
		{
			mUserServiceMock = A.Fake<IUserService>();
			mMailServiceMock = A.Fake<IEmailService>();
			mAuthManagerMock = new AuthenticationManagerMock();

			mTestedController = new UserController(mUserServiceMock, mMailServiceMock, mAuthManagerMock);
		}

		[Test]
		public async Task CreateActionCreatesTheUser()
		{
			await mTestedController.Create(CreateDummyViewModel());

			A.CallTo(() => mUserServiceMock.AddUser(null, null, null)).WithAnyArguments().MustHaveHappened();
		}

		[Test]
		public async Task CreateActionSendsConfirmationMail()
		{
			await mTestedController.Create(CreateDummyViewModel());

			A.CallTo(() => mMailServiceMock.SendAsync(A<Email>.That.IsInstanceOf(typeof(ConfirmationMail)))).MustHaveHappened();
		}
		
		[Test]
		public async Task ConfirmationMailIsNotSendWhenAddingUserFails()
		{
			A.CallTo(() => mUserServiceMock.AddUser(null, null, null)).WithAnyArguments().Invokes(x =>
			{
				var validationErrorNotifier = (IValidationErrorNotifier) x.Arguments[2];
				validationErrorNotifier.AddError("Error");
			}).Returns(Task.FromResult(0));

			await mTestedController.Create(CreateDummyViewModel());

			A.CallTo(() => mMailServiceMock.SendAsync(A<Email>.Ignored)).MustNotHaveHappened();
		}
		
		[Test]
		public async Task UserIsNotLoggedInIfHisEmailHasNotBeenConfirmed()
		{
			A.CallTo(() => mUserServiceMock.GetUserByUsername(A<string>.Ignored)).Returns(new User { EmailConfirmed = false });

			await mTestedController.Login(new UserLoginViewModel { Username = "User", Password = "Pass" });

			Assert.That(mAuthManagerMock.LoggedUserName, Is.Null);
		}

		private UserCreateViewModel CreateDummyViewModel()
		{
			return new UserCreateViewModel
			{
				Username         = "User",
				Password         = "Pass",
				RepeatedPassword = "Pass",
				Email            = "mail@mail.com",
				FirstName        = "FirstName",
				LastName         = "LastName",
				Address          = "Address"
			};
		}

		private class AuthenticationManagerMock : IAuthenticationManager
		{
			public Task<SignInStatus> SignIn(string userName, string password, bool rememberMe)
			{
				LoggedUserName = userName;

				return Task.FromResult(SignInStatus.Success);
			}

			public Task SignOut()
			{
				LoggedUserName = null;

				return Task.FromResult(0);
			}

			public string LoggedUserName
			{
				get;
				private set;
			}
		}
	}
}
