import api from "../api/"

// Groups
export function getCourseGroups(courseId) {
	return api.get("groups/in/" + courseId)
		.then(response => response.json())
}

export function getCourseArchiveGroups(courseId) {
	return api.get("groups/in/" + courseId + "/archived")
		.then(response => response.json())
}

// Group
export function getGroup(groupId) {
	return api.get("groups/" + groupId)
		.then(response => response.json());
}

export function createGroup(courseId, name) {
	return api.post("groups/in/" + courseId,
		createRequestParams({ name }))
		.then(response => response.json());
}

export function copyGroup(groupId, destinationCourseId, makeMeOwner) {
	return api.post("groups/" + groupId + "/copy?destination_course_id="
		+ encodeURIComponent(destinationCourseId) + '&make_me_owner=' + makeMeOwner)
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

export function changeGroupOwner(groupId, ownerId) {
	return api.put("groups/" + groupId + '/owner',
		createRequestParams({owner_id: ownerId}))
		.then(response => response.json());
}

// Scores
export function getGroupScores(groupId) {
	return api.get("groups/" + groupId + '/scores')
		.then(response => response.json());
}

export function saveScoresSettings(groupId, scoresId) {
	return api.post("groups/" + groupId + '/scores', {
			headers: {
				'Content-Type': 'application/json'
			},
			body:
				JSON.stringify({ scores: scoresId }),
			})
		.then(response => response.json());
}

// Accesses
export function getGroupAccesses(groupId) {
	return api.get("groups/" + groupId + "/accesses")
		.then(response => response.json());
}

export function addGroupAccesses(groupId, userId) {
	return api.post("groups/" + groupId + "/accesses/" + userId)
		.then(response => response.json());
}

export function removeAccess(groupId, userId) {
	return api.delete("groups/" + groupId + "/accesses/" + userId)
		.then(response => response.json());
}

//Students
export function getStudents(groupId) {
	return api.get("groups/" + groupId + "/students")
		.then(response => response.json());
}

export function deleteStudents(groupId, students) {
	return api.delete("groups/" + groupId + "/students/",
		createRequestParams({'student_ids': students}))
		.then(response => response.json());
}

export function copyStudents(groupId, destinationGroupId, students) {
	return api.post("groups/" + groupId + "/students/copy/to/" + destinationGroupId,
		createRequestParams({'student_ids': students}))
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
