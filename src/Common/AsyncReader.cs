using System.IO;
using System.Threading.Tasks;

namespace Ulearn.Common
{
	public class AsyncReader
	{
		private readonly char[] buffer;
		private readonly Task<int> readerTask;

		public AsyncReader(StreamReader reader, int length)
		{
			buffer = new char[length];
			readerTask = reader.ReadBlockAsync(buffer, 0, length);
		}

		public async Task<string> GetDataAsync()
		{
			var length = await readerTask;
			return new string(buffer, 0, length);
		}

		private bool IsCompleted => readerTask.IsCompleted;

		public int ReadedLength => IsCompleted ? readerTask.Result : -1;
	}
}