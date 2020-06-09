using System;
using System.IO;
using System.Linq;
using AntiPlagiarism.Web.Database;
using Database;
using ManualUtils.AntiPlagiarism;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Ulearn.Core.Configuration;

namespace ManualUtils
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			var configuration = ApplicationConfiguration.Read<UlearnConfiguration>();
			var optionsBuilder = new DbContextOptionsBuilder<UlearnDb>()
				.UseLazyLoadingProxies()
				.UseSqlServer(configuration.Database);
			var db = new UlearnDb(optionsBuilder.Options);
			var aOptionsBuilder = new DbContextOptionsBuilder<AntiPlagiarismDb>()
				.UseLazyLoadingProxies()
				.UseSqlServer(configuration.Database);
			var adb = new AntiPlagiarismDb(aOptionsBuilder.Options);

			//ParsePairWeightsFromLogs();
			//GetBlackAndWhiteLabels(db, adb);
			//ParseTaskWeightsFromLogs();
			//CampusRegistration();
			//GetIps();
			FillAntiplagFields.FillClientSubmissionId(adb);
		}

		private static void GetBlackAndWhiteLabels(UlearnDb db, AntiPlagiarismDb adb)
		{
			var lines = File.ReadLines("pairweights.txt");
			var jsons = PlagiarismInstructorDecisions.GetBlackAndWhiteLabels(db, adb,
				lines.Select(JsonConvert.DeserializeObject<BestPairWeight>));
			File.WriteAllLines("blackAndWhiteLabels.txt", jsons);
		}

		private static void ParsePairWeightsFromLogs()
		{
			var lines = File.ReadLines("pairweights.txt");
			var jsons = AntiplagiarismLogsParser.GetWeightsOfSubmisisonPairs(lines);
			File.WriteAllLines("result.txt", jsons);
		}

		private static void ParseTaskWeightsFromLogs(UlearnDb db)
		{
			var lines = File.ReadLines("weights.txt");
			var jsons = AntiplagiarismLogsParser.GetWeightsForStatistics(db, lines);
			File.WriteAllLines("result.txt", jsons);
		}

		private static void CampusRegistration(UlearnDb db)
		{
			ManualUtils.CampusRegistration.Run(db, courseId: "Campus1920", groupId: 803, slideWithRegistrationQuiz: new Guid("67bf45bd-bebc-4bde-a705-c16763b94678"), false);
		}

		private static void GetIps(UlearnDb db)
		{
			// Для получения городов см. geoip.py
			// Где взять GeoLite2-City.mmdb читай в GeoLite2-City.mmdb.readme.txt
			var courses = new[] { "BasicProgramming", "BasicProgramming2", "Linq", "complexity", "CS2" };
			GetIpAddresses.Run(db, lastMonthCount: 6, courses, isNotMembersOfGroups: true);
		}
	}
}