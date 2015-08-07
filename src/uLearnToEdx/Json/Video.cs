using System;

namespace uLearnToEdx.Json
{
	public class Video
	{
		public Record[] Records;
	}

	public class Record
	{
		public Data Data;
		public Guid Guid;
	}

	public class Data
	{
		public string Description;
		public string Name;
		public string Id;
	}
}
