using System;

namespace uLearn
{
	public class StagingPackage
	{
		public StagingPackage(string name, DateTime timestamp)
		{
			Name = name;
			Timestamp = timestamp;
		}

		public string Name;
		public DateTime Timestamp;
	}
}