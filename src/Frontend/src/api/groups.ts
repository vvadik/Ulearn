import api from "src/api/index";
import { resetStudentsLimits } from "src/consts/routes";
import {
	CopyGroupResponse,
	GroupAccessesResponse,
	GroupInfo,
	GroupScoringGroupsResponse,
	GroupStudentsResponse
} from "src/models/groups";

// Groups
export function getCourseGroups(courseId: string): Promise<{ groups: GroupInfo[] }> {
	return api.get("groups?courseId=" + courseId);
}

export function getCourseArchivedGroups(courseId: string): Promise<{ groups: GroupInfo[] }> {
	return api.get("groups?courseId=" + courseId + "&archived=true");
}

// Group
export function getGroup(groupId: number): Promise<GroupInfo> {
	return api.get("groups/" + groupId);
}

export function createGroup(courseId: string, name: string): Promise<Response> {
	return api.post("groups?courseId=" + courseId,
		api.createRequestParams({ name }));
}

export function copyGroup(groupId: number, destinationCourseId: string,
	makeMeOwner: boolean
): Promise<CopyGroupResponse> {
	return api.post("groups/" + groupId + "/copy?destinationCourseId="
		+ encodeURIComponent(destinationCourseId) + '&makeMeOwner=' + makeMeOwner);
}

export function saveGroupSettings(groupId: number, groupSettings: Record<string, unknown>): Promise<GroupInfo> {
	return api.patch("groups/" + groupId,
		api.createRequestParams(groupSettings));
}

export function deleteGroup(groupId: number): Promise<Response> {
	return api.delete("groups/" + groupId);
}

export function changeGroupOwner(groupId: number, ownerId: string): Promise<Response> {
	return api.put("groups/" + groupId + '/owner',
		api.createRequestParams({ ownerId }));
}

// Scores
export function getGroupScores(groupId: number): Promise<GroupScoringGroupsResponse> {
	return api.get("groups/" + groupId + '/scores');
}

export function saveScoresSettings(groupId: number, checkedScoresSettingsIds: string[]): Promise<Response> {
	return api.post("groups/" + groupId + '/scores',
		api.createRequestParams({ 'scores': checkedScoresSettingsIds }));
}

// Accesses
export function getGroupAccesses(groupId: number): Promise<GroupAccessesResponse> {
	return api.get("groups/" + groupId + "/accesses");
}

export function addGroupAccesses(groupId: number, userId: string): Promise<Response> {
	return api.post("groups/" + groupId + "/accesses/" + userId);
}

export function removeAccess(groupId: number, userId: string): Promise<Response> {
	return api.delete("groups/" + groupId + "/accesses/" + userId);
}

// Students
export function getStudents(groupId: number): Promise<GroupStudentsResponse> {
	return api.get("groups/" + groupId + "/students");
}

export function deleteStudents(groupId: number, studentIds: string[]): Promise<Response> {
	return api.delete("groups/" + groupId + "/students",
		api.createRequestParams({ studentIds }));
}

export function copyStudents(groupId: number, studentIds: string[]): Promise<Response> {
	return api.post("groups/" + groupId + "/students",
		api.createRequestParams({ studentIds }));
}

export function resetLimitsForStudents(groupId: number, studentIds: string[]): Promise<Response> {
	return api.post("groups/" + groupId + resetStudentsLimits,
		api.createRequestParams({ studentIds }));
}
