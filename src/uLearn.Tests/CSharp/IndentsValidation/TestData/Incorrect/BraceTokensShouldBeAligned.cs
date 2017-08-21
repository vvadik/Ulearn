namespace uLearn.CSharp.IndentsValidation.TestData.Incorrect
	{
	public class BraceTokensShouldBeAligned
		{
		public static void Main(string[] args)
			{
				var a = 0;
		}

		public static void Main1(string[] args)
		{
			var c = 0;
			}

		public static void Main3(string[] args)
	{
			var c = 0;
			}

		public static void Main2(string[] args)
		{
			var c = new[] { 1, 2, 3 };

			var d = new[] {
				1, 2, 3
			};

			var e = new[] {
				new[] {
					1
				}, new[] {
					2
				}
			};
		}
	}
}