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

			var jwtToken = await ulearnApiClient.Login(login, password);
			
			return TrySetJwtTokenInConfig(jwtToken);
		}

		private async Task<bool> TryLoginByConfig()
		{
			if (config.JwtToken is null)
				return false;

			var jwtToken = await ulearnApiClient.RenewToken();

			return TrySetJwtTokenInConfig(jwtToken);
		}

		private bool TrySetJwtTokenInConfig(string jwtToken)
		{
			if (jwtToken is null)
				return false;

			config.JwtToken = jwtToken;
			config.Flush();

			return true;
		}
	}
}