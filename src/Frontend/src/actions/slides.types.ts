import { FAIL, START, SUCCESS, } from "src/consts/actions";
import { Block, BlockTypes } from "src/models/slide";
import { ReviewCommentResponse, RunSolutionResponse, } from "src/models/exercise";

const SLIDES = "SLIDES__SLIDE";
const SLIDE_LOAD = SLIDES + "_LOAD";
export const SLIDE_LOAD_START = SLIDE_LOAD + START;
export const SLIDE_LOAD_SUCCESS = SLIDE_LOAD + SUCCESS;
export const SLIDE_LOAD_FAIL = SLIDE_LOAD + FAIL;

export const SLIDES_SLIDE_READY = SLIDES + "_READY";

export interface SlideReadyAction {
	type: typeof SLIDES_SLIDE_READY,
	isSlideReady: boolean,
}

export interface SlideLoadSuccessAction {
	type: typeof SLIDE_LOAD_SUCCESS,
	courseId: string,
	slideId: string,
	slideBlocks: Block<BlockTypes>[],
}

export interface SlideLoadFailAction {
	type: typeof SLIDE_LOAD_FAIL,
	error: string,
}

export interface SlideLoadStartAction {
	type: typeof SLIDE_LOAD_START,
}

//Exercise
const EXERCISE = "EXERCISE";
export const EXERCISE_ADD_SUBMISSION = EXERCISE + "__ADD_SUBMISSION";

const EXERCISE_ADD_REVIEW_COMMENT = EXERCISE + "__ADD_REVIEW_COMMENT";
export const EXERCISE_ADD_REVIEW_COMMENT_START = EXERCISE_ADD_REVIEW_COMMENT + START;
export const EXERCISE_ADD_REVIEW_COMMENT_SUCCESS = EXERCISE_ADD_REVIEW_COMMENT + SUCCESS;
export const EXERCISE_ADD_REVIEW_COMMENT_FAIL = EXERCISE_ADD_REVIEW_COMMENT + FAIL;

const EXERCISE_DELETE_REVIEW_COMMENT = EXERCISE + "__DELETE_REVIEW_COMMENT";
export const EXERCISE_DELETE_REVIEW_COMMENT_START = EXERCISE_DELETE_REVIEW_COMMENT + START;
export const EXERCISE_DELETE_REVIEW_COMMENT_SUCCESS = EXERCISE_DELETE_REVIEW_COMMENT + SUCCESS;
export const EXERCISE_DELETE_REVIEW_COMMENT_FAIL = EXERCISE_DELETE_REVIEW_COMMENT + FAIL;

export interface ExerciseAddSubmissionAction {
	type: typeof EXERCISE_ADD_SUBMISSION,
	courseId: string,
	slideId: string,
	result: RunSolutionResponse,
}

export interface ExerciseAddReviewStartAction {
	type: typeof EXERCISE_ADD_REVIEW_COMMENT_START,
	courseId: string,
	slideId: string,
	submissionId: number,
	reviewId: number,
}

export interface ExerciseAddReviewSuccessAction {
	type: typeof EXERCISE_ADD_REVIEW_COMMENT_SUCCESS,
	courseId: string,
	slideId: string,
	submissionId: number,
	reviewId: number,
	comment: ReviewCommentResponse,
}

export interface ExerciseAddReviewFailAction {
	type: typeof EXERCISE_ADD_REVIEW_COMMENT_FAIL,
	courseId: string,
	slideId: string,
	submissionId: number,
	reviewId: number,
	error: string
}

export interface ExerciseDeleteReviewStartAction {
	type: typeof EXERCISE_DELETE_REVIEW_COMMENT_START,
	courseId: string,
	slideId: string,
	submissionId: number,
	reviewId: number,
	commentId: number,
}

export interface ExerciseDeleteReviewSuccessAction {
	type: typeof EXERCISE_DELETE_REVIEW_COMMENT_SUCCESS,
	courseId: string,
	slideId: string,
	submissionId: number,
	reviewId: number,
	commentId: number,
}

export interface ExerciseDeleteReviewFailAction {
	type: typeof EXERCISE_DELETE_REVIEW_COMMENT_FAIL,
	courseId: string,
	slideId: string,
	submissionId: number,
	reviewId: number,
	commentId: number,
	error: string,
}

export type ExerciseAction =
	ExerciseAddSubmissionAction
	| ExerciseAddReviewStartAction
	| ExerciseAddReviewSuccessAction
	| ExerciseAddReviewFailAction
	| ExerciseDeleteReviewStartAction
	| ExerciseDeleteReviewSuccessAction
	| ExerciseDeleteReviewFailAction;

export type SlideAction =
	SlideReadyAction
	| SlideLoadSuccessAction
	| SlideLoadFailAction
	| SlideLoadStartAction
	| ExerciseAction;

