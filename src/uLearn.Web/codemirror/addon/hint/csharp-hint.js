(function (mod) {
	if (typeof exports == "object" && typeof module == "object") // CommonJS
		mod(require("../../lib/codemirror"));
	else if (typeof define == "function" && define.amd) // AMD
		define(["../../lib/codemirror"], mod);
	else // Plain browser env
		mod(CodeMirror);
})(function (CodeMirror) {
	"use strict";

	function forEach(arr, f) {
		for (var i = 0, e = arr.length; i < e; ++i) f(arr[i]);
	}

	function arrayContains(arr, item) {
		if (!Array.prototype.indexOf) {
			var i = arr.length;
			while (i--) {
				if (arr[i] === item) {
					return true;
				}
			}
			return false;
		}
		return arr.indexOf(item) != -1;
	}

	function csharpHint(editor) {
		var cur = editor.getCursor(), token = editor.getTokenAt(cur), tprop = token;
		var from = CodeMirror.Pos(cur.line, token.start);
		var completionList;
		if (token.string === ".") {
			from = CodeMirror.Pos(cur.line, token.start + 1);
			completionList = getCompletions("", true);
		}
		else if (!/^[\w@_]*$/.test(token.string)) {
			from = cur;
			completionList = getCompletions("", false);
		}
		else
			completionList = getCompletions(token.string, false);

		completionList = completionList.sort();

		return {
			list: completionList,
			from: from,
			to: CodeMirror.Pos(cur.line, token.end)
		};
	}
	var keywords = Object.keys(CodeMirror.resolveMode("text/x-csharp").keywords);
	var methods = (""
		+ " WriteLine Write"
		+ " Format Substring Length Parse TryParse ToString Split Join Round Min Max Ceiling Floor "
		+ " Where Select SelectMany ToArray ToList ToDictionary ToLookup Join Zip Aggregate GroupBy OrderBy OrderByDescending ThenBy ThenByDescending"
		+ " "
		).split(" ");
	var types = ("Console CultureInfo Enumerable Math Int Double "
		+ " "
		).split(" ");

	function getCompletions(start, afterDot) {
		var found = [];
		console.log("|" + start + "|");
		function maybeAdd(str) {
			if (str.lastIndexOf(start, 0) == 0 && !arrayContains(found, str)) found.push(str);
		}
		if (afterDot) {
			forEach(methods, maybeAdd);
		}
		else {
			forEach(types, maybeAdd);
			forEach(methods, maybeAdd);
			forEach(keywords, maybeAdd);
		}
		return found;
	}

	CodeMirror.registerHelper("hint", "csharp", csharpHint);
});
