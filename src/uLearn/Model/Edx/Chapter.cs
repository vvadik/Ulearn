using System.Linq;
using System.Xml.Serialization;

namespace uLearn.Model.Edx
{
	[XmlRoot("chapter")]
	public class Chapter : EdxItem
	{
		[XmlAttribute("display_name")]
		public string DisplayName;

		[XmlIgnore]
		public override string SubfolderName
		{
			get { return "chapter"; }
		}

		[XmlElement("sequential")]
		public SequentialReference[] SequentialReferences;

		[XmlIgnore]
		public Sequential[] Sequentials;

		public Chapter()
		{
		}

		public Chapter(string urlName, string displayName, Sequential[] sequentials)
		{
			DisplayName = displayName;
			UrlName = urlName;
			Sequentials = sequentials;
			SequentialReferences = sequentials.Select(x => x.GetReference()).ToArray();
		}

		public ChapterReference GetReference()
		{
			return new ChapterReference { UrlName = UrlName };
		}

		public override void SaveAdditional(string folderName)
		{
			foreach (var sequential in Sequentials)
				sequential.Save(folderName);
		}
	}
}
