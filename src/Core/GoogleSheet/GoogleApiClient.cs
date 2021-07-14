using System.Collections.Generic;
using System.Linq;
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

		public GoogleApiClient(bool writeTokenToConsole, string accessToken, string credentialsJson)
		{
			this.writeTokenToConsole = writeTokenToConsole;
			this.accessToken = accessToken;
			var credentials = GetCredentials(credentialsJson);
			service = GetService(credentials);
		}

		private UserCredential GetCredentials(string credentialsJson)
		{
			var scopes = new[] { SheetsService.Scope.Spreadsheets };
			var secrets = JsonConvert.DeserializeObject<ClientSecrets>(credentialsJson);
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
			WhiteWashSheet(spreadsheetId, googleSheet.ListId, googleSheet.Width);
			var requests = RequestCreator.GetRequests(googleSheet);
			service.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest { Requests = requests },
				spreadsheetId).Execute();
			//TODO: проверить асинхронные методы
		}

		private void WhiteWashSheet(string spreadsheetId, int listId, int length)
		{
			var spreadsheet = service.Spreadsheets.Get(spreadsheetId).Execute();
			// var range = "";
			foreach (var sheet in spreadsheet.Sheets)
			{
				if (sheet.Properties.SheetId == listId)
					continue;
				// range = sheet.Properties.Title;
			}

			// var request = service.Spreadsheets.Values.Clear(new ClearValuesRequest(),
			// 	spreadsheetId, range);
			// request.Execute();
			var requests = new List<Request>
			{
				new()
				{
					DeleteDimension = new DeleteDimensionRequest
					{
						Range = new DimensionRange
						{
							Dimension = "COLUMNS",
							StartIndex = 0,
							EndIndex = spreadsheet.Sheets.FirstOrDefault(e => e.Properties.SheetId == listId)?.Properties.GridProperties.ColumnCount - 1,
							SheetId = listId
						}
					}
				},
				new()
				{
					InsertDimension = new InsertDimensionRequest
					{
						Range = new DimensionRange
						{
							Dimension = "COLUMNS",
							StartIndex = 0,
							EndIndex = length,
							SheetId = listId
						}
					}
				}
			};
			service.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest { Requests = requests },
				spreadsheetId).Execute();
		}
	}
}