import {
	COURSES__COURSE_ENTERED,
	COURSES__UPDATED,
	COURSES__COURSE_LOAD,
	COURSES__FLASHCARDS,
	COURSES__FLASHCARDS_RATE,
	COURSES__SLIDE_READY,
	START, SUCCESS, FAIL,
	COURSES__COURSE_LOAD_ERRORS,
} from '../consts/actions';
import { RateTypes } from "../consts/rateTypes";
import { flashcards as flashcardsSlideType } from "../consts/routes";

const initialCoursesState = {
	courseById: {},
	currentCourseId: undefined,
	fullCoursesInfo: {},
	courseLoading: false,
	courseLoadingErrorStatus: null,

	flashcardsLoading: false,
	flashcardsByCourses: {},
	flashcardsByUnits: {},

	isSlideReady: false,
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
				courseLoadingErrorStatus: null,
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
		case COURSES__COURSE_LOAD_ERRORS:
			return {
				...state,
				fullCoursesInfo: {
					...state.fullCoursesInfo,
					[action.courseId]: {
						...state.fullCoursesInfo[action.courseId],
						tempCourseError: action.result
					},
				}
			};
		case COURSES__COURSE_LOAD + FAIL:
			return {
				...state,
				courseLoading: false,
				courseLoadingErrorStatus: action.error,
			};
		case COURSES__FLASHCARDS + START:
			return {
				...state,
				flashcardsLoading: true,
			};
		case COURSES__FLASHCARDS + SUCCESS: {
			let { courseId, result } = action;
			courseId = courseId.toLowerCase();
			const courseFlashcards = {},
				flashcardsByUnits = {};
			const courseUnits = state.fullCoursesInfo[courseId].units;

			for (const unit of result) {
				const { unitId, unitTitle, unlocked, flashcards } = unit;
				const flashcardsIds = [];
				const unitInfo = courseUnits
					.find(unit => unit.id === unitId);
				if(!unitInfo) {
					continue;
				}
				const unitSlides = unitInfo.slides;
				const flashcardsSlide = unitSlides.find(slide => slide.type === flashcardsSlideType);
				const flashcardsSlideSlug = flashcardsSlide ? flashcardsSlide.slug : "";
				let unratedFlashcardsCount = 0;

				for (const flashcard of flashcards) {
					courseFlashcards[flashcard.id] = flashcard;
					flashcardsIds.push(flashcard.id);
					if(flashcard.rate === RateTypes.notRated) {
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
					flashcardsSlideSlug,
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
		}
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

			if(unitInfo.unratedFlashcardsCount > 0) {
				unitInfo.unratedFlashcardsCount--;
			}

			if(unitInfo.unratedFlashcardsCount === 0) {
				unitInfo.unlocked = true;
			}

			return newState;
		case COURSES__SLIDE_READY: {
			return {
				...state,
				isSlideReady: action.isSlideReady,
			};
		}
		default:
			return state;
	}
}
