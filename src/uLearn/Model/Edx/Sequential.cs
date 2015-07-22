using System.Linq;
using System.Security.Policy;
using System.Xml.Serialization;

namespace uLearn.Model.Edx
{
	[XmlRoot("sequential")]
	public class Sequential : EdxItem
	{
		[XmlAttribute("display_name")]
		public string DisplayName;

		[XmlIgnore]
		public override string SubfolderName
		{
			get { return "sequential"; }
		}

		[XmlElement("vertical")]
		public VerticalReference[] VerticalReferences;

		[XmlIgnore]
		public Vertical[] Verticals;

		public Sequential()
		{
		}

		public Sequential(string urlName, string displayName, Vertical[] verticals)
		{
			UrlName = urlName;
			DisplayName = displayName;
			Verticals = verticals;
			VerticalReferences = verticals.Select(x => x.GetReference()).ToArray();
		}

		public SequentialReference GetReference()
		{
			return new SequentialReference { UrlName = UrlName };
		}

		public override void SaveAdditional(string folderName)
		{
			foreach (var vertical in Verticals)
				vertical.Save(folderName);
		}
	}
}
