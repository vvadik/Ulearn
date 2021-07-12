export function returnPromiseAfterDelay<T>(ms: number, result?: T): Promise<T> {
	if(result) {
		return new Promise(resolve => setTimeout(resolve, ms)).then(() => (result));
	}
	return new Promise(resolve => setTimeout(resolve, ms));
}

export function mockFunc() {
	return ({});
}
