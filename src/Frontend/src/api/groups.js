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
	return api.post("groups/in/" + courseId,
		createRequestParams({ name: name }))
		.then(response => response.json());
}

export function copyGroup(groupId, destination_course_id, value) {
	return api.post("groups/" + groupId + "/copy?destination_course_id="
		+ encodeURIComponent(destination_course_id) + '&make_me_owner=' + value)
		.then(response => response.json());
}

export function saveGroupSettings(groupId, groupSettings) {
	return api.patch("groups/" + groupId,
		createRequestParams(groupSettings))
		.then(response => response.json());
}

export function deleteGroup(groupId) {
	return api.delete("groups/" + groupId)
		.then(response => response.json());
}

export function changeGroupOwner(groupId, owner) {
	return api.put("groups/" + groupId + '/owner',
		createRequestParams({owner: owner}))
		.then(response => response.json());
}

export function getGroupScores(groupId) {
	return api.get("groups/" + groupId + '/scores')
		.then(response => response.json());
}

export function saveScoresSettings(groupId, scores) {
	return api.post("groups/" + groupId + '/scores', {
			headers: {
				'Content-Type': 'application/json'
			},
			body:
				JSON.stringify({ scores: scores }),
			})
		.then(response => response.json());
}

function createRequestParams(body) {
	return {
		headers: {
			'Content-Type': 'application/json'
		},
		body: JSON.stringify(body)
	}
}
