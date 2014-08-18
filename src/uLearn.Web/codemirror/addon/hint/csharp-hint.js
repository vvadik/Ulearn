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
		var bracketsCount = 0;
		var totalLen = 0;
		var completionList;
		if (token.string === ".") {
			var lastToken = "";
			from = CodeMirror.Pos(cur.line, token.start + 1);
			cur.ch = cur.ch - 1;
			lastToken = editor.getTokenAt(cur);
			if (lastToken.string == ')') {
				bracketsCount++;
				totalLen = 0;
				while (bracketsCount != 0) {
					cur.ch = cur.ch - lastToken.string.length;
					totalLen += lastToken.string.length;
					lastToken = editor.getTokenAt(cur);
					if (lastToken.string == ')')
						bracketsCount++;
					if (lastToken.string == '(')
						bracketsCount--;
					if (cur.ch == 0)
						break;
				}
				cur.ch = cur.ch - 1;
				lastToken = editor.getTokenAt(cur);
				cur.ch = cur.ch + totalLen + 1;
			}
			cur.ch = cur.ch + 1;
			completionList = getCompletions(lastToken.string, "", true);
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
				bracketsCount++;
				totalLen = 0;
					while (bracketsCount != 0) {
						cur.ch = cur.ch - beforeDot.string.length;
						totalLen += beforeDot.string.length;
						beforeDot = editor.getTokenAt(cur);
						if (beforeDot.string == ')')
							bracketsCount++;
						if (beforeDot.string == '(')
							bracketsCount--;
						if (cur.ch == 0)
							break;
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

	var types = ['int', 'string', 'double', 'Console', 'Math', 'long', 'Boolean', 'double', 'Enumerable', 'Array', 'List'];

	var methods = (" WriteLine Write Format Substring Length Parse TryParse ToString Split Join Round Min Max Ceiling Floor"
		+ " Where Select SelectMany ToArray ToList ToDictionary ToLookup Join Zip Aggregate GroupBy OrderBy OrderByDescending ThenBy ThenByDescending ").split(" ");

	var dictWithStaticMethods = [];
	dictWithStaticMethods['int'] = ['Parse', 'TryParse'];
	dictWithStaticMethods['string'] = ['Join', 'Equals', 'IsNullOrEmpty', 'IsNullOrWhiteSpace', 'Compare', 'CompareOrdinal', 'Format', 'Copy', 'Concat', 'Intern', 'IsInterned'];
	dictWithStaticMethods['double'] = ['IsInfinity', 'IsPositiveInfinity', 'IsNegativeInfinity', 'IsNaN', 'Parse', 'TryParse'];
	dictWithStaticMethods['Console'] = ['Beep', 'Clear', 'ResetColor', 'MoveBufferArea', 'SetBufferSize', 'SetWindowSize', 'SetWindowPosition', 'SetCursorPosition', 'ReadKey', 'OpenStandardError', 'OpenStandardInput', 'OpenStandardOutput', 'SetIn', 'SetOut', 'SetError', 'Read', 'ReadLine', 'WriteLine', 'Write'];
	dictWithStaticMethods['Math'] = ['Acos', 'Asin', 'Atan', 'Atan2', 'Ceiling', 'Cos', 'Cosh', 'Floor', 'Sin', 'Tan', 'Sinh', 'Tanh', 'Round', 'Truncate', 'Sqrt', 'Log', 'Log10', 'Exp', 'Pow', 'IEEERemainder', 'Abs', 'Max', 'Min', 'Sign', 'BigMul', 'DivRem'];
	dictWithStaticMethods['long'] = ['Parse', 'TryParse'];
	dictWithStaticMethods['Boolean'] = ['Parse', 'TryParse'];
	dictWithStaticMethods['double'] = ['IsInfinity', 'IsPositiveInfinity', 'IsNegativeInfinity', 'IsNaN', 'Parse', 'TryParse'];
	dictWithStaticMethods['Enumerable'] = ['Sum', 'Min', 'Max', 'Average', 'Except', 'Reverse', 'SequenceEqual', 'AsEnumerable', 'ToArray', 'ToList', 'ToDictionary', 'ToLookup', 'DefaultIfEmpty', 'OfType', 'Cast', 'First', 'FirstOrDefault', 'Last', 'LastOrDefault', 'Single', 'SingleOrDefault', 'ElementAt', 'ElementAtOrDefault', 'Range', 'Repeat', 'Empty', 'Any', 'All', 'Count', 'LongCount', 'Contains', 'Aggregate', 'Where', 'Select', 'SelectMany', 'Take', 'TakeWhile', 'Skip', 'SkipWhile', 'Join', 'GroupJoin', 'OrderBy', 'OrderByDescending', 'ThenBy', 'ThenByDescending', 'GroupBy', 'Concat', 'Zip', 'Distinct', 'Union', 'Intersect'];
	dictWithStaticMethods['Array'] = ['AsReadOnly', 'Resize', 'CreateInstance', 'Copy', 'ConstrainedCopy', 'Clear', 'BinarySearch', 'ConvertAll', 'Exists', 'Find', 'FindAll', 'FindIndex', 'FindLast', 'FindLastIndex', 'ForEach', 'IndexOf', 'LastIndexOf', 'Reverse', 'Sort', 'TrueForAll'];

	var dictWithNonStaticMethods = [];
	dictWithNonStaticMethods['int'] = ['CompareTo', 'Equals', 'GetHashCode', 'ToString', 'GetTypeCode', 'GetType'];
	dictWithNonStaticMethods['string'] = ['Equals', 'CopyTo', 'ToCharArray', 'GetHashCode', 'Split', 'Substring', 'Trim', 'TrimStart', 'TrimEnd', 'IsNormalized', 'Normalize', 'CompareTo', 'Contains', 'EndsWith', 'IndexOf', 'IndexOfAny', 'LastIndexOf', 'LastIndexOfAny', 'PadLeft', 'PadRight', 'StartsWith', 'ToLower', 'ToLowerInvariant', 'ToUpper', 'ToUpperInvariant', 'ToString', 'Clone', 'Insert', 'Replace', 'Remove', 'GetTypeCode', 'GetEnumerator', 'GetType'];
	dictWithNonStaticMethods['double'] = ['CompareTo', 'Equals', 'GetHashCode', 'ToString', 'GetTypeCode', 'GetType'];
	dictWithNonStaticMethods['Console'] = ['ToString', 'Equals', 'GetHashCode', 'GetType'];
	dictWithNonStaticMethods['Math'] = ['ToString', 'Equals', 'GetHashCode', 'GetType'];
	dictWithNonStaticMethods['long'] = ['CompareTo', 'Equals', 'GetHashCode', 'ToString', 'GetTypeCode', 'GetType'];
	dictWithNonStaticMethods['Boolean'] = ['GetHashCode', 'ToString', 'Equals', 'CompareTo', 'GetTypeCode', 'GetType'];
	dictWithNonStaticMethods['double'] = ['CompareTo', 'Equals', 'GetHashCode', 'ToString', 'GetTypeCode', 'GetType'];
	dictWithNonStaticMethods['Enumerable'] = ['ToString', 'Equals', 'GetHashCode', 'GetType'];
	dictWithNonStaticMethods['Array'] = ['GetValue', 'SetValue', 'GetLength', 'GetLongLength', 'GetUpperBound', 'GetLowerBound', 'Clone', 'CopyTo', 'GetEnumerator', 'Initialize', 'ToString', 'Equals', 'GetHashCode', 'GetType'];
	dictWithNonStaticMethods['List'] = ['Add', 'AddRange', 'AsReadOnly', 'BinarySearch', 'Clear', 'Contains', 'ConvertAll', 'CopyTo', 'Find', 'FindAll', 'FindIndex', 'ForEach', 'GetEnumerator', 'GetRange', 'IndexOf', 'Insert', 'InsertRange', 'Remove', 'RemoveAll', 'RemoveAt', 'RemoveRange', 'Reverse', 'Sort', 'ToArray', 'TrimExcess', 'Exists', 'FindLast', 'FindLastIndex', 'LastIndexOf', 'TrueForAll', 'ToString', 'Equals', 'GetHashCode', 'GetType'];

	var dictWithProperties = [];
	dictWithProperties['string'] = ['Chars', 'Length'];
	dictWithProperties['Console'] = ['IsInputRedirected', 'IsOutputRedirected', 'IsErrorRedirected', 'In', 'Out', 'Error', 'InputEncoding', 'OutputEncoding', 'BackgroundColor', 'ForegroundColor', 'BufferHeight', 'BufferWidth', 'WindowHeight', 'WindowWidth', 'LargestWindowWidth', 'LargestWindowHeight', 'WindowLeft', 'WindowTop', 'CursorLeft', 'CursorTop', 'CursorSize', 'CursorVisible', 'Title', 'KeyAvailable', 'NumberLock', 'CapsLock', 'TreatControlCAsInput'];
	dictWithProperties['Array'] = ['Length', 'LongLength', 'Rank', 'SyncRoot', 'IsReadOnly', 'IsFixedSize', 'IsSynchronized'];
	dictWithProperties['List'] = ['Capacity', 'Count', 'Item'];

	var dictWithConstants = [];
	dictWithConstants['int'] = ['MaxValue', 'MinValue'];
	dictWithConstants['string'] = ['Empty'];
	dictWithConstants['double'] = ['MinValue', 'MaxValue', 'Epsilon', 'NegativeInfinity', 'PositiveInfinity', 'NaN'];
	dictWithConstants['Math'] = ['PI', 'E'];
	dictWithConstants['long'] = ['MaxValue', 'MinValue'];
	dictWithConstants['Boolean'] = ['TrueString', 'FalseString'];
	dictWithConstants['double'] = ['MinValue', 'Epsilon', 'MaxValue', 'PositiveInfinity', 'NegativeInfinity', 'NaN'];

	var returnTypeDict = [];
	returnTypeDict['int'] = ['Parse', 'Compare', 'CompareOrdinal', 'Read', 'Ceiling', 'Floor', 'Round', 'Truncate', 'Abs', 'Max', 'Min', 'Sign', 'DivRem', 'Sum', 'Average', 'Range', 'Count', 'BinarySearch', 'FindIndex', 'FindLastIndex', 'IndexOf', 'LastIndexOf', 'CompareTo', 'GetHashCode', 'IndexOfAny', 'LastIndexOfAny', 'GetLength', 'GetUpperBound', 'GetLowerBound', 'RemoveAll', 'Length', 'BufferHeight', 'BufferWidth', 'WindowHeight', 'WindowWidth', 'LargestWindowWidth', 'LargestWindowHeight', 'WindowLeft', 'WindowTop', 'CursorLeft', 'CursorTop', 'CursorSize', 'Rank', 'Capacity', 'MaxValue', 'MinValue'];
	returnTypeDict['Boolean'] = ['TryParse', 'Equals', 'IsNullOrEmpty', 'IsNullOrWhiteSpace', 'IsInfinity', 'IsPositiveInfinity', 'IsNegativeInfinity', 'IsNaN', 'Parse', 'SequenceEqual', 'Any', 'All', 'Contains', 'Exists', 'TrueForAll', 'IsNormalized', 'EndsWith', 'StartsWith', 'Remove', 'IsInputRedirected', 'IsOutputRedirected', 'IsErrorRedirected', 'CursorVisible', 'KeyAvailable', 'NumberLock', 'CapsLock', 'TreatControlCAsInput', 'IsReadOnly', 'IsFixedSize', 'IsSynchronized'];
	returnTypeDict['string'] = ['Join', 'Format', 'Copy', 'Concat', 'Intern', 'IsInterned', 'ReadLine', 'ToString', 'Split', 'Substring', 'Trim', 'TrimStart', 'TrimEnd', 'Normalize', 'PadLeft', 'PadRight', 'ToLower', 'ToLowerInvariant', 'ToUpper', 'ToUpperInvariant', 'Insert', 'Replace', 'Remove', 'Title', 'Empty', 'TrueString', 'FalseString'];
	returnTypeDict['double'] = ['Parse', 'Acos', 'Asin', 'Atan', 'Atan2', 'Ceiling', 'Cos', 'Cosh', 'Floor', 'Sin', 'Tan', 'Sinh', 'Tanh', 'Round', 'Truncate', 'Sqrt', 'Log', 'Log10', 'Exp', 'Pow', 'IEEERemainder', 'Abs', 'Max', 'Min', 'Sum', 'Average', 'MinValue', 'MaxValue', 'Epsilon', 'NegativeInfinity', 'PositiveInfinity', 'NaN', 'PI', 'E'];
	returnTypeDict['Void'] = ['Beep', 'Clear', 'ResetColor', 'MoveBufferArea', 'SetBufferSize', 'SetWindowSize', 'SetWindowPosition', 'SetCursorPosition', 'SetIn', 'SetOut', 'SetError', 'WriteLine', 'Write', 'Resize', 'Copy', 'ConstrainedCopy', 'ForEach', 'Reverse', 'Sort', 'CopyTo', 'SetValue', 'Initialize', 'Add', 'AddRange', 'Insert', 'InsertRange', 'RemoveAt', 'RemoveRange', 'TrimExcess'];
	returnTypeDict['ConsoleKeyInfo'] = ['ReadKey'];
	returnTypeDict['IO.Stream'] = ['OpenStandardError', 'OpenStandardInput', 'OpenStandardOutput'];
	returnTypeDict['SByte'] = ['Abs', 'Max', 'Min'];
	returnTypeDict['long'] = ['Abs', 'Max', 'Min', 'BigMul', 'DivRem', 'Parse', 'LongCount', 'Sum', 'GetLongLength', 'LongLength', 'MaxValue', 'MinValue'];
	returnTypeDict['Byte'] = ['Max', 'Min'];
	returnTypeDict['TSource'] = ['Min', 'Max', 'First', 'FirstOrDefault', 'Last', 'LastOrDefault', 'Single', 'SingleOrDefault', 'ElementAt', 'ElementAtOrDefault', 'Aggregate'];
	returnTypeDict['TResult'] = ['Min', 'Max', 'Aggregate'];
	returnTypeDict['Enumerable'] = ['Except', 'Reverse', 'AsEnumerable', 'DefaultIfEmpty', 'OfType', 'Cast', 'Repeat', 'Empty', 'Where', 'Select', 'SelectMany', 'Take', 'TakeWhile', 'Skip', 'SkipWhile', 'Join', 'GroupJoin', 'OrderBy', 'OrderByDescending', 'ThenBy', 'ThenByDescending', 'GroupBy', 'Concat', 'Zip', 'Distinct', 'Union', 'Intersect'];
	returnTypeDict['TSource[]'] = ['ToArray'];
	returnTypeDict['List'] = ['ToList', 'ConvertAll', 'FindAll', 'GetEnumerator', 'GetRange'];
	returnTypeDict['Collections.Generic.Dictionary`2[TKey,TSource]'] = ['ToDictionary'];
	returnTypeDict['Collections.Generic.Dictionary`2[TKey,TElement]'] = ['ToDictionary'];
	returnTypeDict['ILookup`2[TKey,TSource]'] = ['ToLookup'];
	returnTypeDict['ILookup`2[TKey,TElement]'] = ['ToLookup'];
	returnTypeDict['TAccumulate'] = ['Aggregate'];
	returnTypeDict['Collections.ObjectModel.ReadOnlyCollection`1[T]'] = ['AsReadOnly'];
	returnTypeDict['Array'] = ['CreateInstance'];
	returnTypeDict['TOutput[]'] = ['ConvertAll'];
	returnTypeDict['T'] = ['Find', 'FindLast', 'Item'];
	returnTypeDict['T[]'] = ['FindAll', 'ToArray'];
	returnTypeDict['TypeCode'] = ['GetTypeCode'];
	returnTypeDict['Type'] = ['GetType'];
	returnTypeDict['Char[]'] = ['ToCharArray'];
	returnTypeDict['Object'] = ['Clone', 'GetValue', 'SyncRoot'];
	returnTypeDict['CharEnumerator'] = ['GetEnumerator'];
	returnTypeDict['Collections.IEnumerator'] = ['GetEnumerator'];
	returnTypeDict['Char'] = ['Chars'];
	returnTypeDict['IO.TextReader'] = ['In'];
	returnTypeDict['IO.TextWriter'] = ['Out', 'Error'];
	returnTypeDict['Text.Encoding'] = ['InputEncoding', 'OutputEncoding'];
	returnTypeDict['ConsoleColor'] = ['BackgroundColor', 'ForegroundColor'];

	function getCompletions(beforeDot, start, afterDot) {
		var found = [];
		console.log("|" + start + "|");

		function maybeAdd(str) {
			if (str.toLowerCase().lastIndexOf(start.toLowerCase(), 0) == 0 && !arrayContains(found, str))
				found.push(str);
		}

		function autocompliteAfterDotWords() {
			isWasFound = false;
			if (arrayContains(types, beforeDot)) {
				if (dictWithStaticMethods[type] != undefined) {
					forEach(dictWithStaticMethods[type], maybeAdd);
					isWasFound = true;
				}
				if (dictWithProperties[type] != undefined) {
					forEach(dictWithProperties[type], maybeAdd);
					isWasFound = true;
				}
				if (dictWithConstants[type] != undefined) {
					forEach(dictWithConstants[type], maybeAdd);
					isWasFound = true;
				}
				if (!isWasFound) {
					forEach(methods, maybeAdd);
				}
			} else {
				for (type in returnTypeDict) {
					var isNeedBreak = false;
					for (element in returnTypeDict[type]) {
						if (beforeDot == returnTypeDict[type][element]) {
							if (dictWithNonStaticMethods[type] != undefined) {
								forEach(dictWithNonStaticMethods[type], maybeAdd);
								isWasFound = true;
							}
							if (dictWithStaticMethods[type] != undefined) {
								forEach(dictWithStaticMethods[type], maybeAdd);
								isWasFound = true;
							}
							if (dictWithProperties[type] != undefined) {
								forEach(dictWithProperties[type], maybeAdd);
								isWasFound = true;
							}
							if (dictWithConstants[type] != undefined) {
								forEach(dictWithConstants[type], maybeAdd);
								isWasFound = true;
							}
							isNeedBreak = true;
							break;
						}
					}
					if (isNeedBreak)
						break;
				}
				if (!isWasFound) {
					forEach(methods, maybeAdd);
				}
			}
		}

		var isWasFound = false;
		var type;
		var element;
		if (afterDot) {
			autocompliteAfterDotWords();
		} else {
			if (beforeDot != undefined) {
				autocompliteAfterDotWords();
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
