import {
	NOTIFICATIONS__COUNT_RESETED,
	NOTIFICATIONS__COUNT_UPDATED,
} from "src/consts/actions";

const initialNotificationsState = {
	count: 0,
	lastTimestamp: "",
};

function notifications(state = initialNotificationsState, action) {
	switch (action.type) {
		case NOTIFICATIONS__COUNT_UPDATED:
			return {
				...state,
				count: state.count + action.count,
				lastTimestamp: action.lastTimestamp
			};
		case NOTIFICATIONS__COUNT_RESETED:
			return {
				...state,
				count: 0
			};
		default:
			return state;
	}
}

export default notifications;
