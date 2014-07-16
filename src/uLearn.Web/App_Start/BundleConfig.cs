using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;

namespace uLearn.Web
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/jquery").Include(
				"~/Scripts/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/jqueryval").Include(
				"~/Scripts/jquery.validate*"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/modernizr").Include(
				"~/Scripts/modernizr-*"));

			bundles.Add(new ScriptBundle("~/codemirror").Include(
				"~/codemirror/lib/codemirror.js",
				"~/codemirror/mode/clike/clike.js",
				"~/codemirror/addon/hint/show-hint.js",
				//"~/codemirror/addon/hint/anyword-hint.js",
				"~/codemirror/addon/hint/csharp-hint.js"
				));

			bundles.Add(new ScriptBundle("~/bootstrap").Include(
				"~/Scripts/bootstrap.js",
				"~/Scripts/respond.js"));

			bundles.Add(new StyleBundle("~/css").Include(
				"~/Content/bootstrap.css",
				"~/codemirror/lib/codemirror.css",
				"~/codemirror/theme/cobalt.css",
				"~/codemirror/addon/hint/show-hint.css",
				"~/Jsdifflib/jsdifflib.css",
				"~/Content/ulearn.css",
				"~/Content/site.css"));
		}
	}
}