using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn
{
	public static class StreamExtensions
	{
		public static string GetString(this Stream inputStream)
		{
			var codeBytes = new MemoryStream();
			inputStream.CopyTo(codeBytes);
			return Encoding.UTF8.GetString(codeBytes.ToArray());
		}
	}
}