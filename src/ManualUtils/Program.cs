using System;
using System.IO;
using System.Linq;
using AntiPlagiarism.Web.Database;
using AntiPlagiarism.Web.Database.Models;
using Database;
using ManualUtils.AntiPlagiarism;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Ulearn.Core.Configuration;
using Ulearn.Core.Logging;

namespace ManualUtils
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			var configuration = ApplicationConfiguration.Read<UlearnConfiguration>();
			LoggerSetup.Setup(configuration.HostLog, configuration.GraphiteServiceName);
			var optionsBuilder = new DbContextOptionsBuilder<UlearnDb>()
				.UseLazyLoadingProxies()
				.UseSqlServer(configuration.Database);
			var db = new UlearnDb(optionsBuilder.Options);
			var aOptionsBuilder = new DbContextOptionsBuilder<AntiPlagiarismDb>()
				.UseLazyLoadingProxies()
				.UseSqlServer(configuration.Database);
			var adb = new AntiPlagiarismDb(aOptionsBuilder.Options);

			//GetMostSimilarSubmission(adb);
			//ParsePairWeightsFromLogs();
			//GetBlackAndWhiteLabels(db, adb);
			//ParseTaskWeightsFromLogs();
			//CampusRegistration();
			GetIps(db);
			//FillAntiplagFields.FillClientSubmissionId(adb);
		}
		
		private static void GetMostSimilarSubmission(AntiPlagiarismDb adb)
		{
			//var lines = File.ReadLines("pairweights.txt");
			//var jsons = AntiplagiarismLogsParser.GetWeightsOfSubmissionPairs(lines).Select(JsonConvert.SerializeObject);
			//File.WriteAllLines("result.txt", jsons);
			var bestPairWeight = File.ReadLines("result.txt").Select(JsonConvert.DeserializeObject<BestPairWeight>);
			var now = DateTime.UtcNow;
			var mostSimilarSubmissions = bestPairWeight.Select(s => new MostSimilarSubmission
			{
				SubmissionId = s.Submission,
				SimilarSubmissionId = s.Other,
				Weight = s.Weight,
				Timestamp = now
			}).ToList();

			var exist = adb.MostSimilarSubmissions.Select(s => s.SubmissionId).ToList().ToHashSet();
			var i = 0;
			foreach (var mostSimilarSubmission in mostSimilarSubmissions)
			{
				if(exist.Contains(mostSimilarSubmission.SubmissionId))
					continue;
				adb.MostSimilarSubmissions.Add(mostSimilarSubmission);
				if (i % 1000 == 0)
				{
					adb.SaveChanges();
					Console.WriteLine(i);
				}
				i++;
			}
			adb.SaveChanges();
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
			var jsons = AntiplagiarismLogsParser.GetWeightsOfSubmissionPairs(lines).Select(JsonConvert.SerializeObject);
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
			GetIpAddresses.Run(db, lastMonthCount: 13, courses, isNotMembersOfGroups: true, onlyRegisteredFrom: true);
		}
	}
}