import { Dispatch } from "redux";

import { getUserProgressInCourse, updateUserProgressInCourse } from '../api/userProgress';
import { SlideUserProgress, UsersProgressResponse } from "../models/userProgress";
import {
	LoadUserProgressFailAction,
	LoadUserProgressStartAction,
	LoadUserProgressSuccessAction,
	UserProgressUpdateAction,
	USER__PROGRESS_LOAD_FAIL,
	USER__PROGRESS_LOAD_START,
	USER__PROGRESS_LOAD_SUCCESS,
	USER__PROGRESS_UPDATE,
	UserProgressActionTypes,
} from "./userProgress.types";


const loadUserProgressStart = (): LoadUserProgressStartAction => ({
	type: USER__PROGRESS_LOAD_START,
});

const loadUserProgressSuccess = (courseId: string,
	result: { [slideId: string]: SlideUserProgress }
): LoadUserProgressSuccessAction => ({
	type: USER__PROGRESS_LOAD_SUCCESS,
	courseId,
	result,
});

const loadUserProgressFail = (): LoadUserProgressFailAction => ({
	type: USER__PROGRESS_LOAD_FAIL,
});

export const userProgressUpdateAction = (courseId: string, slideId: string, fieldsToUpdate: Partial<SlideUserProgress>
): UserProgressUpdateAction => ({
	type: USER__PROGRESS_UPDATE,
	courseId,
	slideId,
	fieldsToUpdate
});

export const userProgressUpdate = (courseId: string, slideId: string
): (dispatch: Dispatch<UserProgressUpdateAction>) => void => {
	courseId = courseId.toLowerCase();

	return (dispatch: Dispatch<UserProgressUpdateAction>) => {
		dispatch(userProgressUpdateAction(courseId, slideId, { visited: true }));
		updateUserProgressInCourse(courseId, slideId)
			.catch((err: Error) => {
				console.error(err); // TODO rozentor handle error
			})
	};
};

export const loadUserProgress = (courseId: string, userId: string
): (dispatch: Dispatch<UserProgressActionTypes>) => void => {
	courseId = courseId.toLowerCase();

	return (dispatch: Dispatch<UserProgressActionTypes>) => {
		dispatch(loadUserProgressStart());

		getUserProgressInCourse(courseId)
			.then((result: UsersProgressResponse) => {
				const progress = result.userProgress[userId] ? result.userProgress[userId].visitedSlides : {};
				dispatch(loadUserProgressSuccess(courseId, progress));
			})
			.catch(() => {
				dispatch(loadUserProgressFail());
			});
	};
};
