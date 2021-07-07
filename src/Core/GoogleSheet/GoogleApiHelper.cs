using System.Collections.Generic;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace Ulearn.Core.GoogleSheet
{
	public static class GoogleApiHelper
	{
		public static UserCredential Authorize(string[] scopes)
		{
			using var stream =
				new FileStream("credentials.json", FileMode.Open, FileAccess.Read);
			const string path = "token.json";
			return GoogleWebAuthorizationBroker.AuthorizeAsync(
				GoogleClientSecrets.FromStream(stream).Secrets,
				scopes,
				"user",
				CancellationToken.None,
				new FileDataStore(path, true)).Result;
			// Console.WriteLine("Credential file saved to: " + path);
		}

		public static SheetsService GetService(IConfigurableHttpClientInitializer initializer, string applicationName)
		{
			return new(new BaseClientService.Initializer
			{
				HttpClientInitializer = initializer,
				ApplicationName = applicationName,
			});
		}

		public static void FillSpreadSheetFromTable(SheetsService service, string spreadsheetId, TableForSheet tableForSheet)
		{
			var requests = new List<Request>();
			for (int i = 0; i < tableForSheet.Height; i++)
			{
				var values = new List<CellData>();
				for (int j = 0; j < tableForSheet.Width; j++)
				{
					values.Add(DataCreator.CreateCellData(tableForSheet.Cells[i, j]));
				}

				requests.Add(DataCreator.CreateCellUpdateRequest(tableForSheet.SheetNumber, values, i));
			}

			service.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest { Requests = requests },
					spreadsheetId)
				.Execute();
		}

		public static void DeleteFromSheet(SheetsService service, string spreadsheetId, string range)
		{
			var request = service.Spreadsheets.Values.Clear(new ClearValuesRequest(),
				spreadsheetId, range);
			request.Execute();
		}
	}
}