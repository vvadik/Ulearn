using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using uLearn;

namespace uLearnToEdx
{
	public class LoginException : Exception
	{
		public LoginException() : base() { }
		public LoginException(string message) : base(message) { }
	}

	public class UploadException : Exception
	{
	}

	public static class DownloadManager
	{
		private static CookieAwareWebClient GetLoggedInClient(string baseUrl, string email, string password)
		{
			var signinUrl = String.Format("{0}/signin", baseUrl);
			var loginUrl = String.Format("{0}/login_post", baseUrl);
			var client = new CookieAwareWebClient();
			client.DownloadData(signinUrl);

			var response = JsonConvert.DeserializeObject<dynamic>(Encoding.UTF8.GetString(client.UploadValues(loginUrl, new NameValueCollection
			{
				{ "email", email },
				{ "password", password }
			})));
			if (!response.success.ToObject<bool>())
				throw new LoginException(response.value.ToObject<string>());
			return client;
		}

		public static void Download(string host, int port, string email, string password, string organization, string course, string time, string filename)
		{
			var baseUrl = String.Format("http://{0}:{1}", host, port);
			var downloadUrl = String.Format("{0}/export/{1}/{2}/{3}?_accept=application/x-tgz", baseUrl, organization, course, time);
			var client = GetLoggedInClient(baseUrl, email, password);
			client.DownloadFile(downloadUrl, filename);
		}

		public static void Download(string baseDir, Config config, Credentials credentials)
		{
			Console.WriteLine("Downloading {0}.tar.gz", config.CourseRun);
			Download(config.Hostname, config.Port, credentials.Email, credentials.GetPassword(), config.Organization, config.CourseNumber, config.CourseRun, config.CourseRun + ".tar.gz");

			ArchiveManager.ExtractTar(config.CourseRun + ".tar.gz", ".");
			Utils.DeleteFileIfExists(config.CourseRun + ".tar.gz");
			Utils.DeleteDirectoryIfExists(baseDir + "/olx");
			Directory.Move(config.CourseRun, baseDir + "/olx");
		}

		public static void Upload(string host, int port, string email, string password, string organization, string course, string time, string filename)
		{
			var baseUrl = String.Format("http://{0}:{1}", host, port);
			var uploadUrl = String.Format("{0}/import/{1}/{2}/{3}", baseUrl, organization, course, time);
			var client = GetLoggedInClient(baseUrl, email, password);

			var boundary = "---" + DateTime.Now.Ticks.ToString("x");
			client.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
			var fileData = client.Encoding.GetString(File.ReadAllBytes(filename));
			var package = String.Format("--{0}\r\nContent-Disposition: form-data; name=\"course-data\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, filename, "application/gzip", fileData);
			var nfile = client.Encoding.GetBytes(package);

			var response = JsonConvert.DeserializeObject<dynamic>(Encoding.UTF8.GetString(client.UploadData(uploadUrl, "POST", nfile)));
			if (response.Status.ToObject<string>() != "OK")
				throw new UploadException();
		}

		public static void Upload(string baseDir, string courseName, Config config, Credentials credentials)
		{
			Environment.CurrentDirectory = baseDir;
			Utils.DeleteDirectoryIfExists("temp");
			if (Directory.Exists(courseName))
				Directory.Move(courseName, "temp");
			Utils.DirectoryCopy("olx", courseName, true);
			Utils.DeleteFileIfExists(courseName + ".tar.gz");

			Console.WriteLine("Creating {0}.tar.gz...", courseName);
			ArchiveManager.CreateTar(courseName + ".tar.gz", courseName);

			Utils.DeleteDirectoryIfExists(courseName);
			if (Directory.Exists("temp"))
				Directory.Move("temp", courseName);

			Console.WriteLine("Uploading {0}.tar.gz...", courseName);
			Upload(config.Hostname, config.Port, credentials.Email, credentials.GetPassword(), config.Organization, config.CourseNumber, config.CourseRun, courseName + ".tar.gz");
			Utils.DeleteFileIfExists(courseName + ".tar.gz");
		}
	}
}
