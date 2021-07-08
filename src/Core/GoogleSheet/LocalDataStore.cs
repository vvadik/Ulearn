using System;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Json;
using Google.Apis.Util.Store;
using Ulearn.Core.Configuration;

namespace Ulearn.Core.GoogleSheet
{
    public class LocalDataStore : IDataStore
	{
		private readonly bool writeTokenToConsole;
		private readonly string accessToken;

		public LocalDataStore(bool writeTokenToConsole, string accessToken)
		{
			this.writeTokenToConsole = writeTokenToConsole;
			this.accessToken = accessToken;
		}

		public Task StoreAsync<T>(string key, T value)
		{
			if (!writeTokenToConsole)
				return Task.FromResult(0);
			var serialized = NewtonsoftJsonSerializer.Instance.Serialize(value);
			Console.WriteLine(serialized);
			return Task.FromResult(0);
		}

		public Task DeleteAsync<T>(string key)
        {
			return Task.FromResult(0);
        }

		public Task<T> GetAsync<T>(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("Key MUST have a value");
			}
			var tcs = new TaskCompletionSource<T>();
			tcs.SetResult(NewtonsoftJsonSerializer.Instance.Deserialize<T>(accessToken));
			return tcs.Task;
		}

		public Task ClearAsync()
        {
            throw new NotImplementedException();
        }
	}
}