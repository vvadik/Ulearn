using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace uLearn.Model.Edx
{
	public abstract class EdxItem
	{
		[XmlAttribute("url_name")]
		public string UrlName { get; set; }

		[XmlAttribute("display_name")]
		public virtual string DisplayName { get; set; }

		[XmlIgnore]
		public virtual string SubfolderName { get; set; }

		public virtual void Save(string folderName)
		{
			var path = Path.Combine(folderName, SubfolderName);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			File.WriteAllText(Path.Combine(path, UrlName + ".xml"), this.XmlSerialize());
			SaveAdditional(folderName);
		}

		public virtual void SaveAdditional(string folderName)
		{
		}
	}
}
