using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.CourseTool
{
	class HttpServer
	{
		private readonly HttpListener listener;
		private readonly string directory;
		private const int timeout = 10;

		public HttpServer(string dir, int port)
		{
			listener = new HttpListener();
			listener.Prefixes.Add(string.Format("http://+:{0}/", port));
			listener.Prefixes.Add(string.Format("http://+:{0}/needRefresh/", port));
			directory = dir;
			Environment.CurrentDirectory = dir;
		}

		public void Start()
		{
			listener.Start();
			StartListen();
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
			context.Request.InputStream.Close();

			await Task.Delay(timeout);
			byte[] response;
			try
			{
				response = File.ReadAllBytes(directory + "/" + path);
			}
			catch (IOException e)
			{
				context.Response.StatusCode = 404;
				response = Encoding.UTF8.GetBytes("<!DOCTYPE html><html><head><meta charset='utf-8'></head><body>" + e.Message + "</body></html>");
			}

			await context.Response.OutputStream.WriteAsync(response, 0, response.Length);
			context.Response.OutputStream.Close();
			Console.WriteLine("{0}: {1} sent back to {2}", requestId, response, remoteEndPoint);
		}
	}
}
