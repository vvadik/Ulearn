using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Optimization;
using AspNetBundling;

namespace uLearn.Web
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(JsBundle());

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/modernizr.bundle.js").Include("~/Scripts/modernizr-*"));

			bundles.Add(CssBundle());
		}

		private static Bundle JsBundle()
		{
			var allScripts = GetLibrariesScripts().Concat(GetUlearnScripts()).ToArray();
			return new ScriptWithSourceMapBundle("~/scripts.bundle.js").Include(allScripts);
		}

		private static IEnumerable<string> GetLibrariesScripts()
		{
			var dir = AppDomain.CurrentDomain.BaseDirectory;
			var lines = File.ReadAllLines(Path.Combine(dir, "LibrariesScripts.txt"));
			return lines.Select(x => x.Replace('\\', '/')).Select(x => "~/" + x).ToArray();
		}

		private static IEnumerable<string> GetUlearnScripts()
		{
			var dir = AppDomain.CurrentDomain.BaseDirectory;
			var lines = File.ReadAllLines(Path.Combine(dir, "UlearnScripts.txt"));
			return lines.Select(x => x.Replace('\\', '/')).Select(x => "~/" + x).ToArray();
		}

		private static Bundle CssBundle()
		{
			var dir = AppDomain.CurrentDomain.BaseDirectory;
			var lines = File.ReadAllLines(Path.Combine(dir, "UlearnStyles.txt"));
			return new StyleBundle("~/ulearn.bundle.css").Include(
				lines.Select(x => x.Replace('\\', '/')).Select(x => "~/" + x).ToArray()
				);
		}
	}
}
