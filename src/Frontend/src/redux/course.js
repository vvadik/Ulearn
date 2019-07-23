import {
	COURSES__COURSE_ENTERED,
	COURSES__UPDATED,
	COURSES__COURSE_LOAD,
	COURSES__FLASHCARDS,
	COURSES__FLASHCARDS_RATE,
	START, SUCCESS, FAIL,
} from '../consts/actions';

const initialCoursesState = {
	courseById: {},
	currentCourseId: undefined,
	fullCoursesInfo: {},
	courseLoading: false,

	flashcards: {},
	flashcardsLoading: false,
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
		case COURSES__FLASHCARDS + START:
			return {
				...state,
				flashcardsLoading: true,
			};
		case COURSES__FLASHCARDS + SUCCESS:
			return {
				...state,
				flashcardsLoading: false,
				flashcards: {
					...state.flashcards,
					[action.courseId]: action.result,
				},
			};
		case COURSES__FLASHCARDS + FAIL:
			return {
				...state,
				flashcardsLoading: false,
			};
		case COURSES__FLASHCARDS_RATE + SUCCESS:
			const units = { ...state.flashcards[action.courseId] };
			const unitInfo = units.find(unit => unit.unitId === action.unitId);
			unitInfo.flashcards.find(f => f.id === action.flashcardId)
				.rate = action.rate;

			if (unitInfo.flashcards.all(fc => fc.rate !== 'notRated')) {
				unitInfo.unlocked = true;
			}

			return {
				...state,
				flashcards: {
					...state.flashcards,
					[action.courseId]: {
						...units,
					},
				},
			};
		default:
			return state;
	}
}