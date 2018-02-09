using System.Linq;
using uLearn.CSharp;
using uLearn.Model.Blocks;

namespace uLearn
{
	public class ValidatorsRepository
	{
		public static ISolutionValidator Get(ValidatorDescription validatorDescription)
		{
			var name = validatorDescription.ValidatorName ?? "";
			var parts = name.ToLower().Split(' ');
			if (parts.Contains("cs"))
			{
				var validator = new CSharpSolutionValidator(validatorDescription.RemoveDefaults);
				foreach (var part in parts)
				{
					var pp = part.Split('-');
					var subValidator = pp[0];
					if (subValidator == "singlestatementmethod")
						validator.AddValidator(new SingleStatementMethodValidator());
					if (subValidator == "singlestaticmethod")
						validator.AddValidator(new IsStaticMethodValidator());
					if (subValidator == "blocklen")
						validator.AddValidator(new BlockLengthStyleValidator(int.Parse(pp[1])));
					if (subValidator == "linelen")
						validator.AddValidator(new LineLengthStyleValidator(int.Parse(pp[1])));
					if (subValidator == "recursion")
						validator.AddValidator(new RecursionStyleValidator(true));
					if (subValidator == "norecursion")
						validator.AddValidator(new RecursionStyleValidator(false));
				}
				return validator;
			}
			return new NullValidator();
		}
	}
}