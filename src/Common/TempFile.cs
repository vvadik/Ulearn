using System;
using System.IO;

namespace Ulearn.Common
{
	public class TempFile: IDisposable
	{
		public readonly FileInfo FileInfo;

		public TempFile(string fileName, Stream stream)
		{
			var path = Path.Combine(Path.GetTempPath(), fileName);
			using (var file = new FileStream(path, FileMode.Create, FileAccess.Write))
				stream.CopyTo(file);
			FileInfo = new FileInfo(path);
		}

		public void Dispose()
		{
			try
			{
				FuncUtils.TrySeveralTimes(() => FileInfo.Delete(), 3);
			}
			catch (Exception ex)
			{
				// ignore
			}
		}
	}
}