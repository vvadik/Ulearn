using System;
using System.Linq;
using NUnit.Framework;
using uLearn.SpellChecking;
using Ulearn.Common.Extensions;

namespace uLearn
{
	class SpellChecking_Tests
	{
		[Test]
		public void Test()
		{
			using (var spellChecker = new SpellChecker())
			{
				CollectionAssert.IsEmpty(spellChecker.SpellCheckString("привет"));
				CollectionAssert.IsNotEmpty(spellChecker.SpellCheckString("превет"));
				var dictionaryLines = Properties.Resources.customDictionary.SplitToLines();
				var errors = dictionaryLines.SelectMany(spellChecker.SpellCheckString).ToList();
				Console.WriteLine(string.Join("\r\n", errors));
				CollectionAssert.IsEmpty(errors);
			}
		}
	}
}