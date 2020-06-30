import {
	COURSES__SLIDE_LOAD,
	START, SUCCESS, FAIL,
} from '../consts/actions';

const initialCoursesSlidesState = {
	slidesByCourses: {},
	slideLoading: false,
	slideError: null,
};

export default function slides(state = initialCoursesSlidesState, action) {
	switch (action.type) {
		case COURSES__SLIDE_LOAD + START:
			return { ...state,
				slideLoading: true,
				slideError: null,
			};
		case COURSES__SLIDE_LOAD + SUCCESS: {
			const { courseId, slideId, result } = action;
			const { slidesByCourses } = state;

			return {
				...state,
				slideLoading: false,
				slideError: null,
				slidesByCourses: {
					...slidesByCourses,
					[courseId]: {
						...slidesByCourses[courseId],
						[slideId]: result,
					}
				}
			};
		}
		case COURSES__SLIDE_LOAD + FAIL: {
			return {
				...state,
				slideLoading: false,
				slideError: action.error,
			};
		}
		default:
			return state;
	}
}