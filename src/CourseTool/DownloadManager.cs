using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace uLearn.CourseTool
{
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
			{
				Console.WriteLine("Login failed:");
				Console.WriteLine(response.value.ToObject<string>());
				throw new OperationFailedGracefully();
			}
			return client;
		}

		public static void Download(string edxStudioUrl, string email, string password, string organization, string course, string time, string filename)
		{
			var downloadUrl = String.Format("{0}/export/{1}/{2}/{3}?_accept=application/x-tgz", edxStudioUrl, organization, course, time);
			var client = GetLoggedInClient(edxStudioUrl, email, password);
			try
			{
				client.DownloadFile(downloadUrl, filename);
			}
			catch (WebException e)
			{
				var response = e.Response.GetResponseStream().GetString();
				Console.WriteLine("Download failed");
				Console.WriteLine(downloadUrl);
				Console.WriteLine(e.Message);
				File.WriteAllText("errorResponse.html", response);
				Console.WriteLine("Error details in errorResponse.html");
				throw new OperationFailedGracefully();
			}
		}

		public static void Download(string baseDir, Config config, string edxStudioUrl, Credentials credentials)
		{
			Console.WriteLine("Downloading {0}.tar.gz", config.CourseRun);
			Download(edxStudioUrl, credentials.Email, credentials.GetPassword(), config.Organization, config.CourseNumber, config.CourseRun, config.CourseRun + ".tar.gz");

			ArchiveManager.ExtractTar(config.CourseRun + ".tar.gz", ".");
			Utils.DeleteFileIfExists(config.CourseRun + ".tar.gz");
			Utils.DeleteDirectoryIfExists(baseDir + "/olx");
			Directory.Move(config.CourseRun, baseDir + "/olx");
		}

		public static void Upload(string edxStudioUrl, string email, string password, string organization, string course, string time, string filename)
		{
			var uploadUrl = string.Format("{0}/import/{1}/{2}/{3}", edxStudioUrl, organization, course, time);
			var client = GetLoggedInClient(edxStudioUrl, email, password);

			var boundary = "---" + DateTime.Now.Ticks.ToString("x");
			client.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
			var fileData = client.Encoding.GetString(File.ReadAllBytes(filename));
			var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"course-data\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, filename, "application/gzip", fileData);
			var nfile = client.Encoding.GetBytes(package);

			var responseString = TryUpload(client, uploadUrl, nfile);
			var response = JsonConvert.DeserializeObject<dynamic>(responseString);
			if (response.Status.ToObject<string>() != "OK")
			{
				Console.WriteLine("Upload failed");
				Console.WriteLine(uploadUrl);
				Console.WriteLine(responseString);
				throw new OperationFailedGracefully();
			}
		}

		private static string TryUpload(CookieAwareWebClient client, string uploadUrl, byte[] nfile)
		{
			try
			{
				return Encoding.UTF8.GetString(client.UploadData(uploadUrl, "POST", nfile));
			}
			catch (WebException e)
			{
				var response = e.Response.GetResponseStream().GetString();
				Console.WriteLine("Upload failed");
				Console.WriteLine(uploadUrl);
				Console.WriteLine(e.Message);
				File.WriteAllText("errorResponse.html", response);
				Console.WriteLine("Error details in errorResponse.html");
				throw new OperationFailedGracefully();
			}
		}

		public static void Upload(string baseDir, string courseName, Config config, string edxStudioUrl, Credentials credentials)
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
			Upload(edxStudioUrl, credentials.Email, credentials.GetPassword(), config.Organization, config.CourseNumber, config.CourseRun, courseName + ".tar.gz");
			Utils.DeleteFileIfExists(courseName + ".tar.gz");
		}
	}

	public class OperationFailedGracefully : Exception
	{
	}
}
