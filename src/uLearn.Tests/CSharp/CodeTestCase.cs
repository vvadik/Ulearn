namespace uLearn.CSharp
{
	public class CodeTestCase
	{
		public string FileName { get; set; }
		public string Warning { get; set; }

		public override string ToString()
		{
			return FileName;
		}
	}
}