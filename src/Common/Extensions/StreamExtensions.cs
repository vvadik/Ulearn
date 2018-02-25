using System.IO;
using System.Text;

namespace Ulearn.Common.Extensions
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