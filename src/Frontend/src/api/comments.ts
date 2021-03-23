import api from "./index";
import { commentPoliciesPath, commentsPath, constructPathToComment } from "src/consts/routes";
import { buildQuery } from "src/utils";
import { convertCamelCaseToSnakeCase } from "src/utils/caseConverter";
import { SlideComments, Comment, CommentPolicy, } from "src/models/comments";

export function getComments(courseId: string, slideId: string, forInstructors: boolean, offset?: number,
	count?: number
): Promise<SlideComments> {
	const query = buildQuery({ courseId, slideId, forInstructors, count, offset }, convertCamelCaseToSnakeCase);
	const url = commentsPath + query;

	return api.get(url);
}

export function getComment(commentId: string): Promise<Comment> {
	const url = constructPathToComment(commentId);

	return api.get(url);
}

export function addComment(courseId: string, slideId: string, text: string, parentCommentId: string | null,
	forInstructors: boolean
): Promise<Comment> {
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

export function deleteComment(commentId: string): Promise<string> {
	const url = constructPathToComment(commentId);

	return api.delete(url);
}

export function updateComment(commentId: string, commentSettings = {}): Promise<Comment> {
	const url = constructPathToComment(commentId);
	const params = api.createRequestParams(commentSettings);

	return api.patch(url, params);
}

export function likeComment(commentId: string): Promise<string> {
	const url = constructPathToComment(commentId, true);

	return api.post(url);
}

export function dislikeComment(commentId: string): Promise<string> {
	const url = constructPathToComment(commentId, true);

	return api.delete(url);
}

export function getCommentPolicy(courseId: string): Promise<CommentPolicy> {
	const query = buildQuery({ courseId }, convertCamelCaseToSnakeCase);
	const url = commentPoliciesPath + query;
	return api.get(url);
}

