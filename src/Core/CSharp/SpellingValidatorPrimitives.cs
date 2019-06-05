using System.Text;
using Ulearn.Common.Extensions;

namespace uLearn.CSharp
{
	public static class SpellingValidatorPrimitives
	{
		public static string MakeTypeNameAbbreviation(this string typeName)
		{
			var abbreviationBuilder = new StringBuilder();
			var wordsFromTypeName = typeName.SplitByCamelCase();
			foreach (var word in wordsFromTypeName)
			{
				abbreviationBuilder.Append(word[0]);
			}

			return abbreviationBuilder.ToString();
		}
	}
}