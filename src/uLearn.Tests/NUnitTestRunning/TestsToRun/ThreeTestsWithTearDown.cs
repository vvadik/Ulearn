using NUnit.Framework;

namespace uLearn.NUnitTestRunning.TestsToRun
{
	public class ThreeTestsWithTearDown
	{
		public static int Counter { get; set; }

		[TearDown]
		public void SetUp()
		{
			Counter++;
		}


		[Test]
		public void Test1()
		{
		}

		[Test]
		public void Test2()
		{
		}

		[Test]
		public void Test3()
		{
		}
	}
}
