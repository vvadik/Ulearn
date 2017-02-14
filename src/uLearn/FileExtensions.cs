using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Ionic.Zip;

namespace uLearn
{
	public static class FileExtensions
	{
		public static void RemoveXmlDeclaration(this FileInfo file)
		{
			var text = File.ReadAllText(file.FullName);
			const string xmlDeclaration = @"<?xml version=""1.0"" encoding=""utf-8""?>";
			var startIndex = text.StartsWith(xmlDeclaration) ? xmlDeclaration.Length : 0;
			while (startIndex < text.Length && char.IsWhiteSpace(text[startIndex]))
				startIndex++;
			File.WriteAllText(file.FullName, text.Substring(startIndex));
		}

		public static string ContentAsUtf8(this FileInfo file)
		{
			return File.ReadAllText(file.FullName, Encoding.UTF8);
		}

		public static byte[] Content(this FileInfo file)
		{
			return File.ReadAllBytes(file.FullName);
		}

		public static T DeserializeXml<T>(this FileInfo file)
		{
			var serializer = new XmlSerializer(typeof(T));
			using (var stream = file.OpenRead())
				return (T)serializer.Deserialize(stream);
		}

		public static T DeserializeXml<T>(this string content)
		{
			var serializer = new XmlSerializer(typeof(T));
			using (var stream = new StringReader(content))
				return (T)serializer.Deserialize(stream);
		}

		public static string GetContent(this DirectoryInfo di, string filepath)
		{
			return di.GetBytes(filepath).AsUtf8();
		}

		public static byte[] GetBytes(this DirectoryInfo di, string filepath)
		{
			var fileInfo = di.GetFile(filepath);
			if (fileInfo.Exists)
				return File.ReadAllBytes(fileInfo.FullName);
			throw new Exception("No " + filepath + " in " + di.FullName);
		}

		/// <param name="excludeCriterias"><see cref="M:Ionic.Zip.ZipFile.AddSelectedFiles(System.String)" /></param>
		public static byte[] ToZip(this DirectoryInfo dirInfo, IEnumerable<string> excludeCriterias, IEnumerable<FileContent> toUpdate = null)
		{
			using (var zip = new ZipFile())
			{
				zip.AddDirectory(dirInfo.FullName);
				var entriesToRemove = excludeCriterias
					.Select(zip.SelectEntries)
					.SelectMany(x => x)
					.ToList();
				zip.RemoveEntries(entriesToRemove);
				foreach (var zipUpdateData in toUpdate ?? new List<FileContent>())
					zip.UpdateEntry(zipUpdateData.Path, zipUpdateData.Data);
				using (var ms = new MemoryStream())
				{
					zip.Save(ms);
					return ms.ToArray();
				}
			}
		}

		public static bool IsInDirectory(this FileSystemInfo fileOrDirectory, DirectoryInfo directory)
		{
			return fileOrDirectory.FullName.ToLower().StartsWith(directory.FullName.ToLower());
		}
	}
}