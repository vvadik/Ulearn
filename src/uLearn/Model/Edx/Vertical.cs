using System;
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
		public override string SubfolderName => "vertical";

		[XmlIgnore]
		public Component[] Components;

		private EdxReference[] componentReferences;

		[XmlElement("video", Type = typeof(VideoComponentReference))]
		[XmlElement("html", Type = typeof(HtmlComponentReference))]
		[XmlElement("problem", Type = typeof(ProblemComponentReference))]
		[XmlElement("lti", Type = typeof(LtiComponentReference))]
		public EdxReference[] ComponentReferences
		{
			get { return componentReferences = componentReferences ?? new EdxReference[0]; }
			set { componentReferences = value; }
		}

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
			try
			{
				var vertical = new FileInfo(string.Format("{0}/vertical/{1}.xml", folderName, urlName)).DeserializeXml<Vertical>();
				vertical.UrlName = urlName;
				vertical.Components = vertical.ComponentReferences.Select(x => x.GetComponent(folderName, x.UrlName)).ToArray();
				return vertical;
			}
			catch (Exception e)
			{
				throw new Exception(string.Format("Vertical {0} load error", urlName), e);
			}
		}
	}
}
