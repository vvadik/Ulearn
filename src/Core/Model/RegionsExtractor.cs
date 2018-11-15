using System.Collections.Generic;
using System.Linq;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses.Slides.Blocks;
using Ulearn.Core.CSharp;

namespace Ulearn.Core.Model
{
	public interface ISingleRegionExtractor
	{
		string GetRegion(Label label, bool withoutAttributes);
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

		public string GetRegion(Label label, bool withoutAttributes=false)
		{
			return extractors.Select(extractor => extractor.GetRegion(label, withoutAttributes)).FirstOrDefault(res => res != null);
		}

		public IEnumerable<string> GetRegions(IEnumerable<Label> labels, bool withoutAttributes=false)
		{
			return labels.Select(l => GetRegion(l, withoutAttributes)).Where(s => s != null).Select(s => s.FixExtraEolns());
		}
	}
}