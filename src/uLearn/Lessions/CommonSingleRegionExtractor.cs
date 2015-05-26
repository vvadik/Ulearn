using System.Collections.Generic;

namespace uLearn.Lessions
{
	public class CommonSingleRegionExtractor : ISingleRegionExtractor
	{
		private readonly Dictionary<string, RegionsParser.Region> regions;
		private readonly string code;

		public CommonSingleRegionExtractor(string code)
		{
			this.code = code;
			regions = RegionsParser.GetRegions(code);
		}

		public string GetRegion(Label label)
		{
			var region = regions.Get(label.Name, null);
			if (region == null)
				return null;
			return code.Substring(region.dataStart, region.dataLength).RemoveCommonNesting();
		}
	}
}