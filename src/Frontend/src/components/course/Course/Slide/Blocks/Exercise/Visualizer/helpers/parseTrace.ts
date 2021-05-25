interface VisualizerStep {
	line: string;
	event: 'exception' | 'uncaught_exception' | 'return';
	stdout: string;
	globals: any;
	stack_locals: any;
	exception_str?: string;
}

function parseGlobals(globals: Record<string, number | string | Array<any>>) :
	Record<string, any>{
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

const parseVal = (val: number | string | Array<any>) :
	number | string | Array<any> | Record<string, any> | null | undefined => {
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

const parseList = (data: Array<any>) : Array<any> =>
	data.slice(2).map(parseVal);

const parseDict = (data: Record<string, any>) : Record<string, any> => {
	const dict = {};
	for (const key of data.slice(2).values()) {
		dict[key[0]] = parseVal(key[1]);
	}
	return dict;
}

const isObjectEmpty = (obj: Record<string, any>) : boolean =>
	JSON.stringify(obj) === "{}";

const getVariables = (visualizerStep: VisualizerStep) : Record<string, any> => {
	const variables = {};
	const parsedGlobals = parseGlobals(visualizerStep.globals);
	const parsedLocals = visualizerStep.stack_locals;

	if (!isObjectEmpty(parsedGlobals)) {
		variables["Глобальные"] = parsedGlobals;
	}
	if (!isObjectEmpty(parsedLocals)) {
		variables["Локальные"] = parsedLocals;
		console.log(parsedLocals);
	}

	return variables
};


export { getVariables, VisualizerStep };
