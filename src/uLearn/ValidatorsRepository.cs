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
						validator.AddValidator(new BlockLengthStyleValidator(int.Parse(pp[1])));
					if (subValidator == "linelen")
						validator.AddValidator(new LineLengthStyleValidator(int.Parse(pp[1])));
					if (subValidator == "recursion")
						validator.AddValidator(new RecursionStyleValidatorAttribute(true));
					if (subValidator == "norecursion")
						validator.AddValidator(new RecursionStyleValidatorAttribute(false));
				}
				return validator;
			}
			return new NullValidator();
		}
	}
}