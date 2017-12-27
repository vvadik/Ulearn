using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Database.DataContexts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GiftsGranter
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var settings = JObject.Parse(File.ReadAllText("appsettings.json"));
			var staff = new Staff(settings["authorization"].Value<string>());
			int maxGiftsPerRun = args.Length > 0 ? int.Parse(args[0]) : settings["maxGiftsPerRun"].Value<int>();
			Console.WriteLine("UseGiftGrantsLimitPerRun\t" + maxGiftsPerRun);
			var db = new ULearnDb();
			var repo = new VisitsRepo(db);
			var courses = settings["courses"].Values<string>();
			foreach (string courseId in courses)
			{
				Console.WriteLine($"StartProcessingCourse\t{courseId}");
				GrantGifts(repo, maxGiftsPerRun, courseId, settings, staff);
			}
			Console.WriteLine("Done");
		}

		private static void GrantGifts(VisitsRepo repo, int maxGiftsPerRun, string courseId, JObject settings, Staff staff)
		{
			var courseSettings = settings[courseId];
			var passScore = courseSettings["passScore"].Value<int>();
			var rating = repo.GetCourseRating(courseId, passScore);
			var konturRating = rating.Where(e => e.User.Logins.Any(login => login.LoginProvider == "Контур.Паспорт")).ToList();
			foreach (var entry in konturRating.Take(30))
				Console.WriteLine($"{entry.Score}	{entry.User.VisibleName} {entry.User.KonturLogin}");
			Console.WriteLine("...");
			Console.WriteLine($"TotalCompeled\t{rating.Count}");
			Console.WriteLine($"KonturCompleted\t{konturRating.Count}");

			EnsureHaveGifts(konturRating, maxGiftsPerRun, courseSettings, staff);
		}

		private static void EnsureHaveGifts(List<RatingEntry> entries, int maxGiftsPerRun, JToken courseSettings, Staff staff)
		{
			var granted = 0;
			foreach (var ratingEntry in entries)
			{
				if (granted >= maxGiftsPerRun)
				{
					Console.WriteLine("GiftGrantsLimitPerRunExceeded\t" + maxGiftsPerRun);
					return;
				}
				if (GrantGiftsIfNone(ratingEntry, courseSettings, staff))
					granted++;
				Thread.Sleep(500);
			}
		}

		private static bool GrantGiftsIfNone(RatingEntry entry, JToken courseSettings, Staff staff)
		{
			string sid = entry.User.Logins.First(login => login.LoginProvider == "Контур.Паспорт").ProviderKey;
			var staffUserId = staff.GetUser(sid)["id"].Value<int>();
			var gifts = staff.GetUserGifts(staffUserId);
			var giftImagePath = courseSettings["giftImagePath"].Value<string>();

			bool hasComplexityGift = gifts["entries"].Children().Any(gift => gift["giftImagePath"].Value<string>() == giftImagePath);
			if (!hasComplexityGift)
			{
				Console.WriteLine("NoGiftYet\t" + entry.Score + "\t" + entry.User.VisibleName);
				staff.GrantGift(staffUserId, entry.Score, courseSettings);
				Console.WriteLine("ComplexityGiftGrantedFor\t" + entry.User.VisibleName + "\t" + entry.User.KonturLogin);
				return true;
			}
			return false;
		}
	}

	public class Staff
	{
		private readonly string authorization;

		public Staff(string authorization)
		{
			this.authorization = authorization;
		}

		public JObject Get(string url)
		{
			var client = CreateHttpClient();
			var response = client.GetAsync($"https://staff.skbkontur.ru/api/{url}").Result;
			string result = response.Content.ReadAsStringAsync().Result;
			if (response.StatusCode != HttpStatusCode.OK)
				throw new Exception(result);
			return JObject.Parse(result);
		}

		private HttpClient CreateHttpClient()
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorization);
			return client;
		}

		public JObject Post(string url, JObject jsonContent)
		{
			var client = CreateHttpClient();
			var content = new StringContent(JsonConvert.SerializeObject(jsonContent), Encoding.UTF8, "application/json");
			var response = client.PostAsync($"https://staff.skbkontur.ru/api/{url}", content).Result;
			string result = response.Content.ReadAsStringAsync().Result;
			if (response.StatusCode != HttpStatusCode.OK)
				throw new Exception(result);
			return JObject.Parse(result);
		}

		public JObject GetUser(string sid)
		{
			return Get($"users/getBySid?sid={sid}");
		}

		public JObject GetUserGifts(int staffUserId)
		{
			return Get($"feed/user_{staffUserId}?filter=gift");
		}

		public JObject GrantGift(int staffUserId, int score, JToken courseSettings)
		{
			var gift = Gifts.CreateGift(score, courseSettings);
			Console.WriteLine(JsonConvert.SerializeObject(gift, Formatting.Indented));
			return Post($"feed/user_{staffUserId}", gift);
		}
	}

	public class CourseSettings
	{
		public int maxScore;
		public int masterScore;
		public int passScore;
		public string masterTitle;
		public string passTitle;
		public string message;
		public string giftImagePath;
		public string subtitle;
	}

	public class Gifts
	{
		public static JObject CreateGift(int score, JToken courseSettings)
		{
			var settings = courseSettings.ToObject<CourseSettings>();
			var title = score >= settings.masterScore ? settings.masterTitle : settings.passTitle;
			var subtitle = string.Format(
				settings.subtitle,
				Pluralize(score, "балл", "балла", "баллов"),
				Pluralize(settings.maxScore, "возможного", "возможных", "возможных"));
			return JObject.Parse(
				$@"{{
	""entry"": {{
		""$type"": ""gift"",
		""from"": {{
			""id"": ""1253"",
			""name"": ""Егоров Павел Владимирович"",
			""type"": 1
		}},
		""message"": ""{settings.message}"",
		""anonymously"": true,
		""giftImagePath"": ""{settings.giftImagePath}"",
		""title"": ""{title}"",
		""subtitle"": ""{subtitle}""
	}},
	""options"": {{
		""postAsOwner"": false
	}}
}}");
		}

		public static JObject ComplexityPassed(int score)
		{
			var maxScore = 32;
			string title = score == maxScore ? "Мастер оценки сложности алгоритмов" : "Умею оценивать сложность алгоритмов";
			return JObject.Parse(
				$@"{{
	""entry"": {{
		""$type"": ""gift"",
		""from"": {{
			""id"": ""1253"",
			""name"": ""Егоров Павел Владимирович"",
			""type"": 1
		}},
		""message"": ""Вот-вот продукты Контура будут активно использовать в каждой организации страны, поэтому в их коде нет места неэффективным алгоримам. [Курс по оценке сложности алгоритмов](https://ulearn.me/Course/complexity?kontur=true) на ulearn.me поможет писать только быстрый код :)"",
		""anonymously"": true,
		""giftImagePath"": ""3PLk3n/achivement.png"",
		""title"": ""{title}"",
		""subtitle"": ""{Pluralize(score, "балл", "балла", "баллов")} из {Pluralize(maxScore, "возможного", "возможных", "возможных")} в онлайн курсе по оценке сложности — это вам не хухры-мухры!""
	}},
	""options"": {{
		""postAsOwner"": false
	}}
}}");
		}

		private static string Pluralize(int count, string балл, string балла, string баллов)
		{
			if (count % 10 == 0 || count % 10 >= 5 || count % 100 > 10 && count % 100 < 20)
				return count + " " + баллов;
			return count + " " + (count % 10 == 1 ? балл : балла);
		}
	}
}