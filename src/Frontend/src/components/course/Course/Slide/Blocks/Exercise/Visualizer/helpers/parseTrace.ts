function parseGlobals(globals: Record<string, any>) : Record<string, any>{
	const parsedGlobals = {}

	for (const key in globals) {
		const val = globals[key];
		const parsedVal = parseVal(val);
		if (parsedVal !== null) {
			parsedGlobals[key] = parseVal(val);
		}
	}

	return parsedGlobals;
}

function parseVal(val: any) : any {
	if (!Array.isArray(val)) {
		return val;
	}

	switch (val[0]) {
		case 'INSTANCE':
			return null;
		case 'LIST':
			return parseList(val);
		case 'DICT':
			return parseDict(val);
		case 'SET':
			return parseList(val);
		case 'TUPLE':
			return parseList(val);
	}
}

function parseList(data) {
	return data.slice(2).map(parseVal);
}

function parseDict(data) {
	const dict = {};
	for (const key of data.slice(2).values()) {
		dict[key[0]] = parseVal(key[1]);
	}
	return dict;
}

export default parseGlobals;
