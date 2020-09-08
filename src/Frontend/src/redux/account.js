import {
	ACCOUNT__USER_HIJACK,
	ACCOUNT__USER_INFO_UPDATED,
	ACCOUNT__USER_ROLES_UPDATED,
} from "src/consts/actions";

const initialAccountState = {
	accountLoaded: false,
	isAuthenticated: false,
	isSystemAdministrator: false,
	accountProblems: [],
	systemAccesses: [],
	roleByCourse: {},
	accessesByCourse: {},
	groupsAsStudent: [],
	gender: null,
	isHijacked: false,
};

function account(state = initialAccountState, action) {
	switch (action.type) {
		case ACCOUNT__USER_INFO_UPDATED:
			let newState = { ...state };
			newState.isAuthenticated = action.isAuthenticated;
			newState.accountLoaded = true;
			if(newState.isAuthenticated) {
				newState.id = action.id;
				newState.login = action.login;
				newState.firstName = action.firstName;
				newState.lastName = action.lastName;
				newState.visibleName = action.visibleName;
				newState.avatarUrl = action.avatarUrl;
				newState.accountProblems = action.accountProblems;
				newState.systemAccesses = action.systemAccesses;
				newState.gender = action.gender;
			}
			return newState;
		case ACCOUNT__USER_ROLES_UPDATED:
			return {
				...state,
				isSystemAdministrator: action.isSystemAdministrator,
				roleByCourse: action.roleByCourse,
				accessesByCourse: action.accessesByCourse,
				groupsAsStudent: action.groupsAsStudent,
			};
		case ACCOUNT__USER_HIJACK:
			return {
				...state,
				isHijacked: action.isHijacked,
			}
		default:
			return state;
	}
}

export default account;
