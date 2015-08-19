using System.IO;
using System.Linq;
using System.Xml.Serialization;
using uLearn.Model.Edx.EdxComponents;

namespace uLearn.Model.Edx
{
	[XmlRoot("vertical")]
	public class Vertical : EdxItem
	{
		[XmlIgnore]
		public override string SubfolderName
		{
			get { return "vertical"; }
		}

		[XmlIgnore]
		public Component[] Components;

		[XmlElement("video", Type = typeof(VideoComponentReference))]
		[XmlElement("html", Type = typeof(HtmlComponentReference))]
		[XmlElement("problem", Type = typeof(ProblemComponentReference))]
		[XmlElement("lti", Type = typeof(LtiComponentReference))]
		public EdxReference[] ComponentReferences = new EdxReference[0];

		public Vertical()
		{
		}

		public Vertical(string urlName, string displayName, Component[] components)
		{
			UrlName = urlName;
			DisplayName = displayName;
			Components = components;
			ComponentReferences = components.Select(x => x.GetReference()).ToArray();
		}

		public VerticalReference GetReference()
		{
			return new VerticalReference { UrlName = UrlName };
		}

		public override void SaveAdditional(string folderName)
		{
			foreach (var component in Components)
				component.Save(folderName);
		}

		public static Vertical Load(string folderName, string urlName)
		{
			var vertical = new FileInfo(string.Format("{0}/vertical/{1}.xml", folderName, urlName)).DeserializeXml<Vertical>();
			vertical.UrlName = urlName;
			vertical.Components = vertical.ComponentReferences.Select(x => x.GetComponent(folderName, x.UrlName)).ToArray();
			return vertical;
		}
	}
}
