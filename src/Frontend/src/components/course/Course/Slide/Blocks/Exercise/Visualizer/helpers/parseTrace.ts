function parseGlobals(globals: Record<string, any>) : Record<string, any> {
	const parsedGlobals = {}

	for (const key in globals) {
		const val = globals[key];
		if (!Array.isArray(val)) {
			parsedGlobals[key] = val;
			continue;
		}

		switch (val[0]) {
			case 'INSTANCE':
				continue;
			case 'LIST':
				parsedGlobals[key] = parseList(val);
				break;
			case 'DICT':
				parsedGlobals[key] = parseDict(val);
				break;
			case 'SET':
				parsedGlobals[key] = parseList(val);
				break;
			case 'TUPLE':
				parsedGlobals[key] = parseList(val);
				break;
		}
	}

	return parsedGlobals;
}

function parseList(data: Array<any>) : Array<any> {
	return new Array<any>(data.slice(2).values());
}

function parseDict(data: Array<any>) : Record<string, any> {
	const dict = {};
	for (const key of data.slice(2).values()) {
		dict[key[0]] = key[1];
	}

	return dict;
}

export default parseGlobals;
