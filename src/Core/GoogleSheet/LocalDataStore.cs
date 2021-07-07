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
		public LocalDataStore()
		{
		}
		
		public Task StoreAsync<T>(string key, T value)
        {
			var serialized = NewtonsoftJsonSerializer.Instance.Serialize(value);
			Console.WriteLine(serialized);
			return Task.FromResult(0);
		}

        public Task DeleteAsync<T>(string key)
        {
            throw new NotImplementedException();
        }

		public Task<T> GetAsync<T>(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("Key MUST have a value");
			}
			var tcs = new TaskCompletionSource<T>();
			var str = ApplicationConfiguration.Read<UlearnConfiguration>().GoogleAccessToken;
			tcs.SetResult(NewtonsoftJsonSerializer.Instance.Deserialize<T>(str));
			return tcs.Task;
		}

		public Task ClearAsync()
        {
            throw new NotImplementedException();
        }
	}
}