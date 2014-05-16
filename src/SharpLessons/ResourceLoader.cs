using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SharpLessons
{
	public class ResourceFile
	{
		public ResourceFile(string filename, string fullName)
		{
			Filename = filename;
			FullName = fullName;
		}

		public string Filename;
		public string FullName;

		public byte[] GetContent()
		{
			return ResourceLoader.LoadResource(FullName);
		}
	}

	public class ResourceLoader
	{
		private static string defaultNs = typeof (ResourceLoader).Namespace + ".";

		public static IEnumerable<ResourceFile> EnumerateResourcesFrom(string ns)
		{
			var prefix = defaultNs + ns + ".";
			return Assembly.GetExecutingAssembly().GetManifestResourceNames()
				.Where(name => name.StartsWith(prefix))
				.Select(name => new ResourceFile(name.Substring(prefix.Length), name));
		}

		public static byte[] LoadResource(string name)
		{
			if (name.StartsWith(defaultNs)) name = name.Substring(defaultNs.Length);
			var buffer = new MemoryStream();
			Stream stream = Assembly.GetExecutingAssembly()
				.GetManifestResourceStream(typeof (ResourceLoader), name);
			if (stream == null) throw new Exception("No resource " + name);
			stream.CopyTo(buffer);
			return buffer.ToArray();
		}
	}
}