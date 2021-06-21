import {
	SLIDES_SLIDE_READY,
	SLIDE_LOAD_START,
	SLIDE_LOAD_SUCCESS,
	SLIDE_LOAD_FAIL,
	EXERCISE_DELETE_REVIEW_COMMENT_START,
	EXERCISE_ADD_REVIEW_COMMENT_FAIL,
	EXERCISE_DELETE_REVIEW_COMMENT_SUCCESS,
	EXERCISE_DELETE_REVIEW_COMMENT_FAIL,
	EXERCISE_ADD_SUBMISSION,
	EXERCISE_ADD_REVIEW_COMMENT_SUCCESS,
	SlideAction,
	EXERCISE_ADD_REVIEW_COMMENT_START,
	SlideReadyAction,
	SlideLoadSuccessAction,
	SlideLoadFailAction,
	ExerciseAddSubmissionAction,
	ExerciseAddReviewStartAction,
	ExerciseAddReviewSuccessAction,
	ExerciseAddReviewFailAction,
	ExerciseDeleteReviewStartAction,
	ExerciseDeleteReviewSuccessAction,
	ExerciseDeleteReviewFailAction,
} from 'src/actions/slides.types';
import { Block, BlockTypes, ExerciseBlock } from "src/models/slide";
import { RunSolutionResponse, SubmissionInfo } from "src/models/exercise";
import { ReviewInfoRedux, SubmissionInfoRedux } from "src/models/reduxState";

interface SlidesState {
	isSlideReady: boolean,
	slideLoading: boolean,
	slidesByCourses: { [courseId: string]: { [slideId: string]: Block<BlockTypes>[] } },
	slideError: string | null;

	//exercise
	submissionsByCourses: { [courseId: string]: { [slideId: string]: { [submissionId: number]: SubmissionInfoRedux } } },
	submissionError: string | null,
	lastCheckingResponse: RunSolutionResponse | null,
}

const initialCoursesSlidesState: SlidesState = {
	isSlideReady: false,
	slideLoading: false,
	slideError: null,
	slidesByCourses: {},

	lastCheckingResponse: null,
	submissionsByCourses: {},
	submissionError: null,
};

export default function slides(state = initialCoursesSlidesState, action: SlideAction): SlidesState {
	switch (action.type) {
		case SLIDES_SLIDE_READY: {
			const { isSlideReady } = action as SlideReadyAction;
			return {
				...state,
				isSlideReady,
			};
		}
		case SLIDE_LOAD_START: {
			return {
				...state,
				slideLoading: true,
				slideError: null,
			};
		}
		case SLIDE_LOAD_SUCCESS: {
			const { courseId, slideId, slideBlocks } = action as SlideLoadSuccessAction;
			const { slidesByCourses } = state;

			const newState = {
				...state,
				slideLoading: false,
				slideError: null,
			};

			const exerciseBlock = slideBlocks.find(block => block.$type === BlockTypes.exercise) as ExerciseBlock;

			if(exerciseBlock && exerciseBlock.submissions) {
				const exerciseSubmissions: { [submissionId: string]: SubmissionInfo } = {};


				for (const submission of exerciseBlock.submissions) {
					exerciseSubmissions[submission.id] = submission;
				}

				exerciseBlock.submissions = undefined;

				newState.submissionsByCourses = {
					...newState.submissionsByCourses,
					[courseId]: {
						...newState.submissionsByCourses[courseId],
						[slideId]: exerciseSubmissions,
					}
				};
			}

			newState.slidesByCourses = {
				...slidesByCourses,
				[courseId]: {
					...slidesByCourses[courseId],
					[slideId]: slideBlocks,
				}
			};

			return newState;
		}
		case SLIDE_LOAD_FAIL: {
			const { error } = action as SlideLoadFailAction;
			return {
				...state,
				slideLoading: false,
				slideError: error,
			};
		}
		case EXERCISE_ADD_SUBMISSION: {
			const { courseId, slideId, result } = action as ExerciseAddSubmissionAction;
			const { submission } = result;

			let newState = {
				...state,
				lastCheckingResponse: { slideId, courseId, ...result },
			};
			if(submission) {
				newState = {
					...newState,
					submissionsByCourses: {
						...newState.submissionsByCourses,
						[courseId]: {
							...newState.submissionsByCourses[courseId],
						}
					},
				};
				submission.manualCheckingReviews;
				newState.submissionsByCourses[courseId][slideId] = {
					...newState.submissionsByCourses[courseId][slideId],
					[submission.id]: submission,
				};
			}

			return newState;
		}
		case EXERCISE_ADD_REVIEW_COMMENT_START: {
			const { courseId, slideId, submissionId, reviewId, } = action as ExerciseAddReviewStartAction;

			const submissions = state.submissionsByCourses[courseId][slideId];
			const submission = submissions[submissionId];
			const { manualCheckingReviews } = submission;

			const newReviews = JSON.parse(JSON.stringify(manualCheckingReviews)) as ReviewInfoRedux[];
			const review = newReviews.find(r => r.id === reviewId);
			if(review) {
				// eslint-disable-next-line @typescript-eslint/ban-ts-comment
				// @ts-ignore
				review.comments.push({ isLoading: true, });
			}

			return {
				...state,
				submissionError: null,
				submissionsByCourses: {
					...state.submissionsByCourses,
					[courseId]: {
						...state.submissionsByCourses[courseId],
						[slideId]: {
							...submissions,
							[submissionId]: {
								...submission,
								manualCheckingReviews: newReviews,
							}
						},
					}
				},
			};
		}
		case EXERCISE_ADD_REVIEW_COMMENT_SUCCESS: {
			const { courseId, slideId, submissionId, reviewId, comment, } = action as ExerciseAddReviewSuccessAction;

			const submissions = state.submissionsByCourses[courseId][slideId];
			const submission = submissions[submissionId];
			const { manualCheckingReviews } = submission;
			const newReviews = JSON.parse(JSON.stringify(manualCheckingReviews)) as ReviewInfoRedux[];
			const review = newReviews.find(r => r.id === reviewId);
			if(review) {
				review.comments[review.comments.length - 1] = comment;
			}

			return {
				...state,
				submissionError: null,
				submissionsByCourses: {
					...state.submissionsByCourses,
					[courseId]: {
						...state.submissionsByCourses[courseId],
						[slideId]: {
							...submissions,
							[submissionId]: {
								...submission,
								manualCheckingReviews: newReviews,
							}
						},
					}
				},
			};
		}
		case EXERCISE_ADD_REVIEW_COMMENT_FAIL: {
			const { courseId, slideId, submissionId, reviewId, error, } = action as ExerciseAddReviewFailAction;

			const submissions = state.submissionsByCourses[courseId][slideId];
			const submission = submissions[submissionId];
			const { manualCheckingReviews } = submission;

			const newReviews = JSON.parse(JSON.stringify(manualCheckingReviews)) as ReviewInfoRedux[];
			const review = newReviews.find(r => r.id === reviewId);
			if(review) {
				review.comments.pop();
			}

			return {
				...state,
				submissionError: error,
				submissionsByCourses: {
					...state.submissionsByCourses,
					[courseId]: {
						...state.submissionsByCourses[courseId],
						[slideId]: {
							...submissions,
							[submissionId]: {
								...submission,
								manualCheckingReviews: newReviews,
							}
						},
					}
				},
			};
		}
		case EXERCISE_DELETE_REVIEW_COMMENT_START: {
			const { courseId, slideId, submissionId, reviewId, commentId, } = action as ExerciseDeleteReviewStartAction;

			const submissions = state.submissionsByCourses[courseId][slideId];
			const submission = submissions[submissionId];
			const { manualCheckingReviews, } = submission;

			const newReviews = JSON.parse(JSON.stringify(manualCheckingReviews)) as ReviewInfoRedux[];
			const review = newReviews.find(r => r.id === reviewId);
			if(review) {
				const comment = review.comments.find(c => c.id === commentId);
				if(comment) {
					comment.isDeleted = true;
				}
			}

			return {
				...state,
				submissionError: null,
				submissionsByCourses: {
					...state.submissionsByCourses,
					[courseId]: {
						...state.submissionsByCourses[courseId],
						[slideId]: {
							...submissions,
							[submissionId]: {
								...submission,
								manualCheckingReviews: newReviews,
							}
						},
					}
				},
			};
		}
		case EXERCISE_DELETE_REVIEW_COMMENT_SUCCESS: {
			const {
				courseId,
				slideId,
				submissionId,
				reviewId,
				commentId,
			} = action as ExerciseDeleteReviewSuccessAction;

			const submissions = state.submissionsByCourses[courseId][slideId];
			const submission = submissions[submissionId];
			const { manualCheckingReviews, } = submission;

			const newReviews = JSON.parse(JSON.stringify(manualCheckingReviews)) as ReviewInfoRedux[];
			const review = newReviews.find(r => r.id === reviewId);
			if(review) {
				review.comments = review.comments.filter(c => c.id !== commentId);
			}

			return {
				...state,
				submissionError: null,
				submissionsByCourses: {
					...state.submissionsByCourses,
					[courseId]: {
						...state.submissionsByCourses[courseId],
						[slideId]: {
							...submissions,
							[submissionId]: {
								...submission,
								manualCheckingReviews: newReviews,
							}
						},
					}
				},
			};
		}
		case EXERCISE_DELETE_REVIEW_COMMENT_FAIL: {
			const {
				courseId,
				slideId,
				submissionId,
				reviewId,
				commentId,
				error,
			} = action as ExerciseDeleteReviewFailAction;

			const submissions = state.submissionsByCourses[courseId][slideId];
			const submission = submissions[submissionId];
			const { manualCheckingReviews, } = submission;

			const newReviews = JSON.parse(JSON.stringify(manualCheckingReviews)) as ReviewInfoRedux[];
			const review = newReviews.find(r => r.id === reviewId);
			if(review) {
				const comment = review.comments.find(c => c.id === commentId);
				if(comment) {
					comment.isDeleted = false;
				}
			}

			return {
				...state,
				submissionError: error,
				submissionsByCourses: {
					...state.submissionsByCourses,
					[courseId]: {
						...state.submissionsByCourses[courseId],
						[slideId]: {
							...submissions,
							[submissionId]: {
								...submission,
								manualCheckingReviews: newReviews,
							}
						},
					}
				},
			};
		}
		default:
			return state;
	}
}
