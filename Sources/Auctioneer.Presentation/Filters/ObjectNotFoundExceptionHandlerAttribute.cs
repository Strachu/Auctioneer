using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Auctioneer.Logic;

namespace Auctioneer.Presentation.Filters
{
	public class ObjectNotFoundExceptionHandlerAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			if(filterContext.Exception == null)
				return;

			if(filterContext.Exception.GetType() == typeof(ObjectNotFoundException))
			{
				filterContext.Result = new HttpNotFoundResult(filterContext.Exception.Message);
				filterContext.ExceptionHandled = true;
			}
		}
	}
}