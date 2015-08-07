using System.Web;
using System.Web.Optimization;

namespace Auctioneer.Presentation
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
							"~/Content/Libraries/JQuery/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
							"~/Content/Libraries/JQuery/jquery.validate*"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
							"~/Content/Libraries/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
						 "~/Content/Libraries/Bootstrap/bootstrap.js",
						 "~/Content/Libraries/respond.js"));

			bundles.Add(new StyleBundle("~/bundles/CSS").Include(
						 "~/Content/Libraries/Bootstrap/bootstrap.css",
						 "~/Content/CSS/*.css"));
		}
	}
}
