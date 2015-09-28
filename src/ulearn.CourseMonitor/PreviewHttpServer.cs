using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RunCsJob;
using RunCsJob.Api;
using uLearn.Model.Blocks;
using uLearn.Web.Models;

namespace uLearn.CourseTool
{
	class PreviewHttpServer
	{
		private readonly HttpListener listener;
		private readonly string courseDir;
		private readonly string htmlDir;
		private readonly int port;
		private DateTime lastChangeTime = DateTime.MinValue;
		public volatile Course course;
		private readonly object locker = new object();

		public PreviewHttpServer(string courseDir, string htmlDir, int port)
		{
			listener = new HttpListener();
			listener.Prefixes.Add(string.Format("http://+:{0}/", port));
			this.courseDir = courseDir;
			this.htmlDir = htmlDir;
			this.port = port;
			CopyStaticToHtmlDir();
		}

		private void CopyStaticToHtmlDir()
		{
			if (!Directory.Exists(htmlDir))
				Directory.CreateDirectory(htmlDir);
			var staticDir = Path.Combine(htmlDir, "static");
			if (!Directory.Exists(staticDir))
				Directory.CreateDirectory(staticDir);
			Utils.DirectoryCopy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "renderer"), htmlDir, true);
		}

		public void Start()
		{
			try
			{
				listener.Start();
			}
			catch (HttpListenerException e)
			{
				Console.WriteLine("HttpListener Start Error: {0}", e.Message);
				Console.WriteLine();
				Console.WriteLine("On 'access is denied' error do one of the following:");
				Console.WriteLine("1. Run this application with admin rights.");
				Console.WriteLine("2. OR run this command in command line ('Everyone' may be some specific user):");
				Console.WriteLine("   netsh http add urlacl url=http://+:{0}/ user=Everyone", port);
			}
			StartListen();
		}

		public void MarkCourseAsChanged()
		{
			lock (locker)
				lastChangeTime = DateTime.Now;
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
				catch (HttpListenerException e)
				{
					if (e.ErrorCode != 1229) throw;
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
			byte[] response;
			var requestTime = DateTime.Now;
			var reloaded = ReloadCourseIfChanged(requestTime);
			if (!new[]{".js", ".css", ".png", ".jpg", ".woff"}.Any(ext => path.EndsWith(ext)))
				Console.WriteLine("{0} {1} {2}", requestTime.ToString("T"), context.Request.HttpMethod, context.Request.Url);
			switch (query)
			{
				case "needRefresh":
					response = await ServeNeedRefresh(reloaded, requestTime);
					break;
				case "submit":
					response = ServeRunExercise(context, path);
					break;
				default:
					response = ServeStatic(context, path);
					break;
			}
			await context.Response.OutputStream.WriteAsync(response, 0, response.Length);
			context.Response.OutputStream.Close();
		}

		private async Task<byte[]> ServeNeedRefresh(bool reloaded, DateTime requestTime)
		{
			var sw = Stopwatch.StartNew();
			while (true)
			{
				if (reloaded || sw.Elapsed > TimeSpan.FromSeconds(20))
				{
					Console.WriteLine("needRefresh:{0}, LastChanged:{1}", reloaded, lastChangeTime);
					return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(reloaded));
				}
				await Task.Delay(1000);
				reloaded = ReloadCourseIfChanged(requestTime);
			}
		}

		private byte[] ServeRunExercise(HttpListenerContext context, string path)
		{
			var code = context.Request.InputStream.GetString();
			var exercise = ((ExerciseSlide)course.Slides[int.Parse(path.Substring(1, 3))]).Exercise;
			var runResult = GetRunResult(exercise, code);
			context.Response.Headers["Content-Type"] = "application/json; charset=utf-8";
			return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(runResult));
		}

		private static RunSolutionResult GetRunResult(ExerciseBlock exercise, string code)
		{
			var buildResult = exercise.Solution.BuildSolution(code);
			if (buildResult.HasErrors)
				return new RunSolutionResult { IsCompileError = true, CompilationError = buildResult.ErrorMessage, ExecutionServiceName = "uLearn" };
			if (buildResult.HasStyleIssues)
				return new RunSolutionResult { IsStyleViolation = true, CompilationError = buildResult.StyleMessage, ExecutionServiceName = "uLearn" };
			var solution = buildResult.SourceCode;
			var submission = new RunnerSubmition
			{
				Code = solution,
				Id = Utils.NewNormalizedGuid(),
				Input = "",
				NeedRun = true
			};
			var result = SandboxRunner.Run(submission);
			return new RunSolutionResult
			{
				IsRightAnswer = result.Verdict == Verdict.Ok && result.GetOutput().NormalizeEoln() == exercise.ExpectedOutput.NormalizeEoln(),
				ActualOutput = result.GetOutput().NormalizeEoln(),
				CompilationError = result.CompilationOutput,
				ExecutionServiceName = "this",
				IsCompileError = result.Verdict == Verdict.CompilationError,
				ExpectedOutput = exercise.ExpectedOutput.NormalizeEoln()
			};
		}

		private byte[] ServeStatic(HttpListenerContext context, string path)
		{
			byte[] response;
			try
			{
				response = File.ReadAllBytes(htmlDir + "/" + path);
			}
			catch (IOException e)
			{
				context.Response.StatusCode = 404;
				context.Response.Headers["Content-Type"] = "text/plain; charset=utf-8";
				response = Encoding.UTF8.GetBytes(e.ToString());
			}
			return response;
		}

		Course ReloadCourse()
		{
			var loadedCourse = new CourseLoader().LoadCourse(new DirectoryInfo(courseDir));
			var renderer = new SlideRenderer(new DirectoryInfo(htmlDir), loadedCourse);
			foreach (var slide in loadedCourse.Slides)
				renderer.RenderSlideToFile(slide, htmlDir);
			foreach (var unit in loadedCourse.GetUnits().Where(u => loadedCourse.FindInstructorNote(u) != null))
				renderer.RenderInstructorNotesToFile(unit, htmlDir);
			return loadedCourse;
		}

		private bool ReloadCourseIfChanged(DateTime requestTime)
		{
			lock (locker)
			{
				var needReload = lastChangeTime > requestTime.Add(TimeSpan.FromMilliseconds(500));
				if (needReload || course == null)
				{
					course = ReloadCourse();
					Console.WriteLine("Course reloaded. LastChangeTime: {0}", lastChangeTime);

				}
				return needReload;
			}
		}
	}
}
