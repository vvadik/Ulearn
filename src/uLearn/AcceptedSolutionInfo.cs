using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace uLearn.Web.Models
{
	public class AcceptedSolutionInfo
	{
		public string Code { get; set; }
		public int Id { get; set; }

		public AcceptedSolutionInfo(string code, int id)
		{
			Code = code;
			Id = id;
		}
	}
}