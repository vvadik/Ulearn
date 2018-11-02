import api from "../api/"

export function getCourseGroups(courseId) {
	return api.get("groups/in/" + courseId)
		.then(response => response.json())
}

export function getCourseArchiveGroups(courseId) {
	return api.get("groups/in/" + courseId + "/archived")
		.then(response => response.json())
}

export function getGroup(groupId) {
	return api.get("groups/" + groupId)
		.then(response => response.json());
}

export function createGroup(courseId, name) {
	return api.post("groups/in/" + courseId,  {
		headers: {
			'Content-Type': 'application/json'
		},
		body: JSON.stringify({ name: name })
	}).then(response => response.json());
}

export function saveGroupSettings(groupId, groupSettings) {
	return api.patch("groups/" + groupId, {
		headers: {
			'Content-Type': 'application/json'
		},
		body: JSON.stringify(groupSettings)
	}).then(response => response.json());
}

export function deleteGroup(groupId) {
	return api.delete("groups/" + groupId)
		.then(response => response.json());
}
