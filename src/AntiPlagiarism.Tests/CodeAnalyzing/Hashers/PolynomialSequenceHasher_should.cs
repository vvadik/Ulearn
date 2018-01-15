using AntiPlagiarism.Web.CodeAnalyzing.Hashers;
using NUnit.Framework;

namespace AntiPlagiarism.Tests.CodeAnalyzing.Hashers
{
	[TestFixture]
	public class PolynomialSequenceHasher_should
	{
		[Test]
		public void TestWithSimpleMultiplier()
		{
			var hasher = new PolynomialSequenceHasher(1);
			hasher.Enqueue("first");
			Assert.AreEqual("first".GetHashCode(), hasher.GetCurrentHash());
			hasher.Enqueue("second");
			Assert.AreEqual("first".GetHashCode() + "second".GetHashCode(), hasher.GetCurrentHash());
			hasher.Dequeue();
			Assert.AreEqual("second".GetHashCode(), hasher.GetCurrentHash());
		}
		
		[TestCase(2)]
		[TestCase(5)]
		[TestCase(137)]
		public void TestWithMultiplier(int multiplier)
		{
			var hasher = new PolynomialSequenceHasher(multiplier);
			hasher.Enqueue("first");
			Assert.AreEqual("first".GetHashCode(), hasher.GetCurrentHash());
			hasher.Enqueue("second");
			Assert.AreEqual(unchecked(multiplier * "first".GetHashCode() + "second".GetHashCode()), hasher.GetCurrentHash());
			hasher.Dequeue();
			Assert.AreEqual("second".GetHashCode(), hasher.GetCurrentHash());
		}

		[Test]
		public void TestDequeue()
		{
			const int count = 100;
			var hasher = new PolynomialSequenceHasher(137);
			for (var i = 0; i < count; i++)
				hasher.Enqueue("some_string");
			for (var i = 0; i < count - 1; i++)
				hasher.Dequeue();
			Assert.AreEqual("some_string".GetHashCode(), hasher.GetCurrentHash());
		}
		
		[TestCase(1000, 2)]
		[TestCase(2000, 5)]
		[TestCase(3000, 137)]
		public void TestWithManyAdds(int count, int multiplier)
		{
			var hasher = new PolynomialSequenceHasher(multiplier);
			var currentHash = 0;
			for (var i = 0; i < count; i++)
			{
				hasher.Enqueue("some_string");
				currentHash = unchecked(currentHash * multiplier + "some_string".GetHashCode());
				Assert.AreEqual(currentHash, hasher.GetCurrentHash());
			}
		}

		[TestCase(100)]
		public void TestReset(int count)
		{
			var hasher = new PolynomialSequenceHasher(137);
			for (var i = 0; i < count; i++)
			{
				hasher.Enqueue("some_string");
				Assert.AreEqual("some_string".GetHashCode(), hasher.GetCurrentHash());
				hasher.Reset();
			}
		}

		[Test]
		public void TestIntegerOverflow()
		{
			const int multiplier = 137;
			var result = 1;
			for (var i = 0; i < 100; i++)
				result = unchecked(result * multiplier);
			Assert.AreEqual(656577185, result);
		}
	}
}