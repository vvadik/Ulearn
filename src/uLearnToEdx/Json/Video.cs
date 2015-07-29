using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
