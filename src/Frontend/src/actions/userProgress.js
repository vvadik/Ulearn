import {
	USER__PROGRESS_LOAD,
	USER__PROGRESS_UPDATE,
	USER__PROGRESS_HIJACK,
	START, SUCCESS, FAIL,
} from '../consts/actions';

import { getUserProgressInCourse, updateUserProgressInCourse } from '../api/userProgress';

const loadUserProgressStart = () => ({
	type: USER__PROGRESS_LOAD + START,
});

const loadUserProgressSuccess = (courseId, result) => ({
	type: USER__PROGRESS_LOAD + SUCCESS,
	courseId,
	result,
});

const loadUserProgressFail = () => ({
	type: USER__PROGRESS_LOAD + FAIL,
});

const userProgressUpdateAction = (courseId, slideId) => ({
	type: USER__PROGRESS_UPDATE,
	courseId,
	slideId,
});

export const userProgressUpdate = (courseId, slideId) => {
	courseId = courseId.toLowerCase();

	return (dispatch) => {
		dispatch(userProgressUpdateAction(courseId, slideId));
		updateUserProgressInCourse(courseId, slideId)
			.catch(err => {
				console.error(err); // TODO rozentor handle error
			})
	};
};

export const loadUserProgress = (courseId, userId) => {
	courseId = courseId.toLowerCase();

	return (dispatch) => {
		dispatch(loadUserProgressStart());

		getUserProgressInCourse(courseId)
			.then(result => {
				const progress = result.userProgress[userId] ? result.userProgress[userId].visitedSlides : {};
				dispatch(loadUserProgressSuccess(courseId, progress));
			})
			.catch(err => {
				dispatch(loadUserProgressFail());
			});
	};
};

const userProgressHijackAction = (isHijacked) => ({
	type: USER__PROGRESS_HIJACK,
	isHijacked,
});

export const setHijack = (isHijacked) => {
	return (dispatch) => {
		dispatch(userProgressHijackAction(isHijacked));
	}
}
