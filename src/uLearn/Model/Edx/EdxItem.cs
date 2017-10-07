using System;
using System.IO;
using System.Xml.Serialization;
using uLearn.Extensions;

namespace uLearn.Model.Edx
{
	public abstract class EdxItem
	{
		[XmlAttribute("url_name")]
		public string UrlName { get; set; }

		public virtual bool ShouldSerializeUrlName()
		{
			return false;
		}

		[XmlAttribute("display_name")]
		public string DisplayName { get; set; }

		[XmlIgnore]
		public virtual string SubfolderName { get; set; }

		public virtual void Save(string folderName)
		{
			Save(folderName, true);
		}

		public void Save(string folderName, bool withAdditionals)
		{
			var path = Path.Combine(folderName, SubfolderName);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			File.WriteAllText(Path.Combine(path, UrlName + ".xml"), this.XmlSerialize());
			if (withAdditionals)
				SaveAdditional(folderName);
		}

		public virtual void SaveAdditional(string folderName)
		{
		}

		public static TComponent Load<TComponent>(string folderName, string type, string urlName, EdxLoadOptions options, Action<TComponent> loadInner = null) where TComponent : EdxItem
		{
			try
			{
				var fileInfo = new FileInfo($"{folderName}/{type}/{urlName}.xml");
				if (!fileInfo.Exists)
				{
					if (options.FailOnNonExistingItem)
						throw new FileNotFoundException($"File {fileInfo.FullName} not found.");
					else
					{
						options.HandleNonExistentItemTypeName?.Invoke(type, urlName);
						return null;
					}
				}
				var component = fileInfo.DeserializeXml<TComponent>();
				component.UrlName = urlName;
				loadInner?.Invoke(component);
				return component;
			}
			catch (Exception e)
			{
				throw new Exception($"{type} {urlName} load error", e);
			}
		}
	}
}