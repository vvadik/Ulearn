using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using uLearn.CSharp;

namespace uLearn.Lessions
{
	public interface ISingleRegionExtractor
	{
		string GetRegion(Label label);
	}

	public class RegionsExtractor
	{
		private readonly List<ISingleRegionExtractor> extractors;

		public RegionsExtractor(string code, string language)
		{
			extractors = new List<ISingleRegionExtractor>
			{
				new CommonSingleRegionExtractor(code)
			};
			if (language == "cs")
				extractors.Add(new CsMembersExtractor(code));
		}

		public string GetRegion(Label label)
		{
			return extractors.Select(extractor => extractor.GetRegion(label)).FirstOrDefault(res => res != null);
		}

		public IEnumerable<string> GetRegions(IEnumerable<Label> labels)
		{
			return labels.Select(GetRegion).Where(s => s != null).Select(FixEolns);
		}

		private static string FixEolns(string arg)
		{
			return Regex.Replace(arg.Trim(), "(\t*\r*\n){3,}", "\r\n\r\n");
		}
	}
}