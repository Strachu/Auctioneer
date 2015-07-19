using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Auctioneer.Presentation.Tests.TestUtils
{
	internal static class ActionResultExtensions
	{
		public static async Task<T> GetModel<T>(this Task<ActionResult> actionResult)
		{
			Contract.Requires(actionResult != null);

			var viewResult = (ViewResult)await actionResult;

			return (T)viewResult.Model;			
		}
	}
}
