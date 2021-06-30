import { loadFail, loadStart, loadSuccess, } from "src/consts/actions";
import { ShortUserInfo } from "src/models/users";
import { SubmissionInfo } from "src/models/exercise";
import { AntiplagiarismStatusResponse, FavouriteReviewResponse } from "src/models/instructor";

const instructor = 'INSTRUCTOR__';
const student = 'STUDENT_';
const info = 'INFO_';
const submissions = 'SUBMISSIONS_';
const antiplagiarismStatus = "ANTIPLAGIARISM_STATUS_";
const favouriteReviews = "FAVOURITE_REVIEWS_";

export const INSTRUCTOR__STUDENT_MODE_TOGGLE = instructor + student + 'MODE_TOGGLE';

export const INSTRUCTOR__STUDENT_INFO_LOAD_START = instructor + student + info + loadStart;
export const INSTRUCTOR__STUDENT_INFO_LOAD_SUCCESS = instructor + student + info + loadFail;
export const INSTRUCTOR__STUDENT_INFO_LOAD_FAIL = instructor + student + info + loadSuccess;

export const INSTRUCTOR__STUDENT_SUBMISSIONS_LOAD_START = instructor + student + submissions + loadStart;
export const INSTRUCTOR__STUDENT_SUBMISSIONS_LOAD_SUCCESS = instructor + student + submissions + loadSuccess;
export const INSTRUCTOR__STUDENT_SUBMISSIONS_LOAD_FAIL = instructor + student + submissions + loadFail;

export const INSTRUCTOR__ANTIPLAGIARISM_STATUS_LOAD_START = instructor + antiplagiarismStatus + loadStart;
export const INSTRUCTOR__ANTIPLAGIARISM_STATUS_LOAD_SUCCESS = instructor + antiplagiarismStatus + loadSuccess;
export const INSTRUCTOR__ANTIPLAGIARISM_STATUS_LOAD_FAIL = instructor + antiplagiarismStatus + loadFail;

export const INSTRUCTOR__FAVOURITE_REVIEWS_LOAD_START = instructor + favouriteReviews + loadStart;
export const INSTRUCTOR__FAVOURITE_REVIEWS_LOAD_SUCCESS = instructor + favouriteReviews + loadSuccess;
export const INSTRUCTOR__FAVOURITE_REVIEWS_LOAD_FAIL = instructor + favouriteReviews + loadFail;

export interface StudentModeAction {
	type: typeof INSTRUCTOR__STUDENT_MODE_TOGGLE;
	isStudentMode: boolean;
}

export interface StudentInfoLoadStartAction {
	type: typeof INSTRUCTOR__STUDENT_INFO_LOAD_START;
	studentId: string;
}

export interface StudentInfoLoadSuccessAction extends ShortUserInfo {
	type: typeof INSTRUCTOR__STUDENT_INFO_LOAD_SUCCESS;
	studentId: string;
}

export interface StudentInfoLoadFailAction {
	type: typeof INSTRUCTOR__STUDENT_INFO_LOAD_FAIL;
	studentId: string;
	error: string;
}

export interface StudentSubmissionsLoadStartAction {
	type: typeof INSTRUCTOR__STUDENT_SUBMISSIONS_LOAD_START;
	studentId: string;
	courseId: string;
	slideId: string;
}

export interface StudentSubmissionsLoadSuccessAction {
	type: typeof INSTRUCTOR__STUDENT_SUBMISSIONS_LOAD_SUCCESS;
	studentId: string;
	courseId: string;
	slideId: string;
	submissions: SubmissionInfo[];
}

export interface StudentSubmissionsLoadFailAction {
	type: typeof INSTRUCTOR__STUDENT_SUBMISSIONS_LOAD_FAIL;
	studentId: string;
	courseId: string;
	slideId: string;
	error: string;
}

export interface AntiplagiarismStatusLoadStartAction {
	type: typeof INSTRUCTOR__ANTIPLAGIARISM_STATUS_LOAD_START;
	submissionId: string;
}

export interface AntiplagiarismStatusLoadSuccessAction extends AntiplagiarismStatusResponse {
	type: typeof INSTRUCTOR__ANTIPLAGIARISM_STATUS_LOAD_SUCCESS;
	submissionId: string;
}

export interface AntiplagiarismStatusLoadFailAction {
	type: typeof INSTRUCTOR__ANTIPLAGIARISM_STATUS_LOAD_FAIL;
	submissionId: string;
	error: string;
}

export interface FavouriteReviewsLoadStartAction {
	type: typeof INSTRUCTOR__FAVOURITE_REVIEWS_LOAD_START;
	courseId: string;
	slideId: string;
}

export interface FavouriteReviewsLoadSuccessAction extends FavouriteReviewResponse {
	type: typeof INSTRUCTOR__FAVOURITE_REVIEWS_LOAD_SUCCESS;
	courseId: string;
	slideId: string;
}

export interface FavouriteReviewsLoadFailAction {
	type: typeof INSTRUCTOR__FAVOURITE_REVIEWS_LOAD_FAIL;
	courseId: string;
	slideId: string;
	error: string;
}

export type InstructorAction =
	StudentModeAction
	| StudentInfoLoadStartAction
	| StudentInfoLoadSuccessAction
	| StudentInfoLoadFailAction
	| StudentSubmissionsLoadStartAction
	| StudentSubmissionsLoadSuccessAction
	| StudentSubmissionsLoadFailAction
	| AntiplagiarismStatusLoadStartAction
	| AntiplagiarismStatusLoadSuccessAction
	| AntiplagiarismStatusLoadFailAction
	| FavouriteReviewsLoadStartAction
	| FavouriteReviewsLoadSuccessAction
	| FavouriteReviewsLoadFailAction
	;
