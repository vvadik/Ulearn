import {
	COURSES__COURSE_ENTERED,
	COURSES__COURSE_LOAD,
	COURSES__FLASHCARDS,
	COURSES__FLASHCARDS_RATE,
	COURSES__SLIDE_LOAD,
	COURSES__SLIDE_READY,
	COURSES__COURSE_LOAD_ERRORS,
	COURSES__EXERCISE_ADD_SUBMISSIONS,
	START, SUCCESS, FAIL,
} from "../consts/actions";

import { getCourse, getCourseErrors } from 'src/api/courses';
import { getSlide, submitCode, } from "src/api/slides";
import {
	getFlashcards,
	putFlashcardStatus,
} from '../api/flashcards';

export const changeCurrentCourseAction = (courseId) => ({
	type: COURSES__COURSE_ENTERED,
	courseId,
});

const loadCourseStart = () => ({
	type: COURSES__COURSE_LOAD + START,
});

const loadCourseSuccess = (courseId, result) => ({
	type: COURSES__COURSE_LOAD + SUCCESS,
	courseId,
	result,
});

const loadCourseErrorsSuccess = (courseId, result) => ({
	type: COURSES__COURSE_LOAD_ERRORS,
	courseId,
	result,
});

const loadCourseFail = (error) => ({
	type: COURSES__COURSE_LOAD + FAIL,
	error,
});

const loadFlashcardsStart = () => ({
	type: COURSES__FLASHCARDS + START,
});

const loadFlashcardsSuccess = (courseId, result) => ({
	type: COURSES__FLASHCARDS + SUCCESS,
	courseId,
	result,
});

const loadFlashcardsFail = () => ({
	type: COURSES__FLASHCARDS_RATE + FAIL,
});

const sendFlashcardResultStart = (courseId, unitId, flashcardId, rate, newTLast) => ({
	type: COURSES__FLASHCARDS_RATE + START,
	courseId,
	unitId,
	flashcardId,
	rate,
	newTLast,
});

const loadSlideStart = () => ({
	type: COURSES__SLIDE_LOAD + START,
});

const loadSlideSuccess = (courseId, slideId, result) => ({
	type: COURSES__SLIDE_LOAD + SUCCESS,
	courseId,
	slideId,
	result,
});

const loadSlideFail = (error) => ({
	type: COURSES__SLIDE_LOAD + FAIL,
	error,
});

const slideReadyAction = (isSlideReady) => ({
	type: COURSES__SLIDE_READY,
	isSlideReady,
});

const addSubmissionAction = (courseId, slideId, result) => ({
	type: COURSES__EXERCISE_ADD_SUBMISSIONS,
	courseId,
	slideId,
	result,
});

export const loadCourse = (courseId) => {
	courseId = courseId.toLowerCase();

	return (dispatch) => {
		dispatch(loadCourseStart());

		getCourse(courseId)
			.then(result => {
				dispatch(loadCourseSuccess(courseId, result));
			})
			.catch(err => {
				dispatch(loadCourseFail(err.status));
			});
	};
};
export const loadCourseErrors = (courseId) => {
	courseId = courseId.toLowerCase();

	return (dispatch) => {
		getCourseErrors(courseId)
			.then(result => {
				if(result.status === 204) {
					dispatch(loadCourseErrorsSuccess(courseId, null));
				} else {
					dispatch(loadCourseErrorsSuccess(courseId, result.tempCourseError));
				}
			})
			.catch(err => {
				dispatch(loadCourseErrorsSuccess(courseId, null));
			});
	};
};

export const loadFlashcards = (courseId) => {
	courseId = courseId.toLowerCase();

	return (dispatch) => {
		dispatch(loadFlashcardsStart());

		getFlashcards(courseId)
			.then(result => {
				dispatch(loadFlashcardsSuccess(courseId, result.units));
			})
			.catch(err => {
				dispatch(loadFlashcardsFail());
			});
	}
};

export const sendFlashcardResult = (courseId, unitId, flashcardId, rate, newTLast) => {
	courseId = courseId.toLowerCase();

	return (dispatch) => {
		dispatch(sendFlashcardResultStart(courseId, unitId, flashcardId, rate, newTLast));
		putFlashcardStatus(courseId, flashcardId, rate)
			.catch(err => {
			});
	}
};

export const loadSlide = (courseId, slideId) => {
	courseId = courseId.toLowerCase();

	return (dispatch) => {
		dispatch(loadSlideStart());

		getSlide(courseId, slideId)
			.then(result => {
				dispatch(loadSlideSuccess(courseId, slideId, result));
			})
			.catch(err => {
				dispatch(loadSlideFail(err));
			});
	};
};

export const setSlideReady = (isSlideReady) => {
	return (dispapcth) => {
		dispapcth(slideReadyAction(isSlideReady));
	}
}

export const sendCode = (courseId, slideId, code,) => {
	return (dispatch) => {
		submitCode(courseId, slideId, code,)
			.then(r => {
				if(r.submission !== null) {
					dispatch(addSubmissionAction(courseId, slideId, r));
				}
			});
	};
}
//TODO add handling send code and add review
