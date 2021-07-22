using System;
using System.IO;

namespace Ulearn.Common
{
	public class TempDirectory: IDisposable
	{
		public readonly DirectoryInfo DirectoryInfo;

		public TempDirectory(string directoryName)
		{
			var path = Path.Combine(Path.GetTempPath(), directoryName);
			DirectoryInfo = new DirectoryInfo(path);
			DirectoryInfo.Create();
		}

		public void Dispose()
		{
			try
			{
				FuncUtils.TrySeveralTimes(() => DirectoryInfo.Delete(true), 3);
			}
			catch (Exception ex)
			{
				// ignore
			}
		}
	}
}