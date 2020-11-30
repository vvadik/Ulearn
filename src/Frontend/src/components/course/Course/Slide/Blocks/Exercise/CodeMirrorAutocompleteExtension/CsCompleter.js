function CsCompleter() {
	this.keywords = [
		'abstract', 'add', 'alias', 'as', 'ascending', 'async', 'await', 'base', 'bool', 'break', 'byte',
		'case', 'catch', 'char', 'checked', 'class', 'const', 'continue', 'decimal', 'default', 'delegate',
		'descending', 'do', 'double', 'dynamic', 'else', 'enum', 'event', 'explicit', 'extern', 'false',
		'finally', 'fixed', 'float', 'for', 'foreach', 'from', 'get', 'global', 'goto', 'group', 'if',
		'implicit', 'in', 'int', 'interface', 'internal', 'into', 'is', 'join', 'let', 'lock',
		'long', 'namespace', 'new', 'null', 'object', 'operator', 'orderby', 'out ', 'out', 'override',
		'params', 'partial ', 'private', 'protected', 'public', 'readonly', 'ref', 'remove', 'return',
		'sbyte', 'sealed', 'select', 'set', 'short', 'sizeof', 'stackalloc', 'static', 'string', 'struct',
		'switch', 'this', 'throw', 'true', 'try', 'typeof', 'uint', 'ulong', 'unchecked', 'unsafe', 'ushort',
		'using', 'value', 'var', 'virtual', 'void', 'volatile', 'where ', 'while', 'yield'
	];
	this.types = ['int', 'char', 'double', 'long', 'double', 'string', 'Console', 'Math', 'bool', 'Enumerable', 'Array', 'StringBuilder', 'DirectoryInfo', 'FileInfo', 'CultureInfo', 'Tuple', 'IComparer', 'IComparable', 'IEnumerable', 'Regex', 'Point', 'IEnumerable', 'IEnumerator'];

	this.synonym = {
		'ReadOnlyCollection': 'Enumerable',
		'Lookup': 'Enumerable',
		'Int': 'int',
		'Int16': 'int',
		'Int32': 'int',
		'Int64': 'long',
		'String': 'string',
		'Single': 'double',
		'Double': 'double',
		'Decimal': 'double',
		'Boolean': 'bool',
		'Char': 'char'
	};

	this.staticMembers = [];
	this.staticMembers['int'] = ['MaxValue', 'MinValue', 'Parse', 'TryParse'];
	this.staticMembers['char'] = ['ConvertFromUtf32', 'ConvertToUtf32', 'GetNumericValue', 'GetUnicodeCategory', 'IsControl', 'IsDigit', 'IsHighSurrogate', 'IsLetter', 'IsLetterOrDigit', 'IsLower', 'IsLowSurrogate', 'IsNumber', 'IsPunctuation', 'IsSeparator', 'IsSurrogate', 'IsSurrogatePair', 'IsSymbol', 'IsUpper', 'IsWhiteSpace', 'MaxValue', 'MinValue', 'Parse', 'ToLower', 'ToLowerInvariant', 'ToString', 'ToUpper', 'ToUpperInvariant', 'TryParse'];
	this.staticMembers['double'] = ['Epsilon', 'IsInfinity', 'IsNaN', 'IsNegativeInfinity', 'IsPositiveInfinity', 'MaxValue', 'MinValue', 'NaN', 'NegativeInfinity', 'Parse', 'PositiveInfinity', 'TryParse'];
	this.staticMembers['long'] = ['MaxValue', 'MinValue', 'Parse', 'TryParse'];
	this.staticMembers['double'] = ['Epsilon', 'IsInfinity', 'IsNaN', 'IsNegativeInfinity', 'IsPositiveInfinity', 'MaxValue', 'MinValue', 'NaN', 'NegativeInfinity', 'Parse', 'PositiveInfinity', 'TryParse'];
	this.staticMembers['string'] = ['Compare', 'CompareOrdinal', 'Concat', 'Copy', 'Empty', 'Equals', 'Format', 'Intern', 'IsInterned', 'IsNullOrEmpty', 'IsNullOrWhiteSpace', 'Join'];
	this.staticMembers['Console'] = ['BackgroundColor', 'Beep', 'BufferHeight', 'BufferWidth', 'CapsLock', 'Clear', 'CursorLeft', 'CursorSize', 'CursorTop', 'CursorVisible', 'Error', 'ForegroundColor', 'In', 'InputEncoding', 'IsErrorRedirected', 'IsInputRedirected', 'IsOutputRedirected', 'KeyAvailable', 'LargestWindowHeight', 'LargestWindowWidth', 'MoveBufferArea', 'NumberLock', 'OpenStandardError', 'OpenStandardInput', 'OpenStandardOutput', 'Out', 'OutputEncoding', 'Read', 'ReadKey', 'ReadLine', 'ResetColor', 'SetBufferSize', 'SetCursorPosition', 'SetError', 'SetIn', 'SetOut', 'SetWindowPosition', 'SetWindowSize', 'Title', 'TreatControlCAsInput', 'WindowHeight', 'WindowLeft', 'WindowTop', 'WindowWidth', 'Write', 'WriteLine'];
	this.staticMembers['Math'] = ['Abs', 'Acos', 'Asin', 'Atan', 'Atan2', 'BigMul', 'Ceiling', 'Cos', 'Cosh', 'DivRem', 'E', 'Exp', 'Floor', 'IEEERemainder', 'Log', 'Log10', 'Max', 'Min', 'PI', 'Pow', 'Round', 'Sign', 'Sin', 'Sinh', 'Sqrt', 'Tan', 'Tanh', 'Truncate'];
	this.staticMembers['bool'] = ['FalseString', 'Parse', 'TrueString', 'TryParse'];
	this.staticMembers['Enumerable'] = ['Aggregate', 'All', 'Any', 'AsEnumerable', 'Average', 'Cast', 'Concat', 'Contains', 'Count', 'DefaultIfEmpty', 'Distinct', 'ElementAt', 'ElementAtOrDefault', 'Empty', 'Except', 'First', 'FirstOrDefault', 'GroupBy', 'GroupJoin', 'Intersect', 'Join', 'Last', 'LastOrDefault', 'LongCount', 'Max', 'Min', 'OfType', 'OrderBy', 'OrderByDescending', 'Range', 'Repeat', 'Reverse', 'Select', 'SelectMany', 'SequenceEqual', 'Single', 'SingleOrDefault', 'Skip', 'SkipWhile', 'Sum', 'Take', 'TakeWhile', 'ThenBy', 'ThenByDescending', 'ToArray', 'ToDictionary', 'ToList', 'ToLookup', 'Union', 'Where', 'Zip'];
	this.staticMembers['Array'] = ['AsReadOnly', 'BinarySearch', 'Clear', 'ConstrainedCopy', 'ConvertAll', 'Copy', 'CreateInstance', 'Exists', 'Find', 'FindAll', 'FindIndex', 'FindLast', 'FindLastIndex', 'ForEach', 'IndexOf', 'LastIndexOf', 'Resize', 'Reverse', 'Sort', 'TrueForAll'];
	this.staticMembers['StringBuilder'] = [];
	this.staticMembers['Dictionary`2'] = [];
	this.staticMembers['List`1'] = [];
	this.staticMembers['DirectoryInfo'] = [];
	this.staticMembers['FileInfo'] = [];
	this.staticMembers['CultureInfo'] = ['CreateSpecificCulture', 'CurrentCulture', 'CurrentUICulture', 'DefaultThreadCurrentCulture', 'DefaultThreadCurrentUICulture', 'GetCultureInfo', 'GetCultureInfoByIetfLanguageTag', 'GetCultures', 'InstalledUICulture', 'InvariantCulture', 'ReadOnly'];
	this.staticMembers['Tuple'] = ['Create'];
	this.staticMembers['Tuple`2'] = [];
	this.staticMembers['Tuple`3'] = [];
	this.staticMembers['Tuple`4'] = [];
	this.staticMembers['IComparer'] = [];
	this.staticMembers['IComparable'] = [];
	this.staticMembers['IEnumerable'] = [];
	this.staticMembers['ILookup`2'] = [];
	this.staticMembers['Regex'] = ['CacheSize', 'CompileToAssembly', 'Escape', 'InfiniteMatchTimeout', 'IsMatch', 'Match', 'Matches', 'Replace', 'Split', 'Unescape'];
	this.staticMembers['KeyValuePair`2'] = [];
	this.staticMembers['Point'] = ['Add', 'Ceiling', 'Empty', 'Round', 'Subtract', 'Truncate'];
	this.staticMembers['IEnumerable'] = [];
	this.staticMembers['IEnumerator'] = [];

	this.dynamicMembers = [];
	this.dynamicMembers['int'] = ['CompareTo', 'Equals', 'GetHashCode', 'GetType', 'GetTypeCode', 'ToString'];
	this.dynamicMembers['char'] = ['CompareTo', 'Equals', 'GetHashCode', 'GetType', 'GetTypeCode', 'ToString'];
	this.dynamicMembers['double'] = ['CompareTo', 'Equals', 'GetHashCode', 'GetType', 'GetTypeCode', 'ToString'];
	this.dynamicMembers['long'] = ['CompareTo', 'Equals', 'GetHashCode', 'GetType', 'GetTypeCode', 'ToString'];
	this.dynamicMembers['double'] = ['CompareTo', 'Equals', 'GetHashCode', 'GetType', 'GetTypeCode', 'ToString'];
	this.dynamicMembers['string'] = ['Aggregate', 'All', 'Any', 'AsEnumerable', 'AsParallel', 'AsQueryable', 'Average', 'Cast', 'Chars', 'Clone', 'CompareTo', 'Concat', 'Contains', 'CopyTo', 'Count', 'DefaultIfEmpty', 'Distinct', 'ElementAt', 'ElementAtOrDefault', 'EndsWith', 'Equals', 'Except', 'First', 'FirstOrDefault', 'GetEnumerator', 'GetHashCode', 'GetType', 'GetTypeCode', 'GroupBy', 'GroupJoin', 'IndexOf', 'IndexOfAny', 'Insert', 'Intersect', 'IsNormalized', 'Join', 'Last', 'LastIndexOf', 'LastIndexOfAny', 'LastOrDefault', 'Length', 'LongCount', 'Max', 'Min', 'Normalize', 'OfType', 'OrderBy', 'OrderByDescending', 'PadLeft', 'PadRight', 'Remove', 'Replace', 'Reverse', 'Select', 'SelectMany', 'SequenceEqual', 'Single', 'SingleOrDefault', 'Skip', 'SkipWhile', 'Split', 'StartsWith', 'Substring', 'Sum', 'Take', 'TakeWhile', 'ToArray', 'ToCharArray', 'ToDictionary', 'ToList', 'ToLookup', 'ToLower', 'ToLowerInvariant', 'ToString', 'ToUpper', 'ToUpperInvariant', 'Trim', 'TrimEnd', 'TrimStart', 'Union', 'Where', 'Zip'];
	this.dynamicMembers['Console'] = ['Equals', 'GetHashCode', 'GetType', 'ToString'];
	this.dynamicMembers['Math'] = ['Equals', 'GetHashCode', 'GetType', 'ToString'];
	this.dynamicMembers['bool'] = ['CompareTo', 'Equals', 'GetHashCode', 'GetType', 'GetTypeCode', 'ToString'];
	this.dynamicMembers['Enumerable'] = ['Equals', 'GetHashCode', 'GetType', 'ToString'];
	this.dynamicMembers['Array'] = ['AsParallel', 'AsQueryable', 'Cast', 'Clone', 'CopyTo', 'Equals', 'GetEnumerator', 'GetHashCode', 'GetLength', 'GetLongLength', 'GetLowerBound', 'GetType', 'GetUpperBound', 'GetValue', 'Initialize', 'IsFixedSize', 'IsReadOnly', 'IsSynchronized', 'Length', 'LongLength', 'OfType', 'Rank', 'SetValue', 'SyncRoot', 'ToString'];
	this.dynamicMembers['StringBuilder'] = ['Append', 'AppendFormat', 'AppendLine', 'Capacity', 'Chars', 'Clear', 'CopyTo', 'EnsureCapacity', 'Equals', 'GetHashCode', 'GetType', 'Insert', 'Length', 'MaxCapacity', 'Remove', 'Replace', 'ToString'];
	this.dynamicMembers['Dictionary`2'] = ['Add', 'Aggregate', 'All', 'Any', 'AsEnumerable', 'AsParallel', 'AsQueryable', 'Average', 'Cast', 'Clear', 'Comparer', 'Concat', 'Contains', 'ContainsKey', 'ContainsValue', 'Count', 'DefaultIfEmpty', 'Distinct', 'ElementAt', 'ElementAtOrDefault', 'Equals', 'Except', 'First', 'FirstOrDefault', 'GetEnumerator', 'GetHashCode', 'GetObjectData', 'GetType', 'GroupBy', 'GroupJoin', 'Intersect', 'Item', 'Join', 'Keys', 'Last', 'LastOrDefault', 'LongCount', 'Max', 'Min', 'OfType', 'OnDeserialization', 'OrderBy', 'OrderByDescending', 'Remove', 'Reverse', 'Select', 'SelectMany', 'SequenceEqual', 'Single', 'SingleOrDefault', 'Skip', 'SkipWhile', 'Sum', 'Take', 'TakeWhile', 'ToArray', 'ToDictionary', 'ToList', 'ToLookup', 'ToString', 'TryGetValue', 'Union', 'Values', 'Where', 'Zip'];
	this.dynamicMembers['List`1'] = ['Add', 'AddRange', 'Aggregate', 'All', 'Any', 'AsEnumerable', 'AsParallel', 'AsQueryable', 'AsReadOnly', 'Average', 'BinarySearch', 'Capacity', 'Cast', 'Clear', 'Concat', 'Contains', 'ConvertAll', 'CopyTo', 'Count', 'DefaultIfEmpty', 'Distinct', 'ElementAt', 'ElementAtOrDefault', 'Equals', 'Except', 'Exists', 'Find', 'FindAll', 'FindIndex', 'FindLast', 'FindLastIndex', 'First', 'FirstOrDefault', 'ForEach', 'GetEnumerator', 'GetHashCode', 'GetRange', 'GetType', 'GroupBy', 'GroupJoin', 'IndexOf', 'Insert', 'InsertRange', 'Intersect', 'Item', 'Join', 'Last', 'LastIndexOf', 'LastOrDefault', 'LongCount', 'Max', 'Min', 'OfType', 'OrderBy', 'OrderByDescending', 'Remove', 'RemoveAll', 'RemoveAt', 'RemoveRange', 'Reverse', 'Select', 'SelectMany', 'SequenceEqual', 'Single', 'SingleOrDefault', 'Skip', 'SkipWhile', 'Sort', 'Sum', 'Take', 'TakeWhile', 'ToArray', 'ToDictionary', 'ToList', 'ToLookup', 'ToString', 'TrimExcess', 'TrueForAll', 'Union', 'Where', 'Zip'];
	this.dynamicMembers['DirectoryInfo'] = ['Attributes', 'Create', 'CreateObjRef', 'CreateSubdirectory', 'CreationTime', 'CreationTimeUtc', 'Delete', 'EnumerateDirectories', 'EnumerateFiles', 'EnumerateFileSystemInfos', 'Equals', 'Exists', 'Extension', 'FullName', 'GetAccessControl', 'GetDirectories', 'GetFiles', 'GetFileSystemInfos', 'GetHashCode', 'GetLifetimeService', 'GetObjectData', 'GetType', 'InitializeLifetimeService', 'LastAccessTime', 'LastAccessTimeUtc', 'LastWriteTime', 'LastWriteTimeUtc', 'MoveTo', 'Name', 'Parent', 'Refresh', 'Root', 'SetAccessControl', 'ToString'];
	this.dynamicMembers['FileInfo'] = ['AppendText', 'Attributes', 'CopyTo', 'Create', 'CreateObjRef', 'CreateText', 'CreationTime', 'CreationTimeUtc', 'Decrypt', 'Delete', 'Directory', 'DirectoryName', 'Encrypt', 'Equals', 'Exists', 'Extension', 'FullName', 'GetAccessControl', 'GetHashCode', 'GetLifetimeService', 'GetObjectData', 'GetType', 'InitializeLifetimeService', 'IsReadOnly', 'LastAccessTime', 'LastAccessTimeUtc', 'LastWriteTime', 'LastWriteTimeUtc', 'Length', 'MoveTo', 'Name', 'Open', 'OpenRead', 'OpenText', 'OpenWrite', 'Refresh', 'Replace', 'SetAccessControl', 'ToString'];
	this.dynamicMembers['CultureInfo'] = ['Calendar', 'ClearCachedData', 'Clone', 'CompareInfo', 'CultureTypes', 'DateTimeFormat', 'DisplayName', 'EnglishName', 'Equals', 'GetConsoleFallbackUICulture', 'GetFormat', 'GetHashCode', 'GetType', 'IetfLanguageTag', 'IsNeutralCulture', 'IsReadOnly', 'KeyboardLayoutId', 'LCID', 'Name', 'NativeName', 'NumberFormat', 'OptionalCalendars', 'Parent', 'TextInfo', 'ThreeLetterISOLanguageName', 'ThreeLetterWindowsLanguageName', 'ToString', 'TwoLetterISOLanguageName', 'UseUserOverride'];
	this.dynamicMembers['Tuple'] = ['Equals', 'GetHashCode', 'GetType', 'ToString'];
	this.dynamicMembers['Tuple`2'] = ['Equals', 'GetHashCode', 'GetType', 'Item1', 'Item2', 'ToString'];
	this.dynamicMembers['Tuple`3'] = ['Equals', 'GetHashCode', 'GetType', 'Item1', 'Item2', 'Item3', 'ToString'];
	this.dynamicMembers['Tuple`4'] = ['Equals', 'GetHashCode', 'GetType', 'Item1', 'Item2', 'Item3', 'Item4', 'ToString'];
	this.dynamicMembers['IComparer'] = ['Compare'];
	this.dynamicMembers['IComparable'] = ['CompareTo'];
	this.dynamicMembers['IEnumerable'] = ['GetEnumerator'];
	this.dynamicMembers['ILookup`2'] = ['Aggregate', 'All', 'Any', 'AsEnumerable', 'AsParallel', 'AsQueryable', 'Average', 'Cast', 'Concat', 'Contains', 'Count', 'DefaultIfEmpty', 'Distinct', 'ElementAt', 'ElementAtOrDefault', 'Except', 'First', 'FirstOrDefault', 'GroupBy', 'GroupJoin', 'Intersect', 'Item', 'Join', 'Last', 'LastOrDefault', 'LongCount', 'Max', 'Min', 'OfType', 'OrderBy', 'OrderByDescending', 'Reverse', 'Select', 'SelectMany', 'SequenceEqual', 'Single', 'SingleOrDefault', 'Skip', 'SkipWhile', 'Sum', 'Take', 'TakeWhile', 'ToArray', 'ToDictionary', 'ToList', 'ToLookup', 'Union', 'Where', 'Zip'];
	this.dynamicMembers['Regex'] = ['Equals', 'GetGroupNames', 'GetGroupNumbers', 'GetHashCode', 'GetType', 'GroupNameFromNumber', 'GroupNumberFromName', 'IsMatch', 'Match', 'Matches', 'MatchTimeout', 'Options', 'Replace', 'RightToLeft', 'Split', 'ToString'];
	this.dynamicMembers['KeyValuePair`2'] = ['Equals', 'GetHashCode', 'GetType', 'Key', 'ToString', 'Value'];
	this.dynamicMembers['Point'] = ['Equals', 'GetHashCode', 'GetType', 'IsEmpty', 'Offset', 'ToString', 'X', 'Y'];
	this.dynamicMembers['IEnumerable'] = ['GetEnumerator'];
	this.dynamicMembers['IEnumerator'] = ['Current', 'MoveNext', 'Reset'];

	this.membersByReturnType = [];
	this.membersByReturnType['int'] = ['Parse', 'MaxValue', 'MinValue', 'ConvertToUtf32', 'Compare', 'CompareOrdinal', 'Read', 'BufferHeight', 'BufferWidth', 'WindowHeight', 'WindowWidth', 'LargestWindowWidth', 'LargestWindowHeight', 'WindowLeft', 'WindowTop', 'CursorLeft', 'CursorTop', 'CursorSize', 'Abs', 'Max', 'Min', 'Sign', 'DivRem', 'Count', 'Sum', 'BinarySearch', 'FindIndex', 'FindLastIndex', 'IndexOf', 'LastIndexOf', 'CacheSize', 'CompareTo', 'GetHashCode', 'IndexOfAny', 'LastIndexOfAny', 'Length', 'GetLength', 'GetUpperBound', 'GetLowerBound', 'Rank', 'EnsureCapacity', 'Capacity', 'MaxCapacity', 'RemoveAll', 'LCID', 'KeyboardLayoutId', 'GroupNumberFromName', 'X', 'Y'];
	this.membersByReturnType['bool'] = ['TryParse', 'IsDigit', 'IsLetter', 'IsWhiteSpace', 'IsUpper', 'IsLower', 'IsPunctuation', 'IsLetterOrDigit', 'IsControl', 'IsNumber', 'IsSeparator', 'IsSurrogate', 'IsSymbol', 'IsHighSurrogate', 'IsLowSurrogate', 'IsSurrogatePair', 'IsInfinity', 'IsPositiveInfinity', 'IsNegativeInfinity', 'IsNaN', 'Equals', 'IsNullOrEmpty', 'IsNullOrWhiteSpace', 'IsInputRedirected', 'IsOutputRedirected', 'IsErrorRedirected', 'CursorVisible', 'KeyAvailable', 'NumberLock', 'CapsLock', 'TreatControlCAsInput', 'Parse', 'SequenceEqual', 'Any', 'All', 'Contains', 'Exists', 'TrueForAll', 'IsMatch', 'IsNormalized', 'EndsWith', 'StartsWith', 'IsReadOnly', 'IsFixedSize', 'IsSynchronized', 'ContainsKey', 'Remove', 'TryGetValue', 'ContainsValue', 'IsNeutralCulture', 'UseUserOverride', 'RightToLeft', 'IsEmpty', 'MoveNext'];
	this.membersByReturnType['string'] = ['ToString', 'ConvertFromUtf32', 'Join', 'Format', 'Copy', 'Concat', 'Intern', 'IsInterned', 'Empty', 'ReadLine', 'Title', 'TrueString', 'FalseString', 'Escape', 'Unescape', 'Replace', 'Substring', 'Trim', 'TrimStart', 'TrimEnd', 'Normalize', 'PadLeft', 'PadRight', 'ToLower', 'ToLowerInvariant', 'ToUpper', 'ToUpperInvariant', 'Insert', 'Remove', 'Name', 'FullName', 'Extension', 'DirectoryName', 'IetfLanguageTag', 'DisplayName', 'NativeName', 'EnglishName', 'TwoLetterISOLanguageName', 'ThreeLetterISOLanguageName', 'ThreeLetterWindowsLanguageName', 'GroupNameFromNumber'];
	this.membersByReturnType['char'] = ['Parse', 'ToUpper', 'ToUpperInvariant', 'ToLower', 'ToLowerInvariant', 'MaxValue', 'MinValue', 'Chars'];
	this.membersByReturnType['UnicodeCategory'] = ['GetUnicodeCategory'];
	this.membersByReturnType['double'] = ['GetNumericValue', 'Parse', 'MinValue', 'MaxValue', 'Epsilon', 'NegativeInfinity', 'PositiveInfinity', 'NaN', 'Acos', 'Asin', 'Atan', 'Atan2', 'Ceiling', 'Cos', 'Cosh', 'Floor', 'Sin', 'Tan', 'Sinh', 'Tanh', 'Round', 'Truncate', 'Sqrt', 'Log', 'Log10', 'Exp', 'Pow', 'IEEERemainder', 'Abs', 'Max', 'Min', 'PI', 'E', 'Sum', 'Average'];
	this.membersByReturnType['long'] = ['Parse', 'MaxValue', 'MinValue', 'Abs', 'Max', 'Min', 'BigMul', 'DivRem', 'LongCount', 'Sum', 'GetLongLength', 'LongLength', 'Length'];
	this.membersByReturnType['Void'] = ['Beep', 'Clear', 'ResetColor', 'MoveBufferArea', 'SetBufferSize', 'SetWindowSize', 'SetWindowPosition', 'SetCursorPosition', 'SetIn', 'SetOut', 'SetError', 'WriteLine', 'Write', 'Resize', 'Copy', 'ConstrainedCopy', 'ForEach', 'Reverse', 'Sort', 'CompileToAssembly', 'CopyTo', 'SetValue', 'Initialize', 'Add', 'GetObjectData', 'OnDeserialization', 'AddRange', 'Insert', 'InsertRange', 'RemoveAt', 'RemoveRange', 'TrimExcess', 'Create', 'SetAccessControl', 'MoveTo', 'Delete', 'Refresh', 'Decrypt', 'Encrypt', 'ClearCachedData', 'Offset', 'Reset'];
	this.membersByReturnType['ConsoleKeyInfo'] = ['ReadKey'];
	this.membersByReturnType['Stream'] = ['OpenStandardError', 'OpenStandardInput', 'OpenStandardOutput'];
	this.membersByReturnType['TextReader'] = ['In'];
	this.membersByReturnType['TextWriter'] = ['Out', 'Error'];
	this.membersByReturnType['Encoding'] = ['InputEncoding', 'OutputEncoding'];
	this.membersByReturnType['ConsoleColor'] = ['BackgroundColor', 'ForegroundColor'];
	this.membersByReturnType['SByte'] = ['Abs', 'Max', 'Min'];
	this.membersByReturnType['Byte'] = ['Max', 'Min'];
	this.membersByReturnType['UInt16'] = ['Max', 'Min'];
	this.membersByReturnType['UInt32'] = ['Max', 'Min'];
	this.membersByReturnType['UInt64'] = ['Max', 'Min'];
	this.membersByReturnType['Nullable`1'] = ['Sum', 'Min', 'Max', 'Average'];
	this.membersByReturnType['TSource'] = ['Min', 'Max', 'First', 'FirstOrDefault', 'Last', 'LastOrDefault', 'Single', 'SingleOrDefault', 'ElementAt', 'ElementAtOrDefault', 'Aggregate'];
	this.membersByReturnType['TResult'] = ['Min', 'Max', 'Aggregate'];
	this.membersByReturnType['IEnumerable`1'] = ['Except', 'Reverse', 'AsEnumerable', 'DefaultIfEmpty', 'OfType', 'Cast', 'Range', 'Repeat', 'Empty', 'Where', 'Select', 'SelectMany', 'Take', 'TakeWhile', 'Skip', 'SkipWhile', 'Join', 'GroupJoin', 'GroupBy', 'Concat', 'Zip', 'Distinct', 'Union', 'Intersect', 'EnumerateDirectories', 'EnumerateFiles', 'EnumerateFileSystemInfos', 'Item'];
	this.membersByReturnType['TSource[]'] = ['ToArray'];
	this.membersByReturnType['List`1'] = ['ToList', 'ConvertAll', 'FindAll', 'GetRange'];
	this.membersByReturnType['Dictionary`2'] = ['ToDictionary'];
	this.membersByReturnType['ILookup`2'] = ['ToLookup'];
	this.membersByReturnType['TAccumulate'] = ['Aggregate'];
	this.membersByReturnType['IOrderedEnumerable`1'] = ['OrderBy', 'OrderByDescending', 'ThenBy', 'ThenByDescending'];
	this.membersByReturnType['ReadOnlyCollection`1'] = ['AsReadOnly'];
	this.membersByReturnType['Array'] = ['CreateInstance'];
	this.membersByReturnType['TOutput[]'] = ['ConvertAll'];
	this.membersByReturnType['T'] = ['Find', 'FindLast', 'Item'];
	this.membersByReturnType['T[]'] = ['FindAll', 'ToArray'];
	this.membersByReturnType['CultureInfo'] = ['CreateSpecificCulture', 'ReadOnly', 'GetCultureInfo', 'GetCultureInfoByIetfLanguageTag', 'CurrentCulture', 'CurrentUICulture', 'InstalledUICulture', 'DefaultThreadCurrentCulture', 'DefaultThreadCurrentUICulture', 'InvariantCulture', 'GetConsoleFallbackUICulture', 'Parent'];
	this.membersByReturnType['CultureInfo[]'] = ['GetCultures'];
	this.membersByReturnType['Tuple`1'] = ['Create'];
	this.membersByReturnType['Tuple`2'] = ['Create'];
	this.membersByReturnType['Tuple`3'] = ['Create'];
	this.membersByReturnType['Tuple`4'] = ['Create'];
	this.membersByReturnType['Tuple`5'] = ['Create'];
	this.membersByReturnType['Tuple`6'] = ['Create'];
	this.membersByReturnType['Tuple`7'] = ['Create'];
	this.membersByReturnType['Tuple`8'] = ['Create'];
	this.membersByReturnType['Match'] = ['Match'];
	this.membersByReturnType['MatchCollection'] = ['Matches'];
	this.membersByReturnType['String[]'] = ['Split', 'GetGroupNames'];
	this.membersByReturnType['TimeSpan'] = ['InfiniteMatchTimeout', 'MatchTimeout'];
	this.membersByReturnType['Point'] = ['Add', 'Subtract', 'Ceiling', 'Truncate', 'Round', 'Empty'];
	this.membersByReturnType['TypeCode'] = ['GetTypeCode'];
	this.membersByReturnType['Type'] = ['GetType'];
	this.membersByReturnType['Char[]'] = ['ToCharArray'];
	this.membersByReturnType['Object'] = ['Clone', 'GetValue', 'SyncRoot', 'GetLifetimeService', 'InitializeLifetimeService', 'GetFormat', 'Current'];
	this.membersByReturnType['CharEnumerator'] = ['GetEnumerator'];
	this.membersByReturnType['ParallelQuery`1'] = ['AsParallel'];
	this.membersByReturnType['IQueryable`1'] = ['AsQueryable'];
	this.membersByReturnType['ParallelQuery'] = ['AsParallel'];
	this.membersByReturnType['IQueryable'] = ['AsQueryable'];
	this.membersByReturnType['IEnumerator'] = ['GetEnumerator'];
	this.membersByReturnType['StringBuilder'] = ['Clear', 'Append', 'AppendLine', 'Insert', 'Remove', 'AppendFormat', 'Replace'];
	this.membersByReturnType['Enumerator'] = ['GetEnumerator'];
	this.membersByReturnType['IEqualityComparer`1'] = ['Comparer'];
	this.membersByReturnType['KeyCollection'] = ['Keys'];
	this.membersByReturnType['ValueCollection'] = ['Values'];
	this.membersByReturnType['TValue'] = ['Item', 'Value'];
	this.membersByReturnType['DirectoryInfo'] = ['CreateSubdirectory', 'Parent', 'Root', 'Directory'];
	this.membersByReturnType['DirectorySecurity'] = ['GetAccessControl'];
	this.membersByReturnType['FileInfo[]'] = ['GetFiles'];
	this.membersByReturnType['DirectoryInfo[]'] = ['GetDirectories'];
	this.membersByReturnType['FileSystemInfo[]'] = ['GetFileSystemInfos'];
	this.membersByReturnType['ObjRef'] = ['CreateObjRef'];
	this.membersByReturnType['DateTime'] = ['CreationTime', 'CreationTimeUtc', 'LastAccessTime', 'LastAccessTimeUtc', 'LastWriteTime', 'LastWriteTimeUtc'];
	this.membersByReturnType['FileAttributes'] = ['Attributes'];
	this.membersByReturnType['FileSecurity'] = ['GetAccessControl'];
	this.membersByReturnType['StreamReader'] = ['OpenText'];
	this.membersByReturnType['StreamWriter'] = ['CreateText', 'AppendText'];
	this.membersByReturnType['FileInfo'] = ['CopyTo', 'Replace'];
	this.membersByReturnType['FileStream'] = ['Create', 'Open', 'OpenRead', 'OpenWrite'];
	this.membersByReturnType['CompareInfo'] = ['CompareInfo'];
	this.membersByReturnType['TextInfo'] = ['TextInfo'];
	this.membersByReturnType['CultureTypes'] = ['CultureTypes'];
	this.membersByReturnType['NumberFormatInfo'] = ['NumberFormat'];
	this.membersByReturnType['DateTimeFormatInfo'] = ['DateTimeFormat'];
	this.membersByReturnType['Calendar'] = ['Calendar'];
	this.membersByReturnType['Calendar[]'] = ['OptionalCalendars'];
	this.membersByReturnType['T1'] = ['Item1'];
	this.membersByReturnType['T2'] = ['Item2'];
	this.membersByReturnType['T3'] = ['Item3'];
	this.membersByReturnType['T4'] = ['Item4'];
	this.membersByReturnType['Int32[]'] = ['GetGroupNumbers'];
	this.membersByReturnType['RegexOptions'] = ['Options'];
	this.membersByReturnType['TKey'] = ['Key'];

	// Total word count in all dictionary: 1213

	const unknowns = new SamePrefixArray();
	for (var type in this.dynamicMembers) {
		unknowns.AddAll(this.dynamicMembers[type]);
	}

	this.unknown = unknowns.getArray();

	this.getCompletions = function (beforeDot, start, afterDot) {
		let res;
		if(afterDot) {
			if(typeof this.synonym[beforeDot] != "undefined")
				beforeDot = this.synonym[beforeDot];
			res = this.AutocompliteAfterDotWords(beforeDot, start);
		} else {
			if(typeof beforeDot != "undefined" && beforeDot !== "") {
				if(typeof this.synonym[beforeDot] != "undefined")
					beforeDot = this.synonym[beforeDot];
				res = this.AutocompliteAfterDotWords(beforeDot, start);
			} else {
				res = new SamePrefixArray(start);
				res.AddAll(this.types);
				res.AddAll(this.keywords);
			}
		}
		return res.getArray().sort();
	}

	this.AutocompliteAfterDotWords = function (beforeDot, start) {
		const found = new SamePrefixArray(start);
		if(arrayContains(this.types, beforeDot)) {
			if(this.staticMembers[beforeDot] !== undefined) {
				found.AddAll(this.staticMembers[beforeDot]);
			} else {
				found.AddAll(this.unknown);
			}
		} else {
			let isWasFound = false;
			for (let type in this.membersByReturnType) {
				for (let element in this.membersByReturnType[type]) {
					if(beforeDot === this.membersByReturnType[type][element]) {
						if(type === "Void") {
							isWasFound = true;
							break;
						}
						if(this.dynamicMembers[type] !== undefined) {
							found.AddAll(this.dynamicMembers[type]);
							isWasFound = true;
						}
					}
				}
			}
			if(!isWasFound) {
				found.AddAll(this.unknown);
			}
		}
		return found;
	}
}

function arrayContains(arr, item) {
	if(!Array.prototype.indexOf) {
		let i = arr.length;
		while (i--) {
			if(arr[i] === item) {
				return true;
			}
		}
		return false;
	}
	return arr.indexOf(item) !== -1;
}

function SamePrefixArray(prefix) {
	this.prefix = typeof prefix == "undefined" ? '' : prefix.toLowerCase();
	this.container = [];

	this.AddAll = function (list) {
		for (let i = 0; i < list.length; ++i)
			this.Add(list[i]);
	}
	this.Add = function (str) {
		if(str.toLowerCase().lastIndexOf(this.prefix, 0) === 0 && !arrayContains(this.container, str))
			this.container.push(str);
	}
	this.getArray = function () {
		return this.container;
	}
}

export default CsCompleter;
