using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using CommandLine;

namespace CourseToolHotReloader
{
	internal class Program
	{
		 private static IContainer container;

        private static void Main(string[] args)
        {
            AutofacDoMagic();

            Parser.Default.ParseArguments<Options>(args).WithParsed(Process);
        }

        private static void AutofacDoMagic()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new ControllerDependencyModule());
            container = containerBuilder.Build();
        }

        private static void Process(Options options)
        {
            WatchDirectory(Path.GetDirectoryName(options.Path), Debounce(RegisterUpdate));
        }

        private static void RegisterUpdate(object _, FileSystemEventArgs fileSystemEventArgs)
        {
            var courseUpdate = new CourseUpdate
            {
                Name = fileSystemEventArgs.Name,
                RelativePath = fileSystemEventArgs.FullPath
            };

            container.Resolve<ICourseUpdateQuery>().Push(courseUpdate);
            container.Resolve<ICourseUpdateSender>().SendCourseUpdates();
        }

        private static void WatchDirectory(string directory, FileSystemEventHandler handler)
        {
            using var watcher = new FileSystemWatcher
            {
                Path = directory,
                NotifyFilter = NotifyFilters.LastAccess
                               | NotifyFilters.LastWrite
                               | NotifyFilters.FileName
                               | NotifyFilters.DirectoryName,
                Filter = "*",
                IncludeSubdirectories = true
            };

            watcher.Changed += handler;
            watcher.Created += handler;
            watcher.Deleted += handler;

            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Press 'q' to quit");
            while (Console.Read() != 'q') ;
        }


        private static FileSystemEventHandler Debounce(FileSystemEventHandler func, int milliseconds = 1000)
        {
            CancellationTokenSource cancelTokenSource = null;
            return (arg1, arg2) =>
            {
                cancelTokenSource?.Cancel();
                cancelTokenSource = new CancellationTokenSource();

                Task.Delay(milliseconds, cancelTokenSource.Token)
                    .ContinueWith(t =>
                    {
                        if (t.IsCompletedSuccessfully)
                            func(arg1, arg2);
                    }, TaskScheduler.Default);
            };
        }
		
		private static void TestMain()
		{
			var task = GetJwtTokenModno(new LoginPasswordParameters { Login = "***", Password = "***" });
			task.Wait();
			var accountTokenResponseDto = task.Result;
			var token = accountTokenResponseDto.Token;
			Console.WriteLine(token);
		}

		private static async Task<AccountTokenResponseDto> GetJwtToken(LoginPasswordParameters parameters)
		{
			const string baseUrl = "http://localhost:8000";
			var url = $"{baseUrl}/account/login";
			//todo навернео в проекте есть способ делать запросы, но я пока так напишу, нужно сделать как и везде
			var request = WebRequest.Create(url);
			// а как сделать деконструкцию здесь???
			request.Method = "POST";
			request.ContentType = "application/json";

			var data = JsonSerializer.Serialize(parameters);
			var byteArray = Encoding.UTF8.GetBytes(data);
			request.ContentLength = byteArray.Length;

			await using var dataStream = request.GetRequestStream();
			dataStream.Write(byteArray, 0, byteArray.Length);

			using var response = await request.GetResponseAsync();
			await using var stream = response.GetResponseStream();
			return await JsonSerializer.DeserializeAsync<AccountTokenResponseDto>(stream);
		}

		private static async Task<AccountTokenResponseDto> GetJwtTokenModno(LoginPasswordParameters parameters)
		{
			const string baseUrl = "http://localhost:8000";
			var url = $"{baseUrl}/account/login";

			var json = JsonSerializer.Serialize(parameters);
			var data = new StringContent(json, Encoding.UTF8, "application/json");

			using var client = new HttpClient();

			var response = await client.PostAsync(url, data);

			var result = response.Content.ReadAsStringAsync().Result;
			return JsonSerializer.Deserialize<AccountTokenResponseDto>(result);
		}

		public class AccountTokenResponseDto
		{
			[JsonPropertyName("token")]
			public string Token { get; set; }

			[JsonPropertyName("status")]

			public string Status { get; set; }
		}

		private class LoginPasswordParameters
		{
			[JsonPropertyName("login")]
			public string Login { get; set; }

			[JsonPropertyName("password")]
			public string Password { get; set; }
		}
	}
}