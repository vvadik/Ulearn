using NUnit.Framework;
using uLearn.SpellChecking;

namespace uLearn
{
	class SpellChecking_Tests
	{
		[Test]
		public void Test()
		{
			CollectionAssert.IsEmpty(new SpellChecker().SpellCheckString("привет"));
			CollectionAssert.IsNotEmpty(new SpellChecker().SpellCheckString("превет"));
		}
	}
}