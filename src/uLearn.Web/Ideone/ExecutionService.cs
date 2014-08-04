using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Ideone;

namespace uLearn.Web.Ideone
{
	public partial class ExecutionService
	{
		private readonly string userName;
		private readonly string password;
		private readonly Ideone_Service_v1Service service = new Ideone_Service_v1Service();

		public ExecutionService(string userName = DefaultName, string password = DefaultPassword, int timeout = 20000)
		{
			this.userName = userName;
			this.password = password;
			service.Timeout = timeout;
		}

		public string CreateSubmition(string code, string input)
		{
			object[] response = service.createSubmission(userName, password, code, 27, input, true, true);
			var createSubmitionResult = new CreateSubmitionResult(ParseResponse(response));
			return createSubmitionResult.Link;
		}
		
		public IEnumerable<KeyValuePair<string, string>> GetSupportedLanguages()
		{
			object[] response = service.getLanguages(userName, password);
			var languagesNode = ParsePair(response.OfType<XmlElement>().ElementAt(1)).Value;
			return ParseResponse(languagesNode.ChildNodes.Cast<XmlNode>());
		}

		private KeyValuePair<string, XmlElement> ParsePair(XmlElement element)
		{
			return new KeyValuePair<string, XmlElement>(element.ChildNodes[0].InnerText, (XmlElement) element.ChildNodes[1]);
		}

		public GetSubmitionStatusResult GetSubmitionStatus(string link)
		{
			object[] response = service.getSubmissionStatus(userName, password, link);
			return new GetSubmitionStatusResult(ParseResponse(response));
		}

		public GetSubmitionDetailsResult GetSubmitionDetails(string link)
		{
			object[] response = service.getSubmissionDetails(userName, password, link, true, true, true, true, true);
			return new GetSubmitionDetailsResult(ParseResponse(response));
		}

		public async Task<GetSubmitionDetailsResult> Submit(string code, string input)
		{
			var link = CreateSubmition(code, input);
			await Task.Delay(1000);
			Debug.WriteLine("start checking status");
			int count = 0;
			var lastStatus = GetSubmitionStatus(link).Status;
			while (lastStatus != SubmitionStatus.Done && count < 10)
			{
				Debug.WriteLine("nope. wait");
				count++;
				await Task.Delay(1000);
				lastStatus = GetSubmitionStatus(link).Status;
			}
			if (lastStatus != SubmitionStatus.Done)
				throw new Exception("Ideone service process execution too slow. Can't wait any more.");
			Debug.WriteLine("requesting details");
			return GetSubmitionDetails(link);
		}

		private Dictionary<string, string> ParseResponse(IEnumerable<object> response)
		{
			return
				response
					.OfType<XmlElement>()
					.ToDictionary(x => x.ChildNodes[0].InnerText, x => x.ChildNodes[1].InnerText);
		}
	}
}