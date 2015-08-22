using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Postal;

namespace Auctioneer.Presentation.Emails
{
	/// <summary>
	/// This class provides a workaround for exception "The request lifetime scope cannot be created because
	/// the HttpContext is not available" when using Postal in a background thread and Autofac as an IoC container.
	/// </summary>
	public class WorkaroundEmailService : EmailService
	{
		public WorkaroundEmailService() : base(new WorkaroundEmailRendererer(ViewEngines.Engines),
		                                       new EmailParser(new WorkaroundEmailRendererer(ViewEngines.Engines)),
		                                       () => new SmtpClient())
		{
		}

		private class WorkaroundEmailRendererer : IEmailViewRenderer
		{
			private readonly EmailViewRenderer mImplementation;

			public WorkaroundEmailRendererer(ViewEngineCollection viewEngines)
			{
				mImplementation = new EmailViewRenderer(viewEngines);
			}

			public string Render(Email email, string viewName)
			{
				if(HttpContext.Current == null)
				{
					HttpContext.Current = new HttpContext(new HttpRequest(String.Empty, Constants.ROOT_URL, String.Empty),
					                                      new HttpResponse(TextWriter.Null));
				}

				return mImplementation.Render(email, viewName);
			}
		}
	}
}
