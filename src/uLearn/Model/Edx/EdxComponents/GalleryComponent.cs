using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace uLearn.Model.Edx.EdxComponents
{
	[XmlRoot("html")]
	public class GalleryComponent : Component
	{
		[XmlIgnore]
		public string[] Images;

		[XmlIgnore]
		public string LocalFolder;

		[XmlAttribute("filename")]
		public string Filename;

		[XmlIgnore]
		public override string SubfolderName
		{
			get { return "html"; }
		}

		public GalleryComponent()
		{
		}

		public GalleryComponent(string urlName, string displayName, string filename, string localFolder, string[] images)
		{
			UrlName = urlName;
			DisplayName = displayName;
			Filename = filename;
			LocalFolder = localFolder;
			Images = images;
		}

		public override void Save(string folderName)
		{
			base.Save(folderName);
			File.WriteAllText(string.Format("{0}/{1}/{2}.html", folderName, SubfolderName, UrlName), AsHtmlString());
		}

		public override void SaveAdditional(string folderName)
		{
			foreach (var image in Images)
				File.Copy(string.Format("{0}/{1}", LocalFolder, image), string.Format("{0}/static/{1}_{2}", folderName, UrlName, image.Replace("/", "_")));
			File.WriteAllText(string.Format("{0}/static/gallery_{1}.html", folderName, UrlName),
				File.ReadAllText(string.Format("{0}/templates/gallery.html", Utils.GetRootDirectory()))
					.Replace("{0}", string.Join("", Images.Select(x => "<li><img src='" + UrlName + "_" + x.Replace("/", "_") + "' alt=''/></li>"))));
		}

		public override EdxReference GetReference()
		{
			return new HtmlComponentReference { UrlName = UrlName };
		}

		public override string AsHtmlString()
		{
			return File.ReadAllText(string.Format("{0}/templates/iframe.html", Utils.GetRootDirectory()))
				.Replace("{0}", "gallery_" + UrlName)
				.Replace("{1}", "(function (obj) { obj.style.height = '600px'; })(this);");
		}
	}
}
