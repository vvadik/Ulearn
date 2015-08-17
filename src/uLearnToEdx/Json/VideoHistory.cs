using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace uLearn.CourseTool.Json
{
	public class VideoHistory
	{
		public RecordHistory[] Records;

		public static VideoHistory FromVideo(Video video)
		{
			return new VideoHistory
			{
				Records = video.Records
					.Select(x => new RecordHistory { Data = new[] { x.Data }, Guid = x.Guid })
					.ToArray()
			};
		}

		public static VideoHistory UpdateHistory(string dir, Video video)
		{
			var videoHistoryJson = string.Format("{0}/videohistory.json", dir);
			VideoHistory videoHistory;
			if (File.Exists(videoHistoryJson))
			{
				var distinctIds = new List<string>();
				videoHistory = JsonConvert.DeserializeObject<VideoHistory>(File.ReadAllText(videoHistoryJson));
				foreach (var record in video.Records)
				{
					var recordHistory = videoHistory.Records.Single(x => x.Guid == record.Guid);
					if (recordHistory.Data.Last().Id != record.Data.Id)
					{
						var historyList = recordHistory.Data.ToList();
						historyList.Add(record.Data);
						recordHistory.Data = historyList.ToArray();
					}
					
					distinctIds = distinctIds
						.Distinct()
						.ToList();
					foreach (var data in recordHistory.Data)
					{
						if (distinctIds.Contains(data.Id))
							throw new Exception(string.Format("Video with id {0} was encountered more than one time.", data.Id));
					}
					distinctIds.AddRange(recordHistory.Data.Select(x => x.Id));
				}
				
				File.WriteAllText(videoHistoryJson, JsonConvert.SerializeObject(videoHistory));
				return videoHistory;
			}

			videoHistory = FromVideo(video);
			File.WriteAllText(videoHistoryJson, JsonConvert.SerializeObject(videoHistory));
			return FromVideo(video);
		}
	}

	public class RecordHistory
	{
		public Data[] Data;
		public Guid Guid;
	}
}
