using System;
using System.Net;
using System.Threading.Tasks;
using CourseToolHotReloader.ApiClient;
using CourseToolHotReloader.Log;

namespace CourseToolHotReloader.LoginAgent
{
	public interface ILoginAgent
	{
		Task<bool> SignIn();
	}

	public class LoginAgent : ILoginAgent
	{
		private readonly IConfig config;
		private readonly IUlearnApiClient ulearnApiClient;

		public LoginAgent(IConfig config, IUlearnApiClient ulearnApiClient)
		{
			this.config = config;
			this.ulearnApiClient = ulearnApiClient;
		}

		public async Task<bool> SignIn()
		{
			if (await TryLoginByConfig())
				return true;

			return await TryLoginByConsole();
		}

		private async Task<bool> TryLoginByConsole()
		{
			var login = ConsoleWorker.GetLogin();
			var password = new NetworkCredential(string.Empty, ConsoleWorker.GetPassword()).Password;

			try
			{
				config.JwtToken = await ulearnApiClient.Login(login, password);

				config.Flush();

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private async Task<bool> TryLoginByConfig()
		{
			if (config.JwtToken is null)
				return false;

			try
			{
				config.JwtToken = await ulearnApiClient.RenewToken();

				config.Flush();

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}