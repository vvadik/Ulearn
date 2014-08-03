using System;
using System.Collections.Generic;
using System.Globalization;

namespace uLearn.Web.Ideone
{
	public class GetSubmitionDetailsResult : GetSubmitionStatusResult
	{
		public GetSubmitionDetailsResult(Dictionary<string, string> response) : base(response)
		{
		}

		public int LangId
		{
			get { return int.Parse(Get("langId")); }
		}

		public string LangName
		{
			get { return Get("langName"); }
		}

		public string LangVersion
		{
			get { return Get("langVersion"); }
		}

		public TimeSpan Time
		{
			get { return TimeSpan.FromSeconds(double.Parse(Get("time"), NumberFormatInfo.InvariantInfo)); }
		}

		public int Memory
		{
			get { return int.Parse(Get("memory")); }
		}

		public int Signal
		{
			get { return int.Parse(Get("signal")); }
		}

		public bool IsPublic
		{
			get { return bool.Parse(Get("public")); }
		}

		public DateTime Date
		{
			get { return DateTime.Parse(Get("date"), DateTimeFormatInfo.InvariantInfo); }
		}

		public string Source
		{
			get { return Get("source"); }
		}

		public string Input
		{
			get { return Get("input"); }
		}

		public string Output
		{
			get { return Get("output"); }
		}

		public string StdErr
		{
			get { return Get("stderr"); }
		}

		public string CompilationError
		{
			get { return Get("cmpinfo"); }
		}

		public override string ToString()
		{
			return string.Format("LangId: {0}\r\n LangName: {1}\r\n LangVersion: {2}\r\n Time: {3}\r\n Memory: {4}\r\n Signal: {5}\r\n IsPublic: {6}\r\n Date: {7}\r\n Source: {8}\r\n Input: {9}\r\n Output: {10}\r\n StdErr: {11}\r\n CompilationError: {12}", LangId, LangName, LangVersion, Time, Memory, Signal, IsPublic, Date, Source, Input, Output, StdErr, CompilationError);
		}
	}
}