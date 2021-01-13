export function getQueryStringParameter(name: string, url: string): string | null {
	if(!url) {
		url = window.location.href;
	}
	name = name.replace(/[[\]]/g, '\\$&');
	const regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)', 'i');
	const results = regex.exec(url);
	if(!results) {
		return null;
	}
	if(!results[2]) {
		return '';
	}
	return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

export function buildQuery(params: Record<string, unknown>, convert: (str: string) => string): string | null {
	if(!params) {
		return null;
	}

	if(!convert) {
		convert = (str) => str;
	}

	const esc = encodeURIComponent;
	const notUndefinedParams = Object.keys(params)
		.filter(key => params[key] !== undefined);

	if(notUndefinedParams.length === 0) {
		return null;
	}

	return '?' + notUndefinedParams
		.map(key => convert(esc(key)) + '=' + esc(params[key] as string | number | boolean).toLowerCase())
		.join('&');
}
