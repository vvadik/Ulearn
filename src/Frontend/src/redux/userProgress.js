import {
	USER__PROGRESS_LOAD,
	USER__PROGRESS_UPDATE,
	START, SUCCESS, FAIL,
} from '../consts/actions';

const initialState = {
	progress: {},
	loading: false,
};

export default function userProgressReducer(state = initialState, action) {
	switch (action.type) {
		case USER__PROGRESS_LOAD + START:
			return {
				...state,
				loading: true,
			};
		case USER__PROGRESS_LOAD + SUCCESS:
			return {
				...state,
				loading: false,
				progress: {
					...state.progress,
					[action.courseId]: action.result,
				}
			};
		case USER__PROGRESS_LOAD + FAIL:
			return {
				...state,
				loading: false,
			};
		case USER__PROGRESS_UPDATE :
			return {
				...state,
				progress: {
					...state.progress,
					[action.courseId]: {
						...state.progress[action.courseId],
						[action.slideId]: {
							...state.progress[action.courseId][action.slideId],
							...action.fieldsToUpdate
						},
					},
				}
			};
		default:
			return state;
	}
}
