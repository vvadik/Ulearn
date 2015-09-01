using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Xml.Serialization;

namespace uLearn.Model.Edx
{
	[XmlRoot("sequential")]
	public class Sequential : EdxItem
	{
		[XmlIgnore]
		public override string SubfolderName
		{
			get { return "sequential"; }
		}

		private VerticalReference[] verticalReferences;

		[XmlElement("vertical")]
		public VerticalReference[] VerticalReferences {
			get { return verticalReferences ?? new VerticalReference[0]; }
			set { verticalReferences = value; } }

		[XmlAttribute("visible_to_staff_only")]
		public bool VisibleToStaffOnly { get; set; }


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

		public static Sequential Load(string folderName, string urlName)
		{
			var sequential = new FileInfo(string.Format("{0}/sequential/{1}.xml", folderName, urlName)).DeserializeXml<Sequential>();
			sequential.UrlName = urlName;
			sequential.Verticals = sequential.VerticalReferences.Select(x => Vertical.Load(folderName, x.UrlName)).ToArray();
			return sequential;
		}
	}
}
