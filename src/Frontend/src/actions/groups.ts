import {
	GROUPS_LOAD_START,
	GROUPS_LOAD_SUCCESS,
	GROUPS_LOAD_FAIL,
	GroupsLoadStartAction,
	GroupsLoadSuccessAction,
	GroupsLoadFailAction,
} from "./groups.types";
import { GroupsInfoResponse } from "src/models/groups";

export const groupLoadStartAction = (courseId: string, userId: string,): GroupsLoadStartAction => ({
	type: GROUPS_LOAD_START,
	courseId,
	userId,
});

export const groupLoadSuccessAction = (courseId: string, userId: string,
	groupsInfoResponse: GroupsInfoResponse,
): GroupsLoadSuccessAction => ({
	type: GROUPS_LOAD_SUCCESS,
	courseId,
	userId,
	...groupsInfoResponse,
});

export const groupLoadFailAction = (
	courseId: string,
	userId: string,
	error: string,
): GroupsLoadFailAction => ({
	type: GROUPS_LOAD_FAIL,
	courseId,
	userId,
	error,
});




