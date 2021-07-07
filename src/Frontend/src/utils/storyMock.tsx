export function returnPromiseAfterDelay<T>(ms: number,
	result?: T,
	callback?: () => void,
): Promise<T> {
	if(result) {
		return new Promise(resolve => setTimeout(resolve, ms))
			.then(() => {
				callback?.();
				return (result);
			});
	}
	return new Promise(resolve => setTimeout(resolve, ms));
}

export function mockFunc() {
	return ({});
}
