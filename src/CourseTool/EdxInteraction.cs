using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
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
			Console.WriteLine($"Downloading {config.ULearnCourseId}.tar.gz from {edxStudioUrl}");
			Download(edxStudioUrl, credentials.Email, credentials.GetPassword(), config.Organization, config.CourseNumber, config.CourseRun, baseDir + "/" + config.ULearnCourseId + ".tar.gz");
		}

		public static string GetTarGzFile(string dir)
		{
			var archives = dir.GetFiles("*.tar.gz");
			if (archives.Length == 1)
				return archives[0];
			throw new Exception($"Can't decide which archive to extract! Remove all other tar.gz files. Found: {string.Join(", ", archives)}");
		}

		public static void ExtractEdxCourseArchive(string baseDir, string tarGzFilepath)
		{
			var filename = tarGzFilepath;
			Console.WriteLine($"Extracting {filename}");

			var extractedTarDirectory = Path.Combine(baseDir, ".extracted-" + Path.GetFileName(filename).Replace(".tar.gz", ""));
			ArchiveManager.ExtractTar(filename, extractedTarDirectory);
			//Utils.DeleteFileIfExists(filename);
			var extractedOlxDirectory = Directory.GetDirectories(extractedTarDirectory).Single();
			Utils.DeleteDirectoryIfExists(baseDir + "/olx");
			Directory.Move(extractedOlxDirectory, baseDir + "/olx");
			Directory.Delete(extractedTarDirectory);
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

		public static void Upload(string courseName, Config config, string edxStudioUrl, Credentials credentials)
		{
			Console.WriteLine($"Uploading {courseName}.tar.gz to {edxStudioUrl}");
			Upload(edxStudioUrl, credentials.Email, credentials.GetPassword(), config.Organization, config.CourseNumber, config.CourseRun, courseName + ".tar.gz");
			Utils.DeleteFileIfExists(courseName + ".tar.gz");
		}

		public static void CreateEdxCourseArchive(string baseDir, string courseName)
		{
			Environment.CurrentDirectory = baseDir;
			var outputTarFilename = $"{courseName}.tar.gz";
			Console.WriteLine($"Creating archive {outputTarFilename}");
			Utils.DeleteFileIfExists(outputTarFilename);
			ArchiveManager.CreateTar(outputTarFilename, "olx");
		}
	}
}
