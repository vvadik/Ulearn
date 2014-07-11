using System;
using System.Collections.Generic;
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

		public ExecutionService(string userName = DefaultName, string password = DefaultPassword, int timeout = 2000)
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
			await Task.Delay(4000);
			int count = 0;
			while (GetSubmitionStatus(link).Status != SubmitionStatus.Done && count < 10)
			{
				count++;
				await Task.Delay(3000);
			}
			if (count >= 10)
				throw new Exception("Ideone service process execution too slow. Can't wait any more.");
			return GetSubmitionDetails(link);
		}

		private Dictionary<string, string> ParseResponse(IEnumerable<object> response)
		{
			return
				response
					.OfType<XmlElement>()
					.Select(o => Tuple.Create(o.ChildNodes[0], o.ChildNodes[1]))
					.ToDictionary(x => x.Item1.InnerText, x => x.Item2.InnerText);
		}
	}
}