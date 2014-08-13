namespace uLearn
{
	public class SlideInfo
	{
		public int Index { get; private set; }
		public string FileName { get; private set; }
		public string UnitName { get; private set; }
		public string CourseName { get; private set; }

		public SlideInfo(string fileName, string unitName, string courseName, int index)
		{
			Index = index;
			FileName = fileName;
			CourseName = courseName;
			UnitName = unitName;
		}
	}
}
