using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunCsJob.Api
{
	public class TestsNotFoundFlag
	{
		public static string Flag
		{
			get
			{
				if (flag == null)
					flag = Guid.NewGuid();
				return flag.ToString();
			}
		}

		private static Guid? flag;
	}
}
