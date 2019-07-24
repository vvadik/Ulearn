import {
	COURSES__COURSE_ENTERED,
	COURSES__UPDATED,
	COURSES__COURSE_LOAD,
	COURSES__FLASHCARDS,
	COURSES__FLASHCARDS_RATE,
	START, SUCCESS, FAIL,
} from '../consts/actions';
import { rateTypes } from "../consts/rateTypes";

const initialCoursesState = {
	courseById: {},
	currentCourseId: undefined,
	fullCoursesInfo: {},
	courseLoading: false,

	flashcardsLoading: false,
	flashcardsByCourses: {},
	flashcardsByUnits: {},
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
			const courseFlashcards = {},
				flashcardsByUnits = {};
			const courseUnits = state.fullCoursesInfo[action.courseId].units;

			for (const unit of action.result) {
				const { unitId, unitTitle, unlocked, flashcards } = unit;
				const flashcardsIds = [];
				const unitSlides = courseUnits
					.find(unit => unit.id === unitId)
					.slides;
				let unratedFlashcardsCount = 0;

				for (const flashcard of flashcards) {
					courseFlashcards[flashcard.id] = flashcard;
					flashcardsIds.push(flashcard.id);
					if (flashcard.rate === rateTypes.notRated) {
						unratedFlashcardsCount++;
					}
				}

				flashcardsByUnits[unitId] = {
					unitId,
					unitTitle,
					unlocked,
					flashcardsIds,
					unratedFlashcardsCount,
					cardsCount: flashcards.length,
					flashcardsSlideSlug: unitSlides[unitSlides.length - 1].slug,
				}
			}

			return {
				...state,
				flashcardsLoading: false,
				flashcardsByCourses: {
					...state.flashcardsByCourses,
					[action.courseId]: courseFlashcards,
				},
				flashcardsByUnits: {
					...state.flashcardsByUnits,
					...flashcardsByUnits,
				},
			};
		case COURSES__FLASHCARDS + FAIL:
			return {
				...state,
				flashcardsLoading: false,
			};
		case COURSES__FLASHCARDS_RATE + START:
			const flashcard = { ...state.flashcardsByCourses[action.courseId][action.flashcardId] };

			flashcard.rate = action.rate;
			flashcard.lastRateIndex = action.newTLast;

			const newState = {
				...state,
				flashcardsByCourses: {
					...state.flashcardsByCourses,
					[action.courseId]: {
						...state.flashcardsByCourses[action.courseId],
						[action.flashcardId]: flashcard
					},
				},
			};

			const unitInfo = newState.flashcardsByUnits[action.unitId];

			if (unitInfo.unratedFlashcardsCount > 0) {
				unitInfo.unratedFlashcardsCount--;
			}

			if (unitInfo.unratedFlashcardsCount === 0) {
				unitInfo.unlocked = true;
			}

			return newState;
		default:
			return state;
	}
}