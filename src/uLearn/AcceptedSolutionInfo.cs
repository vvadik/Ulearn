using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace uLearn.Web.Models
{
	public class AcceptedSolutionInfo
	{
		public string Code { get; private set; }
		public int Id { get; private set; }
		public List<string> UsersWhoLike { get; private set; }

		public AcceptedSolutionInfo(string code, int id, IEnumerable<string> usersWhoLike)
		{
			Code = code;
			Id = id;
			UsersWhoLike = usersWhoLike.ToList();
		}
	}
}