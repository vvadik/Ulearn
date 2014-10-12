using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace uLearn
{
	public class TexReplacer
	{
		public string ReplacedText { get; private set; }

		public TexReplacer(string text)
		{
			id = Guid.NewGuid().ToString("N");
			backRegex = new Regex(id + @"\[\d+\]");
			ReplacedText = ReplaceTexInserts(text);
		}

		private static readonly Regex texRegex = new Regex(@"(^|\W)\$([^ $]+?)\$($|\W)");
		private readonly Regex backRegex; 
		private readonly string id;
		private readonly Dictionary<string, string> texInserts = new Dictionary<string, string>();
		private int counter;

		private string ReplaceTexInserts(string md)
		{
			return texRegex.Replace(md, MakeInsertId).Replace(" -- ", " — ");
		}
		
		public string PlaceTexInsertsBack(string textWithIds)
		{
			return backRegex.Replace(textWithIds, PrepareTexInsert);
		}

		private string PrepareTexInsert(Match match)
		{
			var insertId = match.Value;
			string tex;
			return texInserts.TryGetValue(insertId, out tex) ? FormatTex(tex) : insertId;
		}

		private string FormatTex(string tex)
		{
			return "<span class='tex'>" + tex + "</span>";
		}

		private string MakeInsertId(Match match)
		{
			var insertId = id + "[" + (counter++) + "]";
			texInserts.Add(insertId, match.Groups[2].Value);
			return match.Groups[1].Value + "" + insertId + match.Groups[3].Value;
		}
	}
}