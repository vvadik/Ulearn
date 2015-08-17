using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using uLearn.Model.Edx.EdxComponents;

namespace uLearn.Model.Edx
{
	public class OlxPatcher
	{
		private readonly string olxPath;

		public OlxPatcher(string olxPath)
		{
			this.olxPath = olxPath;
		}

		public void PatchComponents(EdxCourse course, IEnumerable<Component> components, bool replaceExisting = true)
		{
			var newVerticals = new List<Vertical>();
			foreach (var component in components)
			{
				var filename = string.Format("{0}/{1}/{2}.xml", olxPath, component.SubfolderName, component.UrlName);
				if (File.Exists(filename))
				{
					if (replaceExisting)
					{
						string displayName;
						try
						{
							var xml = new XmlDocument();
							xml.Load(filename);
							displayName = xml.DocumentElement.Attributes["display_name"].InnerText;
						}
						catch (Exception)
						{
							displayName = component.DisplayName;
						}
						component.DisplayName = displayName;
						component.Save(olxPath);
					}
				}
				else newVerticals.Add(new Vertical(Utils.NewNormalizedGuid(), component.DisplayName, new[] { component }));
			}
			Add(course, newVerticals.ToArray());
		}

		public void PatchVerticals(EdxCourse course, IEnumerable<Vertical[]> verticals, bool replaceExisting = true)
		{
			var newVerticals = new List<Vertical>();
			foreach (var subverticals in verticals)
			{
				var existsMap = subverticals.ToDictionary(x => x, x => File.Exists(string.Format("{0}/vertical/{1}.xml", olxPath, x.UrlName)));
				if (subverticals.Any(x => existsMap[x]))
				{
					if (replaceExisting)
					{
						foreach (var subvertical in subverticals)
							subvertical.Save(olxPath);

						if (subverticals.Length > 1)
							SaveSequentialContainingSubverticals(
								course,
								subverticals.Where(x => !existsMap[x]).ToArray(),
								subverticals.First(x => existsMap[x])
							);
					}
				}
				else newVerticals.AddRange(subverticals);
			}
			Add(course, newVerticals.ToArray());
		}

		public void Add(EdxCourse course, Vertical[] verticals)
		{
			if (verticals.Length != 0)
				course.CreateUnsortedChapter(olxPath, verticals);
		}

		private void SaveSequentialContainingSubverticals(EdxCourse course, IEnumerable<Vertical> subverticals, Vertical first)
		{
			var sequential = course.GetSequentialContainingVertical(first.UrlName);
			var verticalReferences = sequential.VerticalReferences.ToList();
			var firstReference = verticalReferences.Single(x => x.UrlName == first.UrlName);

			verticalReferences.InsertRange(
				verticalReferences.IndexOf(firstReference) + 1,
				subverticals.Select(x => new VerticalReference { UrlName = x.UrlName })
			);

			sequential.VerticalReferences = verticalReferences.ToArray();
			File.WriteAllText(string.Format("{0}/sequential/{1}.xml", olxPath, sequential.UrlName), sequential.XmlSerialize());
		}
	}
}
