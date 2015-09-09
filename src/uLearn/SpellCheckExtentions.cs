using System.Linq;
using SKBKontur.SpellChecker;

namespace uLearn
{
	public static class SpellCheckExtentions
	{
		public static string[] SpellCheck(this string text)
		{
			using (var spellChecker = new SpellChecker())
			{
				return spellChecker.SpellCheckString(text)
					.Select(e => string.Format("Найдено '{0}', возможные варианты: [{1}]", e.Mispelling, string.Join(", ", e.Suggestions)))
					.ToArray();
			}
		}

		public static string[] SpellCheck(this string text, string prefix)
		{
			return text.SpellCheck().Select(e => string.Format("{0}: {1}", prefix, e)).ToArray();
		}
	}
}