using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using log4net;
using Newtonsoft.Json;
using uLearn;
using uLearn.Extensions;
using XQueue.Models;

namespace XQueue
{
	public class XQueueClient
	{
		private const string loginUrl = "xqueue/login/";
		private const string getSubmissionUrl = "xqueue/get_submission/";
		private const string putResultUrl = "xqueue/put_result/";

		private static readonly ILog log = LogManager.GetLogger(typeof(XQueueClient));

		private readonly string username;
		private readonly string password;
		private readonly HttpClient client;

		public XQueueClient(string baseUrl, string username, string password)
		{
			this.username = username;
			this.password = password;

			client = new HttpClient(new HttpClientHandler
			{
				AllowAutoRedirect = true,
				UseCookies = true,
				CookieContainer = new CookieContainer(),
			})
			{
				BaseAddress = new Uri(baseUrl),
				Timeout = TimeSpan.FromMinutes(1)
			};
		}

		public XQueueClient(string baseUrl)
			: this(baseUrl, "", "")
		{
		}

		private async Task<bool> TryLogin()
		{
			if (string.IsNullOrEmpty(username))
				return true;

			log.Info($"Try to login in xqueue {loginUrl} with username \"{username}\" and password \"{password.MaskAsSecret()}\"");

			var loginData = new Dictionary<string, string>
			{
				{ "username", username },
				{ "password", password }
			};

			var response = await client.PostAsync(loginUrl, new FormUrlEncodedContent(loginData));
			if (response.IsSuccessStatusCode)
			{
				log.Warn($"Unexpected response status code for login: {response.StatusCode}");
				throw new Exception($"Unexpected response status code for login: {response.StatusCode}");
			}
			return response.StatusCode == HttpStatusCode.OK;
		}

		public async Task<bool> Login()
		{
			return await FuncUtils.TrySeveralTimesAsync(TryLogin, 5, () => Task.Delay(TimeSpan.FromSeconds(1)));
		}

		public async Task<XQueueSubmission> GetSubmission(string queueName)
		{
			var urlParams = HttpUtility.ParseQueryString(getSubmissionUrl);
			urlParams.Add("queue_name", queueName);
			urlParams.Add("block", "true");

			log.Info($"Try to get new submission from xqueue {urlParams}");
			HttpResponseMessage response;
			try
			{
				response = await client.GetAsync(urlParams.ToString());
			}
			catch (Exception e)
			{
				log.Warn($"Didn't get submission from xqueue: {e.Message}", e.InnerException);
				return null;
			}

			if (response.IsSuccessStatusCode)
			{
				try
				{
					return (await response.Content.ReadAsStringAsync()).DeserializeJson<XQueueSubmission>();
				}
				catch (Exception e)
				{
					log.Error($"Can\'t parse answer from xqueue: {e.Message}", e);
					return null;
				}
			}

			if (response.StatusCode == HttpStatusCode.Moved || response.StatusCode == HttpStatusCode.Found)
			{
				log.Warn("Redirected to the login page. Try to authorize again");
				await Login();
				return null;
			}

			return null;
		}

		public async Task<bool> PutResult(XQueueResult result)
		{
			return await FuncUtils.TrySeveralTimesAsync(() => TryPutResult(result), 5, () => Task.Delay(TimeSpan.FromMilliseconds(1)));
		}

		public async Task<bool> TryPutResult(XQueueResult result)
		{
			var content = JsonConvert.SerializeObject(result);
			var response = await client.PostAsync(putResultUrl, new StringContent(content));
			if (response.IsSuccessStatusCode)
			{
				log.Warn($"Unexpected response status code while putting results to xqueue: {response.StatusCode}");
				throw new Exception($"Unexpected response status code while putting results to xqueue: {response.StatusCode}");
			}

			return true;
		}
	}
}
