using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using NHunspell;
using Ulearn.Common.Extensions;
using Ulearn.Core.CSharp;
using Ulearn.Core.Properties;

namespace uLearn.CSharp.Validators.SpellingValidator
{
	public class CodeElementSpellChecker
	{
		private static readonly Hunspell hunspellEnUs = new Hunspell(Resources.en_US_aff, Resources.en_US_dic);
		private static readonly Hunspell hunspellEnGb = new Hunspell(Resources.en_GB_aff, Resources.en_GB_dic);
		private static readonly Hunspell hunspellLa = new Hunspell(Resources.la_aff, Resources.la_dic);
		private static readonly HashSet<string> wordsToExcept = new HashSet<string>(Resources.spelling_exceptions.SplitToLines());
		
		public IEnumerable<SolutionStyleError> CheckIdentifierNameForSpellingErrors(SyntaxToken identifier, string typeAsString = null)
		{
			var wordsInIdentifier = identifier.ValueText.SplitByCamelCase();
			foreach (var word in wordsInIdentifier)
			{
				var wordForCheck = RemoveIfySuffix(word);
				var wordInDifferentNumbers = GetWordInDifferentNumbers(wordForCheck);
				var doesWordContainError = true;
				foreach (var wordInDifferentNumber in wordInDifferentNumbers)
				{
					if (CheckForSpecialCases(wordInDifferentNumber, typeAsString)
						|| IsWordContainedInDictionaries(wordInDifferentNumber))
					{
						doesWordContainError = false;
						break;
					}
					var possibleErrorInWord = CheckConcatenatedWordsInLowerCaseForError(wordInDifferentNumber, identifier);
					if (possibleErrorInWord != null)
						continue;
					doesWordContainError = false;
					break;
				}
				if (doesWordContainError)
					yield return new SolutionStyleError(StyleErrorType.Misspeling01, identifier, $"В слове {word} допущена опечатка.");
			}
		}
		
		private static bool CheckForSpecialCases(string wordInDifferentNumber, string typeAsString = null)
		{
			return wordInDifferentNumber.Length <= 2
					|| (typeAsString != null && typeAsString.StartsWith(wordInDifferentNumber, StringComparison.InvariantCultureIgnoreCase))
					|| wordInDifferentNumber.Equals(typeAsString?.MakeTypeNameAbbreviation(), StringComparison.InvariantCultureIgnoreCase);
		}

		private string RemoveIfySuffix(string word)
		{
			return word.LastIndexOf("ify", StringComparison.InvariantCultureIgnoreCase) > 0
				? word.Substring(0, word.Length - 3)
				: word;
		}

		private IEnumerable<string> GetWordInDifferentNumbers(string word)
		{
			yield return word;
			if (word.Length > 1 && word.EndsWith("s", StringComparison.InvariantCultureIgnoreCase))
				yield return word.Substring(0, word.Length - 1);
			if (word.Length > 2 && word.EndsWith("es", StringComparison.InvariantCultureIgnoreCase))
				yield return word.Substring(0, word.Length - 2);
		}
		
		private SolutionStyleError CheckConcatenatedWordsInLowerCaseForError(string concatenatedWords, SyntaxToken tokenWithConcatenatedWords)
		{
			var currentCheckingWord = "";
			var foundWords = new HashSet<string>();
			while (currentCheckingWord.Length != concatenatedWords.Length)
			{
				currentCheckingWord = "";
				var isFirstWord = true;
				foreach (var symbol in concatenatedWords)
				{
					currentCheckingWord += symbol;
					if (!foundWords.Contains(currentCheckingWord)
						&& (currentCheckingWord.Length == 1 && isFirstWord
							||	currentCheckingWord.Length != 1
							&& IsWordContainedInDictionaries(currentCheckingWord)))
					{
						foundWords.Add(currentCheckingWord);
						if (isFirstWord
							&& IsWordContainedInDictionaries(
								concatenatedWords.Substring(currentCheckingWord.Length))) // это нужно для переменных типа dfunction b substring
							return null;
						currentCheckingWord = "";
						isFirstWord = false;
					}
				}

				if (currentCheckingWord == "")
					return null;
			}

			return new SolutionStyleError(StyleErrorType.Misspeling01, tokenWithConcatenatedWords, $"В слове {concatenatedWords} допущена опечатка.");
		}

		private bool IsWordContainedInDictionaries(string word) // есть проблема, при которой разные части одного слова проверяются в разных словарях (одна часть - в английском, вторая - в латинском)?
		{
			return wordsToExcept.Contains(word.ToLowerInvariant())
					|| hunspellEnUs.Spell(word)
					|| hunspellEnGb.Spell(word)
					|| hunspellLa.Spell(word);
		}
	}
}