using System;
using System.Collections.Generic;
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
		private const int timeout = 10;
		private readonly Dictionary<string, bool> needRefresh; 

		public HttpServer(string dir, int port)
		{
			listener = new HttpListener();
			listener.Prefixes.Add(string.Format("http://+:{0}/", port));
			directory = dir;
			needRefresh = new Dictionary<string, bool>();
		}

		public void Start()
		{
			listener.Start();
			StartListen();
		}

		public async void AddUpdate(string name)
		{
			needRefresh[name] = true;
			await Task.Delay(2000);
			needRefresh[name] = false;
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
			Task.Run(
				async () =>
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
				}
				);
		}

		private async Task OnContextAsync(HttpListenerContext context)
		{
			var requestId = Guid.NewGuid();
			var query = context.Request.QueryString["query"];
			var path = context.Request.Url.LocalPath;
			var remoteEndPoint = context.Request.RemoteEndPoint;
			Console.WriteLine("{0}: received {1} from {2}", requestId, query, remoteEndPoint);
//			context.Request.InputStream.Close();
			await Task.Delay(timeout);
			byte[] response;
			if (query == "needRefresh")
			{
				var file = new FileInfo(path).Name;
				var refresh = needRefresh.ContainsKey(file) && needRefresh[file];
				response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(refresh));
			}
			else if (query == "submit")
			{
				var code = context.Request.InputStream.GetString();
				Console.WriteLine(code);
				var result = SandboxRunner.Run(new RunnerSubmition { Code = code, Id = Utils.NewNormalizedGuid(), Input = "", NeedRun = true });
				Console.WriteLine(result.Error);
				Console.WriteLine(result.Output);
				Console.WriteLine(result.CompilationOutput);
				response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new RunSolutionResult() { IsRightAnswer = true, ActualOutput = result.GetOutput(), CompilationError = result.CompilationOutput, ExecutionServiceName = "this", IsCompileError = result.Verdict == Verdict.CompilationError}));
				context.Response.Headers["Content-Type"] = "application/json; charset=utf-8";
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
					response = Encoding.UTF8.GetBytes("<!DOCTYPE html><html><head><meta charset='utf-8'></head><body>" + e.Message + "</body></html>");
				}
			}
			await context.Response.OutputStream.WriteAsync(response, 0, response.Length);
			context.Response.OutputStream.Close();
			Console.WriteLine("{0}: {1} sent back to {2}", requestId, response, remoteEndPoint);
		}
	}
}
