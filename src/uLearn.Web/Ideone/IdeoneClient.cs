using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Ideone;

namespace uLearn.Web.Ideone
{
	public partial class IdeoneClient
	{
		private readonly TimeSpan executionTimeout;
		private static readonly string DefaultName;
		private static readonly string DefaultPassword;

		public string UserName;
		public string Password;

		private readonly Ideone_Service_v1Service service = new Ideone_Service_v1Service();

		public IdeoneClient()
			: this(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30))
		{
		}

		public IdeoneClient(TimeSpan httpTimeout, TimeSpan executionTimeout)
		{
			this.executionTimeout = executionTimeout;
			UserName = DefaultName;
			Password = DefaultPassword;
			service.Timeout = (int)httpTimeout.TotalMilliseconds;
		}

		public string CreateSubmition(string code, string input)
		{
			object[] response = service.createSubmission(UserName, Password, code, 27, input, true, true);
			var createSubmitionResult = new CreateSubmitionResult(ParseResponse(response));
			return createSubmitionResult.Link;
		}

		public IEnumerable<KeyValuePair<string, string>> GetSupportedLanguages()
		{
			object[] response = service.getLanguages(UserName, Password);
			var languagesNode = ParsePair(response.OfType<XmlElement>().ElementAt(1)).Value;
			return ParseResponse(languagesNode.ChildNodes.Cast<XmlNode>());
		}

		private KeyValuePair<string, XmlElement> ParsePair(XmlElement element)
		{
			return new KeyValuePair<string, XmlElement>(element.ChildNodes[0].InnerText, (XmlElement)element.ChildNodes[1]);
		}

		public GetSubmitionStatusResult GetSubmitionStatus(string link)
		{
			object[] response = service.getSubmissionStatus(UserName, Password, link);
			return new GetSubmitionStatusResult(ParseResponse(response));
		}

		public GetSubmitionDetailsResult GetSubmitionDetails(string link)
		{
			object[] response = service.getSubmissionDetails(UserName, Password, link, true, true, true, true, true);
			return new GetSubmitionDetailsResult(ParseResponse(response));
		}

		public async Task<GetSubmitionDetailsResult> Submit(string code, string input)
		{
			var link = CreateSubmition(code, input);
			await Task.Delay(1000);
			Debug.WriteLine("start checking status");
			var sw = Stopwatch.StartNew();
			var lastStatus = GetSubmitionStatus(link).Status;
			while (lastStatus != SubmitionStatus.Done && sw.Elapsed < executionTimeout)
			{
				Debug.WriteLine("nope. wait");
				await Task.Delay(1000);
				lastStatus = GetSubmitionStatus(link).Status;
			}
			if (lastStatus != SubmitionStatus.Done)
			{
				Debug.WriteLine("Ideone service process execution too slow. Can't wait any more.");
				return null;
			}
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