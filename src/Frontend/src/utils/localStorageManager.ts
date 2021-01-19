export function saveToCache<T>(groupName: string, id: string, value: T): void {
	const data = JSON.parse(localStorage[groupName] || '{}');
	data[id] = value;
	localStorage[groupName] = JSON.stringify(data);
}

export function loadFromCache<T>(groupName: string, id: string): T | undefined {
	const data = JSON.parse(localStorage[groupName] || '{}');
	return data[id];
}

export const exerciseSolutions = 'exercise_solutions';
