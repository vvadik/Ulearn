using System;
using System.Collections.Generic;
using System.Web;
using OfficeOpenXml.FormulaParsing;

namespace uLearn.Web.Extensions
{
	public static class StringExtensions
	{
		public static string TruncateWithEllipsis(this string s, int maxLength, string ellipsis="...")
		{
			if (maxLength < 0)
				return s;

			if (ellipsis.Length > maxLength)
				throw new ArgumentOutOfRangeException("length", maxLength, "length must be at least as long as ellipsis.");

			if (s.Length > maxLength - ellipsis.Length)
				return s.Substring(0, maxLength - ellipsis.Length) + ellipsis;

			return s;
		}

		public static string TruncateHtmlWithEllipsis(this string s, int maxLength, string ellipsis = "...")
		{
			if (maxLength < 0)
				return s;

			if (ellipsis.Length > maxLength)
				throw new ArgumentOutOfRangeException("length", maxLength, "length must be at least as long as ellipsis.");

			var inTag = false;
			var realCharsCount = 0;
			var tags = new Stack<string>();
			var currentTag = "";
			var isClosingTag = false;
			for (var i = 0; i < s.Length; i++)
			{
				if (s[i] == '<' && !inTag)
				{
					currentTag = "";
					inTag = true;
					isClosingTag = false;
				}
				if (s[i] == '>' && inTag)
				{
					if (isClosingTag && tags.Count > 0)
						tags.Pop();
					if (! isClosingTag)
						tags.Push(currentTag);
					inTag = false;
				}
				if (inTag)
				{
					if (s[i] == '/' && s[i - 1] == '<')
						isClosingTag = true;
					else
						currentTag += s[i];
					continue;
				}

				realCharsCount++;
				if (realCharsCount == maxLength)
				{
					var closingTags = "";
					while (tags.Count > 0)
						closingTags += $"</{tags.Pop()}>";
					return s.Substring(0, i + 1) + closingTags + ellipsis;
				}
			}

			return s;
		}

		public static int IndexOfFirstLetter(this string s)
		{
			for (var i = 0; i < s.Length; i++)
				if (char.IsLetter(s[i]))
					return i;
			return -1;
		}

		public static char FindFirstLetter(this string s, char ifNotFound)
		{
			var index = s.IndexOfFirstLetter();
			if (index == -1)
				return ifNotFound;
			return s[index];
		}

		public static int FindLineStartIndex(this string s, int lineNumber)
		{
			var currentIndex = 0;
			for (var i = 0; i < lineNumber; i++)
			{
				currentIndex = s.IndexOf('\n', currentIndex) + 1;
				if (currentIndex == -1)
					return 0;
			}
			return currentIndex;
		}

		public static int FindPositionByLineAndCharacted(this string s, int lineNumber, int charNumber)
		{
			return s.FindLineStartIndex(lineNumber) + charNumber;
		}

		/* TODO (andgein): Move to ControllerBase? */
		public static bool IsLocalUrl(this string url, HttpRequestBase request)
		{
			if (string.IsNullOrEmpty(url))
				return false;

			if (Uri.TryCreate(url, UriKind.Absolute, out Uri absoluteUri))
				return request.Url != null && string.Equals(request.Url.Host, absoluteUri.Host, StringComparison.OrdinalIgnoreCase);

			var isLocal = !url.StartsWith("http:", StringComparison.OrdinalIgnoreCase)
						&& !url.StartsWith("https:", StringComparison.OrdinalIgnoreCase)
						&& Uri.IsWellFormedUriString(url, UriKind.Relative);
			return isLocal;
		}
	}
}