using System.IO;
using System.Linq;
using SKBKontur.SpellChecker;

namespace uLearn
{
	public static class SpellCheckExtentions
	{
		private static readonly string[] dictionary =
		{
			"хеш", "хеша", "хешу", "хешем", "хеше", "хеши", "хешей", "хешам", "хешами", "хешах",
			"серверный", "серверного", "серверному", "серверным", "серверном", "серверное", "серверная", "серверной", "серверную", "серверною", "серверные", "серверных", "серверным", "серверными",
			"википедия", "википедии", "википедию", "википедией", "википедиею", "википедий", "википедиям", "википедиями", "википедиях",
			"лямбда", "лямбды", "лямбде", "лямбду", "лямбдой", "лямбд", "лямбдам", "лямбдами", "лямбдах"
		};

		public static string[] SpellCheck(this Course course)
		{
			using (var spellchecker = SpellChecker(course.TryGetDictionaryPath()))
			{
				return spellchecker.SpellCheckCourse(course);
			}
		}

		public static string[] SpellCheckCourse(this SpellChecker spellchecker, Course course)
		{
			var titleErrors = spellchecker.SpellCheckString(course.Title).Select(e => e.ToPrettyString());
			var unitsErrors = course.GetUnits().SelectMany(spellchecker.SpellCheckString).Select(e => e.ToPrettyString());
			var slidesErrors = course.Slides.SelectMany(spellchecker.SpellCheckSlide);
			return titleErrors.Concat(unitsErrors).Concat(slidesErrors).ToArray();
		}

		public static string[] SpellCheckSlide(this SpellChecker spellchecker, Slide slide)
		{
			var prefix = string.Format("{0} ({1}): ", slide.Title, slide.Id);
			var titleErrors = spellchecker.SpellCheckString(slide.Title);
			var blocksErrors = slide.Blocks.Select(b => b.TryGetText()).Where(s => !string.IsNullOrWhiteSpace(s)).SelectMany(spellchecker.SpellCheckString);
			return titleErrors.Concat(blocksErrors).Select(e => prefix + e.ToPrettyString()).ToArray();
		}

		public static string ToPrettyString(this SpellingError error)
		{
			return string.Format("Найдено '{0}', возможные варианты: [{1}]", error.Mispelling, string.Join(", ", error.Suggestions));
		}

		private static string TryGetDictionaryPath(this Course course)
		{
			var file = Path.Combine(course.Directory.FullName, course.Settings.GetDictionaryFile());
			return File.Exists(file) ? file : null;
		}

		private static SpellChecker SpellChecker(string dictionaryFile)
		{
			var spellChecker = new SpellChecker(dictionaryFile);
			foreach (var word in dictionary)
			{
				spellChecker.AddWord(word);
			}
			return spellChecker;
		}
	}
}