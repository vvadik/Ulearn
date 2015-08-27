using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RunCsJob;
using RunCsJob.Api;
using uLearn.Web.Models;

namespace uLearn.CourseTool
{
	class HttpServer
	{
		private readonly HttpListener listener;
		private readonly string directory;
		private bool needGlobalRefresh;
		public Course course;

		public HttpServer(string dir, int port)
		{
			listener = new HttpListener();
			listener.Prefixes.Add(string.Format("http://+:{0}/", port));
			directory = dir;
			needGlobalRefresh = false;
		}

		public void Start()
		{
			listener.Start();
			StartListen();
		}

		public async void UpdateAll()
		{
			needGlobalRefresh = true;
			await Task.Delay(2000);
			needGlobalRefresh = false;
		}

		public async void StartListen()
		{
			while (true)
			{
				try
				{
					var context = await listener.GetContextAsync();
					StartHandle(context);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		private void StartHandle(HttpListenerContext context)
		{
			Task.Run(async () =>
			{
				var ctx = context;
				try
				{
					await OnContextAsync(ctx);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
				finally
				{
					ctx.Response.Close();
				}
			});
		}

		private async Task OnContextAsync(HttpListenerContext context)
		{
			var query = context.Request.QueryString["query"];
			var path = context.Request.Url.LocalPath;
			Console.WriteLine("{0} {1} HTTP/{2}", context.Request.HttpMethod, context.Request.Url, context.Request.ProtocolVersion);
			byte[] response;
			if (query == "needRefresh")
			{
				var sw = Stopwatch.StartNew();
				while (true)
				{
					if (needGlobalRefresh || sw.Elapsed > TimeSpan.FromSeconds(10))
					{
						response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(needGlobalRefresh));
						break;
					}
					await Task.Delay(1000);
				}
			}
			else if (query == "submit")
			{
				var code = context.Request.InputStream.GetString();
				var exercise = ((ExerciseSlide)course.Slides[int.Parse(path.Substring(1, 3))]).Exercise;
				var solution = exercise.Solution.BuildSolution(code).SourceCode;
				var result = SandboxRunner.Run(new RunnerSubmition { Code = solution, Id = Utils.NewNormalizedGuid(), Input = "", NeedRun = true });
				context.Response.Headers["Content-Type"] = "application/json; charset=utf-8";
				response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new RunSolutionResult
				{
					IsRightAnswer = result.Verdict == Verdict.Ok && result.GetOutput().NormalizeEoln() == exercise.ExpectedOutput.NormalizeEoln(), 
					ActualOutput = result.GetOutput().NormalizeEoln(), 
					CompilationError = result.CompilationOutput, 
					ExecutionServiceName = "this", 
					IsCompileError = result.Verdict == Verdict.CompilationError,
					ExpectedOutput = exercise.ExpectedOutput
				}));
			}
			else
			{
				try
				{
					response = File.ReadAllBytes(directory + "/" + path);
				}
				catch (IOException e)
				{
					context.Response.StatusCode = 404;
					context.Response.Headers["Content-Type"] = "text/plain; charset=utf-8";
					response = Encoding.UTF8.GetBytes(e.ToString());
				}
			}
			await context.Response.OutputStream.WriteAsync(response, 0, response.Length);
			context.Response.OutputStream.Close();
		}
	}
}
