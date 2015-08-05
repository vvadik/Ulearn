using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace uLearn.Model.Edx
{
	public abstract class EdxItem
	{
		[XmlIgnore]
		public string UrlName { get; set; }

		[XmlAttribute("display_name")]
		public virtual string DisplayName { get; set; }

		[XmlIgnore]
		public virtual string SubfolderName { get; set; }

		public virtual void Save(string folderName)
		{
			File.WriteAllText(string.Format("{0}/{1}/{2}.xml", folderName, SubfolderName, UrlName), this.XmlSerialize());
			SaveAdditional(folderName);
		}

		public virtual void SaveAdditional(string folderName)
		{
		}
	}
}
