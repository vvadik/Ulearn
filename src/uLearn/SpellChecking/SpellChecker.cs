using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NHunspell;
using uLearn.Properties;

namespace uLearn.SpellChecking
{
	public class SpellingError
	{
		private bool Equals(SpellingError other)
		{
			return string.Equals(Mispelling, other.Mispelling);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((SpellingError)obj);
		}

		public override int GetHashCode()
		{
			return (Mispelling != null ? Mispelling.GetHashCode() : 0);
		}

		public string Mispelling { get; set; }

		public string Context { get; set; }

		public string[] Suggestions { get; set; }
	}

	public class SpellChecker : IDisposable
	{
		private readonly Regex wordRegEx = new Regex(@"\b(?<word>\w+)(?<dotEnding>\.\s*)?", RegexOptions.Compiled);
		private readonly Regex camelCaseRegEx = new Regex(@"[А-Яа-я][a-я]*(([А-Я][а-я]*)+)", RegexOptions.Compiled);

		private readonly string[] prefixes;

		private readonly HashSet<char> vowels = new HashSet<char>(new[] { 'а', 'о', 'у', 'ы', 'и', 'е', 'ю', 'я', 'ё', 'э' });

		private readonly Hunspell hunspell;

		public SpellChecker(string customDictionaryPath)
		{
			prefixes = Resources.customPrefixes.SplitToLines();
			hunspell = new Hunspell(Resources.ru_RU_aff, Resources.ru_RU_dic);

			InitializeInternalCustomDicionary(hunspell);

			if (!string.IsNullOrEmpty(customDictionaryPath))
			{
				InitializeCustomDictionary(hunspell, customDictionaryPath);
			}
		}

		public SpellChecker() : this("") { }

		private void InitializeCustomDictionary(Hunspell _hunspell, string[] dictionaryLines)
		{
			foreach (var dictionaryLine in dictionaryLines)
			{
				if (string.IsNullOrEmpty(dictionaryLine) || dictionaryLine.StartsWith("#"))
					continue;

				var tokens = dictionaryLine.Split(':');
				if (tokens.Length == 1)
				{
					_hunspell.Add(tokens[0].Trim());
				}
				else
				{
					_hunspell.AddWithAffix(tokens[0].Trim(), tokens[1].Trim());
				}
			}
		}

		private void InitializeCustomDictionary(Hunspell _hunspell, string customDictionaryPath)
		{
			var dictionaryLines = File.ReadAllLines(customDictionaryPath);
			InitializeCustomDictionary(_hunspell, dictionaryLines);
		}

		private void InitializeInternalCustomDicionary(Hunspell _hunspell)
		{
			var dictionaryLines = Resources.customDictionary.SplitToLines();
			InitializeCustomDictionary(_hunspell, dictionaryLines);
		}

		public void AddWord(string customWord)
		{
			hunspell.Add(customWord);
		}

		public void AddWord(string customWord, string similarWord)
		{
			hunspell.AddWithAffix(customWord, similarWord);
		}

		public SpellingError[] SpellCheckString(string str)
		{
			return SpellCheckString(str, "");
		}

		private SpellingError[] SpellCheckString(string str, string context)
		{
			var wordCandidates = wordRegEx.Matches(str).Cast<Match>();
			var wordList = new HashSet<string>();

			foreach (var wordCandidate in wordCandidates)
			{
				var word = wordCandidate.Groups["word"].Value.Replace('Ё', 'Е').Replace('ё', 'е');
				var dotEnding = wordCandidate.Groups["dotEnding"];

				//we will check the word only if it is not shortening
				if (!dotEnding.Success)
				{
					wordList.Add(word);
				}
				else
				{
					//dot was sentence sign
					var nextCharIndex = dotEnding.Index + dotEnding.Length;
					if (nextCharIndex >= str.Length || char.IsUpper(str[nextCharIndex]))
					{
						wordList.Add(word);
					}
				}
			}

			var spellingErrors = wordList.Select(x => SpellWord(x, context))
									  .Where(spellingError => spellingError != null)
									  .ToArray();
			return spellingErrors;
		}


		private SpellingError SpellWord(string word, string context)
		{
			//no any russian letter - ignore it
			if (!Regex.IsMatch(word, @"\p{IsCyrillic}"))
				return null;

			//digits in word - it is not word
			if (word.Any(char.IsDigit))
				return null;

			//WORD_WITH_UNDERSCORE - probably xml tag
			if (word.Contains("_"))
				return null;

			var normalizedWord = new string(word.Where(IsRussianLetter).ToArray());

			//two-letters word - ignore it, who knows what they mean
			if (normalizedWord.Length <= 2)
				return null;

			//it's not word
			if (normalizedWord.Length > 40)
				return null;

			// SomeWordInCamelCase - ignore it
			if (camelCaseRegEx.IsMatch(normalizedWord) && normalizedWord.Any(char.IsLower))
				return null;

			var abbreviationPartLength = normalizedWord.TakeWhile(char.IsUpper).Count();
			// it is abbreviation or its wordform - ignore it
			if (abbreviationPartLength >= 3)
			{
				var suffixSize = (normalizedWord.Length - abbreviationPartLength);
				var wordFormPartExist = (1 <= suffixSize && suffixSize <= 2);
				var appropriateSize = suffixSize == 0 && abbreviationPartLength <= 5;
				if (wordFormPartExist || appropriateSize)
					return null;
			}

			if (hunspell.Spell(normalizedWord) || hunspell.Spell(normalizedWord.ToLower()))
				return null;

			var suggests = hunspell.Suggest(normalizedWord);

			//One does not simply make error without suggestions. It's not word at all
			if (suggests.Count == 0)
				return null;

			foreach (var suggest in suggests)
			{
				var suggestionParts = suggest.Split(' ');

				//if word = prefix + stem of right spelling, ignore it
				if (suggestionParts.Length == 2 &&
					normalizedWord.Equals(suggestionParts[0] + suggestionParts[1], StringComparison.InvariantCultureIgnoreCase))
				{
					var prefix = suggestionParts[0].ToLower();
					if (prefixes.Contains(prefix))
						return null;

					//"не" можно писать слитно, но только не с глаголом
					if (prefix == "не" && !IsVerb(suggestionParts[1]))
					{
						return null;
					}
				}
			}

			return new SpellingError
			{
				Mispelling = word,
				Suggestions = suggests.ToArray(),
				Context = context
			};

		}

		private bool IsVerb(string word)
		{
			var verbConsonants = new[] { 'с', 'з' };

			var verbSuffixes = new[] { "ть", "чь", "тся", "ться", "чься" };

			if (word.Length < 3)
				return false;

			var stems = hunspell.Stem(word);
			if (!stems.Any())
				return false;

			var stem = stems[0];

			if (verbSuffixes.Any(verbSuffix => stem.EndsWith(verbSuffix) && (vowels.Contains(stem[stem.Length - verbSuffix.Length - 1]) || verbConsonants.Contains(stem[stem.Length - verbSuffix.Length - 1]))))
			{
				return true;
			}

			verbSuffixes = new[] { "ти", "тись" };

			if (verbSuffixes.Any(verbSuffix => stem.EndsWith(verbSuffix) && (!vowels.Contains(stem[stem.Length - verbSuffix.Length - 1]))))
			{
				return true;
			}

			return false;
		}

		public void Dispose()
		{
			hunspell.Dispose();
		}

		private static bool IsRussianLetter(char c)
		{
			return BelongsToRange(c, 'а', 'я') || BelongsToRange(c, 'А', 'Я');
		}

		private static bool BelongsToRange(char c, char left, char right)
		{
			return (uint)(c - left) <= right - left;
		}
	}
}