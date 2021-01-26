import api from "./index";
import { commentPoliciesPath, commentsPath, constructPathToComment } from "src/consts/routes";
import { buildQuery } from "src/utils";
import { convertCamelCaseToSnakeCase } from "src/utils/caseConverter";

export function getComments(courseId, slideId, forInstructors, offset, count) {
	const query = buildQuery({ courseId, slideId, forInstructors, count, offset }, convertCamelCaseToSnakeCase);
	const url = commentsPath + query;

	return api.get(url);
}

export function getComment(commentId) {
	const url = constructPathToComment(commentId);

	return api.get(url);
}

export function addComment(courseId, slideId, text, parentCommentId, forInstructors) {
	const query = buildQuery({ courseId }, convertCamelCaseToSnakeCase);
	const url = commentsPath + query;
	const params = api.createRequestParams({
		slideId,
		text,
		parentCommentId,
		forInstructors,
	});

	return api.post(url, params);
}

export function deleteComment(commentId) {
	const url = constructPathToComment(commentId);

	return api.delete(url);
}

export function updateComment(commentId, commentSettings = {}) {
	const url = constructPathToComment(commentId);
	const params = api.createRequestParams(commentSettings);

	return api.patch(url, params);
}

export function likeComment(commentId) {
	const url = constructPathToComment(commentId, true);

	return api.post(url);
}

export function dislikeComment(commentId) {
	const url = constructPathToComment(commentId, true);

	return api.delete(url);
}

export function getCommentPolicy(courseId) {
	const query = buildQuery({ courseId }, convertCamelCaseToSnakeCase);
	const url = commentPoliciesPath + query;
	return api.get(url);
}

