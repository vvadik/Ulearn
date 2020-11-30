export function saveToCache(groupName, id, value) {
	const data = JSON.parse(localStorage[groupName] || '{}');
	data[id] = value;
	localStorage[groupName] = JSON.stringify(data);
}

export function loadFromCache(groupName, id) {
	const data = JSON.parse(localStorage[groupName] || '{}');
	return data[id];
}

export const exerciseSolutions = 'exercise_solutions';
