import {
	COURSES__COURSE_ENTERED,
	COURSES__UPDATED,
	COURSES__COURSE_LOAD,
	COURSES__FLASHCARDS_INFO,
	COURSES_FLASHCARDS_PACK,
	START, SUCCESS, FAIL, COURSES_FLASHCARDS_STATISTICS, COURSES_FLASHCARDS_RATE,
} from '../consts/actions';

const initialCoursesState = {
	courseById: {},
	currentCourseId: undefined,
	fullCoursesInfo: {},
	courseLoading: false,

	flashcardsInfo: {},
	flashcardsInfoLoading: false,

	flashcardsPackByCourses: {},
	flashcardsPackByCoursesLoading: false,

	flashcardsStatisticsByUnits: {},
	flashcardsStatisticsByUnitsLoading: false,
};

export default function courses(state = initialCoursesState, action) {
	switch (action.type) {
		case COURSES__UPDATED:
			return {
				...state,
				courseById: action.courseById
			};
		case COURSES__COURSE_ENTERED:
			return {
				...state,
				currentCourseId: action.courseId
			};
		case COURSES__COURSE_LOAD + START:
			return {
				...state,
				courseLoading: true,
			};
		case COURSES__COURSE_LOAD + SUCCESS:
			return {
				...state,
				courseLoading: false,
				fullCoursesInfo: {
					...state.fullCoursesInfo,
					[action.courseId]: action.result,
				}
			};
		case COURSES__COURSE_LOAD + FAIL:
			return {
				...state,
				courseLoading: false,
			};
		case COURSES__FLASHCARDS_INFO + START:
			return {
				...state,
				flashcardsLoading: true,
			};
		case COURSES__FLASHCARDS_INFO + SUCCESS:
			return {
				...state,
				flashcardsLoading: false,
				flashcardsInfo: {
					...state.flashcardsInfo,
					[action.courseId]: action.result,
				},
			};
		case COURSES__FLASHCARDS_INFO + FAIL:
			return {
				...state,
				flashcardsLoading: false,
			};
		case COURSES_FLASHCARDS_PACK + START:
			return {
				...state,
				flashcardsPackByCoursesLoading: true,
			};
		case COURSES_FLASHCARDS_PACK + SUCCESS:
			return {
				...state,
				flashcardsPackByCoursesLoading: false,
				flashcardsPackByCourses: {
					...state.flashcardsPackByCourses,
					[action.courseId]: action.result,
				}
			};
		case COURSES_FLASHCARDS_PACK + FAIL:
			return {
				...state,
				flashcardsPackByCoursesLoading: false,
			};
		case COURSES_FLASHCARDS_STATISTICS + START:
			return {
				...state,
				flashcardsStatisticsByUnitsLoading: true,
			};
		case COURSES_FLASHCARDS_STATISTICS + SUCCESS:
			return {
				...state,
				flashcardsStatisticsByUnitsLoading: false,
				flashcardsStatisticsByUnits: {
					...state.flashcardsStatisticsByUnits,
					[action.unitId]: action.result,
				}
			};
		case COURSES_FLASHCARDS_STATISTICS + FAIL:
			return {
				...state,
				flashcardsStatisticsByUnitsLoading: false,
			};

		case COURSES_FLASHCARDS_RATE + SUCCESS:
			return {
				...state,
				flashcardsStatisticsByUnitsLoading: false,
			};

		default:
			return state;
	}
}