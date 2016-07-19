using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using RunCsJob.Api;
using uLearn;
using System.Linq;

namespace RunCsJob
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string uri, T payload)
        {
            var serializedPayload = JsonConvert.SerializeObject(payload, Formatting.Indented);
            return await client.PostAsync(
                uri,
                new StringContent(serializedPayload, Encoding.UTF8, "application/json"),
                CancellationToken.None);
        }

        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            var s = await content.ReadAsStringAsync();
            Console.WriteLine(s);
            return JsonConvert.DeserializeObject<T>(s, JsonConfig.GetSettings());
        }
    }

    [TestFixture]
    public class Json_Test
    {
        private static readonly ProjRunnerSubmition projSubmission = new ProjRunnerSubmition();
        private static FileRunnerSubmition fileSubmition = new FileRunnerSubmition();


        [Test]
        public void Test_Serialization_and_Deserialization()
        {
            var inputList = new List<RunnerSubmition>
            {
                new ProjRunnerSubmition
                {
                    Id = "E26C2109-F074-4117-B53F-0799E4140DEF",
                    Input = "",
                    NeedRun = true,
                    ProjectFileName = "proj",
                    ZipFileData = new byte[] { 1, 2, 3, 4, 5, 61, 23, 4, 3 }
                },
                new FileRunnerSubmition
                {
                    Code = "code",
                    Id = "E9D8C168-A3D9-48CC-AF60-CE3B8A1D8314",
                    Input = "",
                    NeedRun = true
                }
            };
            var json = JsonConvert.SerializeObject(inputList, JsonConfig.GetSettings());
            var deserializedList = JsonConvert.DeserializeObject<List<RunnerSubmition>>(json, JsonConfig.GetSettings());
            Assert.That(deserializedList.Count, Is.EqualTo(inputList.Count));
            for (int i = 0; i < inputList.Count; i++)
                Assert.That(deserializedList[i].GetType(), Is.EqualTo(inputList[i].GetType()));
        }

        [Test]
        public void Test_Serialization()
        {
            var list = new List<RunnerSubmition>
            {
                new ProjRunnerSubmition
                {
                    Id = "E26C2109-F074-4117-B53F-0799E4140DEF",
                    Input = "",
                    NeedRun = true,
                    ProjectFileName = "proj",
                    ZipFileData = new byte[] { 1, 2, 3, 4, 5, 61, 23, 4, 3 }
                }
            };

            var json = JsonConvert.SerializeObject(list, JsonConfig.GetSettings());
            Assert.That(json, Is.EqualTo("[{\"$type\":\"proj\",\"ZipFileData\":\"AQIDBAU9FwQD\",\"ProjectFileName\":\"proj\",\"Id\":\"E26C2109-F074-4117-B53F-0799E4140DEF\",\"Input\":\"\",\"NeedRun\":true}]"));
        }

        [Test]
        public
        void Test
            ()
        {
            var json = @"[{""$type"":""file"",""Code"":""code"",""Id"":""1029"",""Input"":"""",""NeedRun"":true}]";
            var list = JsonConvert.DeserializeObject<List<RunnerSubmition>>(json, JsonConfig.GetSettings());
            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list[0], Is.InstanceOf<FileRunnerSubmition>());
        }
    }
}