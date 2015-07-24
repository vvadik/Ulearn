using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using NUnit.Framework;
using ObjectPrinter;
using uLearn;
using uLearn.Model.Blocks;
using uLearn.Model.Edx;
using uLearn.Model.Edx.EdxComponents;
using uLearn.Quizes;
using uLearnToEdx.Json;

namespace uLearnToEdx
{
	public static class Converter
	{
		private static string exerciesUrl = "https://192.168.33.1:44300/Course/{0}/LtiSlide/";
		private static string solutionsUrl = "https://192.168.33.1:44300/Course/{0}/AcceptedAlert/";

		private static Sequential[] CourseToSequentials(Course course, Dictionary<string, string> videoGuids)
		{
			var units = course.GetUnits().ToList();
			return Enumerable
				.Range(0, units.Count)
				.Select(
					x => new Sequential(course.Id + "-1-" + x, units[x], 
						course.Slides
							.Where(y => y.Info.UnitName == units[x])
							.SelectMany(y => y.ToVerticals(course.Id, exerciesUrl, solutionsUrl, videoGuids))
							.ToArray()
					)
				).ToArray();
		}

		public static EdxCourse ToEdxCourse(
			Course course, string organization, string[] advancedModules, string[] ltiPassports, string ltiHostname,
			Dictionary<string, string> videoGuids)
		{
			return new EdxCourse(
				course.Id, organization, course.Title, advancedModules, ltiPassports, 
				new [] { new Chapter(course.Id + "-1", course.Title, CourseToSequentials(course, videoGuids)) }
			);
		}
	}
}
