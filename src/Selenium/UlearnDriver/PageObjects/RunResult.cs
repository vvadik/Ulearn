using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium.UlearnDriver.PageObjects
{
	public class RunResult
	{
		private readonly ResultType resultType;

		public RunResult(ResultType resultType)
		{
			this.resultType = resultType;
		}

		public ResultType GetResultType()
		{
			return resultType;
		}
	}

	public enum ResultType
	{
		ServiceError,
		CompileError,
		StyleError,
		Wa,
		WaNoDiff,
		Success
	}
}
