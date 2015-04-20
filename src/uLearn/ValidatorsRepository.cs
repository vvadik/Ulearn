using System.Linq;
using uLearn.CSharp;

namespace uLearn
{
	public class ValidatorsRepository
	{
		public static ISolutionValidator NullValidator = new NullValidator();
		public static ISolutionValidator CSharpValidator = new CSharpSolutionValidator();
		public static ISolutionValidator CSharpSingleStatementValidator = new CSharpSolutionValidator().AddValidator(new SingleStatementMethodAttribute());

		public static ISolutionValidator Get(string name)
		{
			var parts = name.ToLower().Split(' ');
			if (parts.Contains("csharp"))
			{
				if (parts.Contains("singlestatementmethod"))
					return CSharpSingleStatementValidator;
				return CSharpValidator;
			}
			return NullValidator;
		}
	}
}