using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace uLearn.CourseTool
{
	public static class EdxInteraction
	{
		private static CookieAwareWebClient LogIn(string baseUrl, string email, string password)
		{
			var signinUrl = string.Format("{0}/signin", baseUrl);
			var loginUrl = string.Format("{0}/login_post", baseUrl);
			var client = new CookieAwareWebClient();
			client.DownloadData(signinUrl);

			var response = JsonConvert.DeserializeObject<dynamic>(Encoding.UTF8.GetString(client.UploadValues(loginUrl, new NameValueCollection
			{
				{ "email", email },
				{ "password", password }
			})));
			if (response.success.ToObject<bool>())
				return client;
			Console.WriteLine("Login failed:");
			Console.WriteLine(response.value.ToObject<string>());
			throw new OperationFailedGracefully();
		}

		private static void Download(string edxStudioUrl, string email, string password, string organization, string course, string time, string filename)
		{
			var downloadUrl = $"{edxStudioUrl}/export/course-v1:{organization}+{course}+{time}?_accept=application/x-tgz";
			var client = LogIn(edxStudioUrl, email, password);
			client.TryDownloadFile(downloadUrl, filename);
		}

		public static void Download(string baseDir, Config config, string edxStudioUrl, Credentials credentials)
		{
			Console.WriteLine("Downloading {0}.tar.gz from {1}", config.CourseRun, edxStudioUrl);
			Download(edxStudioUrl, credentials.Email, credentials.GetPassword(), config.Organization, config.CourseNumber, config.CourseRun, baseDir + "/" + config.CourseRun + ".tar.gz");
		}

		public static void ExtractEdxCourseArchive(string baseDir, string courseId)
		{
			var filename = baseDir + "/" + courseId + ".tar.gz";
			Console.WriteLine($"Extracting {filename}");
			
			ArchiveManager.ExtractTar(filename, baseDir);
			Utils.DeleteFileIfExists(filename);
			Utils.DeleteDirectoryIfExists(baseDir + "/olx");
			Directory.Move(baseDir + "/course", baseDir + "/olx");
		}

		private static void Upload(string edxStudioUrl, string email, string password, string organization, string course, string time, string filename)
		{
			var uploadUrl = $"{edxStudioUrl}/import/course-v1:{organization}+{course}+{time}";
			var client = LogIn(edxStudioUrl, email, password);

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

		private static string TryUpload(WebClient client, string uploadUrl, byte[] nfile)
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
			Console.WriteLine("Uploading {0}.tar.gz to {1}", courseName, edxStudioUrl);
			Upload(edxStudioUrl, credentials.Email, credentials.GetPassword(), config.Organization, config.CourseNumber, config.CourseRun, courseName + ".tar.gz");
			Utils.DeleteFileIfExists(courseName + ".tar.gz");
		}

		public static void CreateEdxCourseArchive(string baseDir, string courseName)
		{
			Environment.CurrentDirectory = baseDir;
			var outputTarFilename = $"{courseName}.tar.gz";
			Console.WriteLine($"Creating archive {outputTarFilename}");

			Utils.DeleteDirectoryIfExists("temp");
			if (Directory.Exists(courseName))
				Directory.Move(courseName, "temp");
			Utils.DirectoryCopy("olx", courseName, true);
			Utils.DeleteFileIfExists(outputTarFilename);

			ArchiveManager.CreateTar(outputTarFilename, courseName);

			Utils.DeleteDirectoryIfExists(courseName);
			if (Directory.Exists("temp"))
				Directory.Move("temp", courseName);
		}
	}
}
