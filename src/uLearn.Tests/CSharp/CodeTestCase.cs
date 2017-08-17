namespace uLearn.CSharp
{
	public class CodeTestCase
	{
		public string Name { get; set; }
		public string Code { get; set; }
		public string Warning { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}