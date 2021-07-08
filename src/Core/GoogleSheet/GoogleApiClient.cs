using System.Collections.Generic;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using Ulearn.Core.Configuration;

namespace Ulearn.Core.GoogleSheet
{
	public class GoogleApiClient
	{
		private readonly SheetsService service;
		private readonly bool writeTokenToConsole;
		private readonly string accessToken;
		
		public GoogleApiClient(bool writeTokenToConsole, string accessToken)
		{
			this.writeTokenToConsole = writeTokenToConsole;
			this.accessToken = accessToken;
			var credentials = GetCredentials();
			service = GetService(credentials);
		}

		private UserCredential GetCredentials()
		{
			var scopes = new[] { SheetsService.Scope.Spreadsheets };
			var credentials = ApplicationConfiguration.Read<UlearnConfiguration>().GoogleAccessCredentials;
			var secrets = JsonConvert.DeserializeObject<ClientSecrets>(credentials);
			return GoogleWebAuthorizationBroker.AuthorizeAsync(
				secrets,
				scopes,
				"user",
				CancellationToken.None,
				new LocalDataStore(writeTokenToConsole, accessToken)).Result;
		}

		private static SheetsService GetService(IConfigurableHttpClientInitializer initializer)
		{
			return new(new BaseClientService.Initializer
			{
				HttpClientInitializer = initializer
			});
		}

		public void FillSpreadSheet(string spreadsheetId, GoogleSheet googleSheet)
		{
			ClearSheet(spreadsheetId, googleSheet.ListId);
			var requests = RequestCreator.GetRequests(googleSheet);
			service.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest { Requests = requests },
				spreadsheetId).Execute();
			//TODO: проверить асинхронные методы
		}

		private void ClearSheet(string spreadsheetId, int listId)
		{
			var spreadsheet = service.Spreadsheets.Get(spreadsheetId).Execute();
			var range = "";
			foreach (var sheet in spreadsheet.Sheets)
			{
				if (sheet.Properties.SheetId == listId)
					range = sheet.Properties.Title;
			}
			var request = service.Spreadsheets.Values.Clear(new ClearValuesRequest(),
				spreadsheetId, range);
			request.Execute();
		}
	}
}