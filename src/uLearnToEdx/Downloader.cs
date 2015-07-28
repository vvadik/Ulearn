using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace uLearnToEdx
{
	public static class Downloader
	{
		public static void Download(string host, int port, string email, string password, string organization, string course, string time, string filename)
		{
			var baseUrl = string.Format("http://{0}:{1}", host, port);
			var signinUrl = string.Format("{0}/signin", baseUrl);
			var loginUrl = string.Format("{0}/login_post", baseUrl);
			var downloadUrl = string.Format("{0}/export/{1}/{2}/{3}?_accept=application/x-tgz", baseUrl, organization, course, time);
			var client = new CookieAwareWebClient();
			client.DownloadData(signinUrl);
			var response = JsonConvert.DeserializeObject<dynamic>(Encoding.UTF8.GetString(client.UploadValues(loginUrl, new NameValueCollection
			{
				{ "email", email },
				{ "password", password }
			})));
			if (!response.success.ToObject<bool>())
				throw new Exception("Could not download file.");
			client.DownloadFile(downloadUrl, filename);
		}

		public static void Upload(string host, int port, string email, string password, string organization, string course, string time, string filename)
		{
			var baseUrl = string.Format("http://{0}:{1}", host, port);
			var signinUrl = string.Format("{0}/signin", baseUrl);
			var loginUrl = string.Format("{0}/login_post", baseUrl);
			var uploadUrl = string.Format("{0}/import/{1}/{2}/{3}", baseUrl, organization, course, time);
			var client = new CookieAwareWebClient();
			client.DownloadData(signinUrl);
			var response = JsonConvert.DeserializeObject<dynamic>(Encoding.UTF8.GetString(client.UploadValues(loginUrl, new NameValueCollection
			{
				{ "email", email },
				{ "password", password }
			})));
			if (!response.success.ToObject<bool>())
				throw new Exception("Could not upload file.");

			string boundary = "------------------------MLG-NOSCOPE" + DateTime.Now.Ticks.ToString("x");
			client.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
			var fileData = client.Encoding.GetString(File.ReadAllBytes(filename));
			var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, filename, "application/gzip", fileData);

			var nfile = client.Encoding.GetBytes(package);

			try
			{
				byte[] resp = client.UploadData(uploadUrl, "POST", nfile);
				Console.WriteLine(Encoding.UTF8.GetString(resp));
			}
			catch (WebException e)
			{
//				Console.WriteLine(e.Response.);
			}
			
//			client.UploadFile(uploadUrl, filename);
		}
	}
}
