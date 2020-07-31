using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Serilog;
using Ulearn.Common;
using Ulearn.Common.Extensions;
using Ulearn.Core;

namespace AntiPlagiarism.Web.CodeAnalyzing
{
	public class Token
	{
		public string Type;
		public string Value;
		public int Position;

		public override string ToString()
		{
			return $"{Type} {Value}";
		}
	}

	public class TokensExtractor
	{
		private readonly ILogger logger;

		public TokensExtractor(ILogger logger)
		{
			this.logger = logger;
		}

		public static IEnumerable<Token> FilterWhitespaceTokens(IEnumerable<Token> tokens)
		{
			return tokens.Where(t => !string.IsNullOrWhiteSpace(t.Value));
		}

		public static IEnumerable<Token> FilterCommentTokens(IEnumerable<Token> tokens)
		{
			return tokens.Where(t => !t.Type.StartsWith("Comment"));
		}

		[CanBeNull]
		public List<Token> GetAllTokensFromPygmentize(string code, Language language)
		{
			var (codeWithNLineEndings, originalLineEndings) = PrepareLineEndingsForPygmentize(code);
			var pygmentizeResult = GetPygmentizeResult(codeWithNLineEndings, language);
			if (pygmentizeResult == null)
				return null;
			var tokensWithNLineEndings = ParseTokensFromPygmentize(code, pygmentizeResult).ToList();
			var tokens = ReturnOriginalLineEndings(tokensWithNLineEndings, originalLineEndings).ToList();
			SetPositions(tokens);
			return tokens;
		}

		private IEnumerable<Token> ParseTokensFromPygmentize(string code, string pygmentizeResult)
		{
			var lines = pygmentizeResult.TrimEnd().SplitToLines();
			for (var i = 0; i < lines.Length; i++)
			{
				var line = lines[i];
				var parts = line.Split('\t', 2);
				var tokenType = parts[0];
				tokenType = tokenType.Substring("Token.".Length);
				var tokenContentInQuotes = parts[1];
				var tokenContentEscaped = tokenContentInQuotes.Substring(1, parts[1].Length - 2);
				var tokenContent = Regex.Unescape(tokenContentEscaped);
				var token = new Token
				{
					Type = tokenType,
					Value = tokenContent
				};
				if (i == lines.Length - 1 && token.Value == "\n" && code.Last() != '\n')
					yield break; // pygmentize добавляет токен перевода строки в конце кода. Убираю его, если исходно не было
				yield return token;
			}
		}

		// Pygmentize заменяет в токенах конец строки на \n. Заменим сами на \n, а потом восстановим в токенах
		private static readonly Regex lineEndingsRegex = new Regex("\r\n|\n", RegexOptions.Compiled);
		private (string code, List<string> originalLineEndings) PrepareLineEndingsForPygmentize(string code)
		{
			var originalLineEndings = lineEndingsRegex.Matches(code).Select(m => m.Value).ToList();
			var resultCode = code.Replace("\r\n", "\n");
			return (resultCode, originalLineEndings);
		}

		private IEnumerable<Token> ReturnOriginalLineEndings(List<Token> tokens, List<string> originalLineEndings)
		{
			var lineNumber = 0;
			for (var i=0; i<tokens.Count; i++)
			{
				var token = tokens[i];
				if (token.Value == "\n")
				{
					token.Value = originalLineEndings[lineNumber];
					lineNumber++;
					yield return token;
					continue;
				}
				var parts = token.Value.Split('\n'); // Если \n в конце, последняя часть будет пустой строкой
				var sb = new StringBuilder();
				foreach (var part in parts.SkipLast(1))
				{
					sb.Append(part);
					sb.Append(originalLineEndings[lineNumber]);
					lineNumber++;
				}
				sb.Append(parts.Last());
				token.Value = sb.ToString();
				yield return token;
			}
		}

		public static void ThrowExceptionIfTokensNotMatchOriginalCode(string code, List<Token> tokens)
		{
			for (var i = 0; i < tokens.Count; i++)
			{
				var token = tokens[i];
				if (code.Substring(token.Position, token.Value.Length) != token.Value)
					throw new Exception();
			}
			var allTokensContentLength = tokens.Sum(t => t.Value.Length);
			if (code.Length != allTokensContentLength)
				throw new Exception();
		}

		private static void SetPositions(IEnumerable<Token> tokens)
		{
			var pos = 0;
			foreach (var token in tokens)
			{
				token.Position = pos;
				pos += token.Value.Length;
			}
		}

		private string GetPygmentizeResult(string code, Language language)
		{
			var lexer = LanguageToLexer(language);
			var arguments = lexer == null ? "-g" : $"-l {lexer}";
			arguments += " -f tokens";

			var sw = Stopwatch.StartNew();
			using (var process = BuildProcess(arguments))
			{
				process.Start();
				const int limit = 10 * 1024 * 1024;
				var readErrTask = new AsyncReader(process.StandardError, limit).GetDataAsync();
				var readOutTask = new AsyncReader(process.StandardOutput, limit).GetDataAsync();
				process.StandardInput.Write(code);
				process.StandardInput.Close();
				var isFinished = Task.WaitAll(new Task[] { readErrTask, readOutTask }, 1000);
				var ms = sw.ElapsedMilliseconds;

				if (!process.HasExited)
					Shutdown(process);

				if (readErrTask.Result.Length > 0)
					logger.Warning($"pygmentize написал на stderr: {readErrTask.Result}");

				if (!isFinished)
					logger.Warning($"Не хватило времени ({ms} ms) на работу pygmentize");
				else
					logger.Information($"pygmentize закончил работу за {ms} ms");

				if (process.ExitCode != 0)
					logger.Information($"pygmentize завершился с кодом {process.ExitCode}");

				return isFinished && readErrTask.Result.Length == 0 && readOutTask.Result.Length > 0
					? readOutTask.Result
					: null;
			}
		}

		private static void Shutdown(Process process)
		{
			try
			{
				process.Kill();
			}
			catch (Win32Exception)
			{
				/* Sometimes we can catch Access Denied error because the process is already terminating. It's ok, we don't need to rethrow exception */
			}
			catch (InvalidOperationException)
			{
				/* If process has already terminated */
			}

			var remainingTimeoutMs = 3000;
			while (!process.HasExited)
			{
				const int time = 10;
				Thread.Sleep(time);
				remainingTimeoutMs -= time;
				if (remainingTimeoutMs <= 0)
					throw new Exception($"process {process.Id} is not completed after kill");
			}
		}

		private static Process BuildProcess(string arguments)
		{
			return new Process
			{
				StartInfo =
				{
					Arguments = arguments,
					FileName = "pygmentize",
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					RedirectStandardInput =  true,
					CreateNoWindow = true,
					UseShellExecute = false
				}
			};
		}

		private string LanguageToLexer(Language language)
		{
			switch (language)
			{
				case Language.CSharp:
					return "csharp";
				case Language.Python2:
					return "py2";
				case Language.Python3:
					return "py3";
				case Language.Java:
					return "java";
				case Language.JavaScript:
					return "js";
				case Language.Html:
					return "html";
				case Language.TypeScript:
					return "ts";
				case Language.Css:
					return "css";
				case Language.Text:
					return "text";
				default:
					return null;
			}
		}
	}
}