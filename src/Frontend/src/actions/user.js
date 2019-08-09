import {
	USER__PROGRESS_LOAD,
	USER__PROGRESS_UPDATE,
	START, SUCCESS, FAIL,
} from '../consts/actions';

import { getUserProgressInCourse, updateUserProgressInCourse } from '../api/user';

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
	return (dispatch) => {
		dispatch(userProgressUpdateAction(courseId, slideId));
		updateUserProgressInCourse(courseId, slideId)
			.catch(err => {
				console.error(err); // TODO rozentor handle error
			})
	};
};

export const loadUserProgress = (courseId) => {
	return (dispatch) => {
		dispatch(loadUserProgressStart());

		getUserProgressInCourse(courseId)
			.then(result => {
				dispatch(loadUserProgressSuccess(courseId, result.visitedSlides));
			})
			.catch(err => {
				dispatch(loadUserProgressFail());
			});
	};
};