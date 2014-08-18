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
		var tmpLen = 0;
		var totalLen = 0;
		var completionList;
		if (token.string === ".") {
			var lastToken = "";
			from = CodeMirror.Pos(cur.line, token.start + 1);
			cur.ch = cur.ch - 1;
			lastToken = editor.getTokenAt(cur);
			if (lastToken.string == ')') {
				totalLen = 0;
				while (lastToken.string != '(') {
					cur.ch = cur.ch - lastToken.string.length;
					totalLen += lastToken.string.length;
					lastToken = editor.getTokenAt(cur);
				}
				cur.ch = cur.ch - 1;
				lastToken = editor.getTokenAt(cur);
				cur.ch = cur.ch + totalLen + 1;
			}
			cur.ch = cur.ch + 1;
			completionList = getCompletions("", lastToken.string, true);
		}
		else if (!/^[\w@_]*$/.test(token.string)) {
			from = cur;
			completionList = getCompletions("", "", false);
		} else {
			var beforeDot = "";
			cur.ch = cur.ch - token.string.length;
			var lastDot = editor.getTokenAt(cur);
			if (lastDot.string == '.') {
				cur.ch = cur.ch - 1;
				beforeDot = editor.getTokenAt(cur);
				if (beforeDot.string == ')') {
					totalLen = 0;
					while (beforeDot.string != '(') {
						cur.ch = cur.ch - beforeDot.string.length;
						totalLen += beforeDot.string.length;
						beforeDot = editor.getTokenAt(cur);
					}
					cur.ch = cur.ch - 1;
					beforeDot = editor.getTokenAt(cur);
					cur.ch = cur.ch + totalLen + 1;
				}
				cur.ch = cur.ch + 1;
			}
			cur.ch = cur.ch + token.string.length;

			completionList = getCompletions(beforeDot.string, token.string, false);
		}

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
	var types = ("Console CultureInfo Enumerable Math int double string "
		+ " "
		).split(" ");

	var dictWithMethods = [];
	dictWithMethods['int'] = ['CompareTo', 'Equals', 'GetHashCode', 'ToString', 'Parse', 'TryParse', 'GetTypeCode', 'GetType'];
	dictWithMethods['string'] = ['Join', 'Equals', 'CopyTo', 'ToCharArray', 'IsNullOrEmpty', 'IsNullOrWhiteSpace', 'GetHashCode', 'Split', 'Substring', 'Trim', 'TrimStart', 'TrimEnd', 'IsNormalized', 'Normalize', 'Compare', 'CompareTo', 'CompareOrdinal', 'Contains', 'EndsWith', 'IndexOf', 'IndexOfAny', 'LastIndexOf', 'LastIndexOfAny', 'PadLeft', 'PadRight', 'StartsWith', 'ToLower', 'ToLowerInvariant', 'ToUpper', 'ToUpperInvariant', 'ToString', 'Clone', 'Insert', 'Replace', 'Remove', 'Format', 'Copy', 'Concat', 'Intern', 'IsInterned', 'GetTypeCode', 'GetEnumerator', 'GetType'];
	dictWithMethods['double'] = ['IsInfinity', 'IsPositiveInfinity', 'IsNegativeInfinity', 'IsNaN', 'CompareTo', 'Equals', 'GetHashCode', 'ToString', 'Parse', 'TryParse', 'GetTypeCode', 'GetType'];
	dictWithMethods['Console'] = ['Beep', 'Clear', 'ResetColor', 'MoveBufferArea', 'SetBufferSize', 'SetWindowSize', 'SetWindowPosition', 'SetCursorPosition', 'ReadKey', 'OpenStandardError', 'OpenStandardInput', 'OpenStandardOutput', 'SetIn', 'SetOut', 'SetError', 'Read', 'ReadLine', 'WriteLine', 'Write', 'ToString', 'Equals', 'GetHashCode', 'GetType'];
	dictWithMethods['Math'] = ['Acos', 'Asin', 'Atan', 'Atan2', 'Ceiling', 'Cos', 'Cosh', 'Floor', 'Sin', 'Tan', 'Sinh', 'Tanh', 'Round', 'Truncate', 'Sqrt', 'Log', 'Log10', 'Exp', 'Pow', 'IEEERemainder', 'Abs', 'Max', 'Min', 'Sign', 'BigMul', 'DivRem', 'ToString', 'Equals', 'GetHashCode', 'GetType'];

	var dictWithProperties = [];
	dictWithProperties['string'] = ['Chars', 'Length'];
	dictWithProperties['Console'] = ['IsInputRedirected', 'IsOutputRedirected', 'IsErrorRedirected', 'In', 'Out', 'Error', 'InputEncoding', 'OutputEncoding', 'BackgroundColor', 'ForegroundColor', 'BufferHeight', 'BufferWidth', 'WindowHeight', 'WindowWidth', 'LargestWindowWidth', 'LargestWindowHeight', 'WindowLeft', 'WindowTop', 'CursorLeft', 'CursorTop', 'CursorSize', 'CursorVisible', 'Title', 'KeyAvailable', 'NumberLock', 'CapsLock', 'TreatControlCAsInput'];

	var dictWithConstants = [];
	dictWithConstants['int'] = ['MaxValue', 'MinValue'];
	dictWithConstants['string'] = ['Empty'];
	dictWithConstants['double'] = ['MinValue', 'MaxValue', 'Epsilon', 'NegativeInfinity', 'PositiveInfinity', 'NaN'];
	dictWithConstants['Math'] = ['PI', 'E'];

	var returnTypeDict = [];
	returnTypeDict['int'] = ['CompareTo', 'GetHashCode', 'Parse', 'Compare', 'CompareOrdinal', 'IndexOf', 'IndexOfAny', 'LastIndexOf', 'LastIndexOfAny', 'Read', 'Abs', 'Max', 'Min', 'Sign', 'DivRem', 'Length', 'BufferHeight', 'BufferWidth', 'WindowHeight', 'WindowWidth', 'LargestWindowWidth', 'LargestWindowHeight', 'WindowLeft', 'WindowTop', 'CursorLeft', 'CursorTop', 'CursorSize', 'MaxValue', 'MinValue'];
	returnTypeDict['string'] = ['ToString', 'Join', 'Substring', 'Trim', 'TrimStart', 'TrimEnd', 'Normalize', 'PadLeft', 'PadRight', 'ToLower', 'ToLowerInvariant', 'ToUpper', 'ToUpperInvariant', 'Insert', 'Replace', 'Remove', 'Format', 'Copy', 'Concat', 'Intern', 'IsInterned', 'ReadLine', 'Title', 'Empty'];
	returnTypeDict['double'] = ['Parse', 'Acos', 'Asin', 'Atan', 'Atan2', 'Ceiling', 'Cos', 'Cosh', 'Floor', 'Sin', 'Tan', 'Sinh', 'Tanh', 'Round', 'Truncate', 'Sqrt', 'Log', 'Log10', 'Exp', 'Pow', 'IEEERemainder', 'Abs', 'Max', 'Min', 'MinValue', 'MaxValue', 'Epsilon', 'NegativeInfinity', 'PositiveInfinity', 'NaN', 'PI', 'E'];

	function getCompletions(beforeDot, start, afterDot) {
		var found = [];
		console.log("|" + start + "|");
		function maybeAdd(str) {
			if (str.lastIndexOf(start, 0) == 0 && !arrayContains(found, str)) found.push(str);
		}
		var isWasFound = false;
		if (afterDot) {
			var wordBeforeDot = start;
			start = "";
			if (dictWithConstants[wordBeforeDot] != undefined || dictWithMethods[wordBeforeDot] != undefined || dictWithProperties[wordBeforeDot] != undefined) {
				if (dictWithConstants[wordBeforeDot] != undefined)
					forEach(dictWithConstants[wordBeforeDot], maybeAdd);
				if (dictWithMethods[wordBeforeDot] != undefined)
					forEach(dictWithMethods[wordBeforeDot], maybeAdd);
				if (dictWithProperties[wordBeforeDot] != undefined)
					forEach(dictWithProperties[wordBeforeDot], maybeAdd);
			} else {
				isWasFound = false;
				for (var type in returnTypeDict) {
					for (var method in returnTypeDict[type]) {
						if (wordBeforeDot == returnTypeDict[type][method]) {
							if (dictWithMethods[type] != undefined)
								forEach(dictWithMethods[type], maybeAdd);
							if (dictWithProperties[type] != undefined)
								forEach(dictWithProperties[type], maybeAdd);
							if (dictWithConstants[type] != undefined)
								forEach(dictWithConstants[type], maybeAdd);
							isWasFound = true;
							break;
						}
					}
					if (isWasFound)
						break;
				}
				if (!isWasFound) {
					forEach(methods, maybeAdd);
				}
			}
		} else {
			if (beforeDot != undefined) {
				if (dictWithConstants[beforeDot] != undefined || dictWithMethods[beforeDot] != undefined || dictWithProperties[beforeDot] != undefined) {
					if (dictWithConstants[beforeDot] != undefined)
						forEach(dictWithConstants[beforeDot], maybeAdd);
					if (dictWithMethods[beforeDot] != undefined)
						forEach(dictWithMethods[beforeDot], maybeAdd);
					if (dictWithProperties[beforeDot] != undefined)
						forEach(dictWithProperties[beforeDot], maybeAdd);
				} else {
					isWasFound = false;
					for (var type2 in returnTypeDict) {
						for (var method2 in returnTypeDict[type2]) {
							if (beforeDot == returnTypeDict[type2][method2]) {
								if (dictWithMethods[type2] != undefined)
									forEach(dictWithMethods[type2], maybeAdd);
								if (dictWithProperties[type2] != undefined)
									forEach(dictWithProperties[type2], maybeAdd);
								if (dictWithConstants[type2] != undefined)
									forEach(dictWithConstants[type2], maybeAdd);
								isWasFound = true;
								break;
							}
						}
						if (isWasFound)
							break;
					}
					if (!isWasFound) {
						forEach(types, maybeAdd);
						forEach(methods, maybeAdd);
						forEach(keywords, maybeAdd);
					}
				}
			} else {
				forEach(types, maybeAdd);
				forEach(methods, maybeAdd);
				forEach(keywords, maybeAdd);
			}
		}
		return found;
	}

	CodeMirror.registerHelper("hint", "csharp", csharpHint);
});
