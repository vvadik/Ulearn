using System.Collections.Generic;
using System.Linq;
using uLearn.CSharp;

namespace uLearn
{
	public class ValidatorsRepository
	{
		public static ISolutionValidator Get(string name)
		{
			var parts = name.ToLower().Split(' ');
			if (parts.Contains("cs"))
			{
				var validator = new CSharpSolutionValidator();
				foreach (var part in parts)
				{
					var pp = part.Split('-');
					var subValidator = pp[0];
					if (subValidator == "singlestatementmethod")
						validator.AddValidator(new SingleStatementMethodAttribute());
					if (subValidator == "singlestaticmethod")
						validator.AddValidator(new IsStaticMethodAttribute());
					if (subValidator == "blocklen")
					{
						int maxLen = int.TryParse(pp[1], out maxLen) ? maxLen : -1;
						validator.AddValidator(new BlockLengthStyleValidator(maxLen));
					}
					if (subValidator == "recursion")
						validator.AddValidator(new RecursionStyleValidator());
				}
				return validator;
			}
			return new NullValidator();
		}
	}
}