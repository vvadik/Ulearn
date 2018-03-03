using System.Collections.Generic;
using System.Linq;
using uLearn.CSharp;
using uLearn.Model.Blocks;
using Ulearn.Common.Extensions;

namespace uLearn.Model
{
	public interface ISingleRegionExtractor
	{
		string GetRegion(Label label);
	}

	public class RegionsExtractor
	{
		private readonly List<ISingleRegionExtractor> extractors;
		public readonly string file;
		public readonly string langId;

		public RegionsExtractor(string code, string langId, string file = null)
		{
			this.file = file;
			this.langId = langId;
			extractors = new List<ISingleRegionExtractor>
			{
				new CommonSingleRegionExtractor(code)
			};
			if (langId == "cs")
				extractors.Add(new CsMembersExtractor(code));
		}

		public string GetRegion(Label label)
		{
			return extractors.Select(extractor => extractor.GetRegion(label)).FirstOrDefault(res => res != null);
		}

		public IEnumerable<string> GetRegions(IEnumerable<Label> labels)
		{
			return labels.Select(GetRegion).Where(s => s != null).Select(s => s.FixExtraEolns());
		}
	}
}