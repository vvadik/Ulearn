export function saveToCache(groupName: string, id: string, value: string): void {
	const data = JSON.parse(localStorage[groupName] || '{}');
	data[id] = value;
	localStorage[groupName] = JSON.stringify(data);
}

export function loadFromCache(groupName: string, id: string): string {
	const data = JSON.parse(localStorage[groupName] || '{}');
	return data[id];
}

export const exerciseSolutions = 'exercise_solutions';
