namespace Correct
{
	public class MultilineParameterListIncreasesIndentationLength
	{
		public static void Main(
			string[] args)
		{
		}

		public static void Main1(
			string[] args,
			string[] args1,
			string[] args3,
			string[] args2)
		{
		}


		public static void Main2(
			string[] args, string[] args1,
			string[] args3, string[] args2)
		{
		}

		public static void Main3(int a, int b,
			string[] args, string[] args1,
			string[] args3, string[] args2)
		{
		}

		public static void Main4(
			string[] args,
			string[] args1,
			string[] args3,
			string[] args2
		)
		{
		}

		public static void Main5(
			string[] args,
			string[] args1,
			string[] args3,
			string[] args2
			)
		{
		}
	}
}