using System;

namespace test
{
	internal static class Program
	{
		private static void Main()
		{
			new Link.L();
			if (MeaningOfLifeTask.GetIt() != 42)
				Console.WriteLine("Not OK");
		}
	}
}