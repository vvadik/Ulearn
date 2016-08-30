using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using RunCsJob.Api;

namespace RunCsJob
{
	internal class Client
	{
		private readonly string token;
		private readonly HttpClient httpClient;

		public Client(string address, string token)
		{
			this.token = token;
			httpClient = new HttpClient { BaseAddress = new Uri(address + "/") };
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public async Task<List<RunnerSubmition>> TryGetSubmissions(int threadsCount)
		{
			var uri = GetUri("GetSubmissions", new[] { "count", threadsCount.ToString(CultureInfo.InvariantCulture) });
			try
			{
				var response = await httpClient.GetAsync(uri);
				if (response.IsSuccessStatusCode)
					return await response.Content.ReadAsAsync<List<RunnerSubmition>>();
			}
			catch (Exception e)
			{
				Console.WriteLine($@"Cant connect to {httpClient.BaseAddress}{uri}. {e.Message}");
				if (e.InnerException != null)
					Console.WriteLine(e.InnerException.Message);
			}
			return new List<RunnerSubmition>();
		}

		public async void SendResult(RunningResults result)
		{
			var uri = GetUri("PostResult");
			var responce = await httpClient.PostAsJsonAsync(uri, result);

			if (responce.IsSuccessStatusCode)
				return;

			Console.Error.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
			Console.Error.WriteLine(responce.ToString());
			Console.Error.WriteLine(result);
		}

		public async void SendResults(List<RunningResults> results)
		{
			var uri = GetUri("PostResults");
			var response = await httpClient.PostAsJsonAsync(uri, results);

			if (response.IsSuccessStatusCode)
				return;

			Console.Error.WriteLine("can't send " + DateTime.Now.ToString("HH:mm:ss"));
			Console.Error.WriteLine(response.ToString());
			Console.Error.WriteLine(response.Content.ReadAsStringAsync().Result);
			foreach (var result in results)
			{
				Console.Error.WriteLine(result);
			}
		}

		private string GetUri(string path, params string[][] parameters)
		{
			var query = HttpUtility.ParseQueryString(string.Empty);
			query["token"] = token;
			foreach (var parameter in parameters)
			{
				query[parameter[0]] = parameter[1];
			}
			return path + "/?" + query;
		}
	}
}