import {
	INSTRUCTOR__STUDENT_MODE_TOGGLE,
	INSTRUCTOR__STUDENT_INFO_LOAD_START,
	INSTRUCTOR__STUDENT_INFO_LOAD_FAIL,
	INSTRUCTOR__STUDENT_INFO_LOAD_SUCCESS,
	INSTRUCTOR__STUDENT_SUBMISSIONS_LOAD_START,
	INSTRUCTOR__STUDENT_SUBMISSIONS_LOAD_SUCCESS,
	INSTRUCTOR__STUDENT_SUBMISSIONS_LOAD_FAIL,
	INSTRUCTOR__ANTIPLAGIARISM_STATUS_LOAD_START,
	INSTRUCTOR__ANTIPLAGIARISM_STATUS_LOAD_SUCCESS,
	INSTRUCTOR__ANTIPLAGIARISM_STATUS_LOAD_FAIL,
	INSTRUCTOR__FAVOURITE_REVIEWS_LOAD_START,
	INSTRUCTOR__FAVOURITE_REVIEWS_LOAD_SUCCESS,
	INSTRUCTOR__FAVOURITE_REVIEWS_LOAD_FAIL,
	StudentModeAction,
	StudentInfoLoadFailAction,
	StudentInfoLoadStartAction,
	StudentInfoLoadSuccessAction,
	StudentSubmissionsLoadStartAction,
	StudentSubmissionsLoadSuccessAction,
	StudentSubmissionsLoadFailAction,
	AntiplagiarismStatusLoadStartAction,
	AntiplagiarismStatusLoadSuccessAction,
	AntiplagiarismStatusLoadFailAction,
	FavouriteReviewsLoadStartAction,
	FavouriteReviewsLoadSuccessAction,
	FavouriteReviewsLoadFailAction,
} from 'src/actions/instructor.types';
import { ShortUserInfo } from "src/models/users";
import { SubmissionInfo } from "../models/exercise";
import { AntiplagiarismStatusResponse, FavouriteReviewResponse } from "../models/instructor";

export const studentModeToggleAction = (isStudentMode: boolean): StudentModeAction => ({
	type: INSTRUCTOR__STUDENT_MODE_TOGGLE,
	isStudentMode,
});

export const studentLoadStartAction = (studentId: string,): StudentInfoLoadStartAction => ({
	type: INSTRUCTOR__STUDENT_INFO_LOAD_START,
	studentId,
});

export const studentLoadSuccessAction = (
	studentInfo: ShortUserInfo,
): StudentInfoLoadSuccessAction => ({
	type: INSTRUCTOR__STUDENT_INFO_LOAD_SUCCESS,
	studentId: studentInfo.id,
	...studentInfo,
});

export const studentLoadFailAction = (
	studentId: string,
	error: string,
): StudentInfoLoadFailAction => ({
	type: INSTRUCTOR__STUDENT_INFO_LOAD_FAIL,
	studentId,
	error,
});

export const studentSubmissionsLoadStartAction = (
	studentId: string,
	courseId: string,
	slideId: string,
): StudentSubmissionsLoadStartAction => ({
	type: INSTRUCTOR__STUDENT_SUBMISSIONS_LOAD_START,
	studentId,
	courseId,
	slideId,
});

export const studentSubmissionsLoadSuccessAction = (
	studentId: string,
	courseId: string,
	slideId: string,
	submissions: SubmissionInfo[],
): StudentSubmissionsLoadSuccessAction => ({
	type: INSTRUCTOR__STUDENT_SUBMISSIONS_LOAD_SUCCESS,
	studentId,
	courseId,
	slideId,
	submissions,
});

export const studentSubmissionsLoadFailAction = (
	studentId: string,
	courseId: string,
	slideId: string,
	error: string,
): StudentSubmissionsLoadFailAction => ({
	type: INSTRUCTOR__STUDENT_SUBMISSIONS_LOAD_FAIL,
	studentId,
	courseId,
	slideId,
	error,
});

export const antiplagiarimsStatusLoadStartAction = (
	submissionId: string,
): AntiplagiarismStatusLoadStartAction => ({
	type: INSTRUCTOR__ANTIPLAGIARISM_STATUS_LOAD_START,
	submissionId,
});

export const antiplagiarimsStatusLoadSuccessAction = (
	submissionId: string,
	response: AntiplagiarismStatusResponse,
): AntiplagiarismStatusLoadSuccessAction => ({
	type: INSTRUCTOR__ANTIPLAGIARISM_STATUS_LOAD_SUCCESS,
	submissionId,
	...response,
});

export const antiplagiarimsStatusLoadFailAction = (
	submissionId: string,
	error: string,
): AntiplagiarismStatusLoadFailAction => ({
	type: INSTRUCTOR__ANTIPLAGIARISM_STATUS_LOAD_FAIL,
	submissionId,
	error,
});

export const favouriteReviewsLoadStartAction = (
	courseId: string,
	slideId: string,
): FavouriteReviewsLoadStartAction => ({
	type: INSTRUCTOR__FAVOURITE_REVIEWS_LOAD_START,
	courseId,
	slideId,
});

export const favouriteReviewsLoadSuccessAction = (
	courseId: string,
	slideId: string,
	response: FavouriteReviewResponse,
): FavouriteReviewsLoadSuccessAction => ({
	type: INSTRUCTOR__FAVOURITE_REVIEWS_LOAD_SUCCESS,
	courseId,
	slideId,
	...response,
});

export const favouriteReviewsLoadFailAction = (
	courseId: string,
	slideId: string,
	error: string,
): FavouriteReviewsLoadFailAction => ({
	type: INSTRUCTOR__FAVOURITE_REVIEWS_LOAD_FAIL,
	courseId,
	slideId,
	error,
});
