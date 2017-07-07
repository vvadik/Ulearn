using NUnit.Framework;

namespace uLearn.NUnitTestRunning.TestsToRun
{
	[TestFixture]
	public class ThreeTestsWithSecondFailing
	{
		[Test]
		public void Test1()
		{
		}

		[Test]
		public void Test2()
		{
			Assert.Fail("Message");
		}

		[Test]
		public void Test3()
		{
		}
	}
}
