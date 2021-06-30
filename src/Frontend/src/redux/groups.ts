import { GroupInfo } from "src/models/groups";
import { ReduxData } from "./index";
import {
	GROUPS_LOAD_FAIL,
	GROUPS_LOAD_START,
	GROUPS_LOAD_SUCCESS,
	GroupsAction,
	GroupsLoadStartAction,
	GroupsLoadSuccessAction,
	GroupsLoadFailAction,
} from "src/actions/groups.types";

interface CourseGroups {
	byGroupId: {
		[groupId: string]: GroupInfo | ReduxData;
	}
}

interface GroupsState {
	groupsByCourseId: {
		[courseId: string]: CourseGroups | ReduxData;
	}
	groupsIdsByUserId: {
		[userId: string]: string[] | ReduxData;
	}
}

const initialDeviceState: GroupsState = {
	groupsByCourseId: {},
	groupsIdsByUserId: {},
};

export default function groupsReducer(state: GroupsState = initialDeviceState, action: GroupsAction): GroupsState {
	switch (action.type) {
		case GROUPS_LOAD_START: {
			const { courseId, userId, } = action as GroupsLoadStartAction;
			return {
				...state,
				groupsByCourseId: {
					...state.groupsByCourseId,
					[courseId]: {
						isLoading: true,
					},
				},
				groupsIdsByUserId: {
					...state.groupsIdsByUserId,
					[userId]: { isLoading: true },
				}
			};
		}
		case GROUPS_LOAD_SUCCESS: {
			const { courseId, groups, userId, } = action as GroupsLoadSuccessAction;
			const courseGroups = state.groupsByCourseId[courseId];
			const oldGroups = (courseGroups as CourseGroups).byGroupId || undefined;
			const groupsByGroupId = groups.reduce((pv, cv,) => ({ ...pv, [cv.id]: cv }), {});

			return {
				...state,
				groupsByCourseId: {
					...state.groupsByCourseId,
					[courseId]: {
						byGroupId: {
							...oldGroups,
							...groupsByGroupId,
						},
					},
				},
				groupsIdsByUserId: {
					...state.groupsIdsByUserId,
					[userId]: Object.keys(groupsByGroupId),
				}
			};
		}
		case GROUPS_LOAD_FAIL: {
			const { courseId, error, userId, } = action as GroupsLoadFailAction;
			return {
				...state,
				groupsByCourseId: {
					...state.groupsByCourseId,
					[courseId]: { error },
				},
				groupsIdsByUserId: {
					[userId]: { error: error },
				}
			};
		}
		default:
			return state;
	}
}
