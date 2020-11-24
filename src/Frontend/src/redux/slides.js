import {
	COURSES__SLIDE_LOAD,
	COURSES__EXERCISE_ADD_SUBMISSION,
	COURSES__EXERCISE_ADD_REVIEW_COMMENT,
	COURSES__EXERCISE_DELETE_REVIEW_COMMENT,
	START, SUCCESS, FAIL,
} from '../consts/actions';
import blockTypes from "src/components/course/Course/Slide/blockTypes";

const initialCoursesSlidesState = {
	slidesByCourses: {},
	lastCheckingResponse: null,
	submissionsByCourses: {},
	submissionError: null,
	slideLoading: false,
	slideError: null,
};

export default function slides(state = initialCoursesSlidesState, action) {
	switch (action.type) {
		case COURSES__SLIDE_LOAD + START:
			return {
				...state,
				slideLoading: true,
				slideError: null,
			};
		case COURSES__SLIDE_LOAD + SUCCESS: {
			const { courseId, slideId, slideBlocks } = action;
			const { slidesByCourses } = state;

			const newState = {
				...state,
				slideLoading: false,
				slideError: null,
			};

			const exerciseBlock = slideBlocks.find(block => block.$type === blockTypes.exercise);

			if(exerciseBlock) {
				const exerciseSubmissions = {};

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
			}

			return newState;
		}
		case COURSES__SLIDE_LOAD + FAIL: {
			return {
				...state,
				slideLoading: false,
				slideError: action.error,
			};
		}
		case COURSES__EXERCISE_ADD_SUBMISSION: {
			const { courseId, slideId, result } = action;
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
				newState.submissionsByCourses[courseId][slideId] = {
					...newState.submissionsByCourses[courseId][slideId],
					[submission.id]: submission,
				};
			}

			return newState;
		}
		case COURSES__EXERCISE_ADD_REVIEW_COMMENT + START: {
			const { courseId, slideId, submissionId, reviewId, } = action;

			const submissions = state.submissionsByCourses[courseId][slideId];
			const submission = submissions[submissionId];
			const { manualCheckingReviews } = submission;

			const newReviews = JSON.parse(JSON.stringify(manualCheckingReviews));
			const review = newReviews.find(r => r.id === reviewId);
			review.comments.push({ isLoading: true });

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
			}
		}
		case COURSES__EXERCISE_ADD_REVIEW_COMMENT + SUCCESS: {
			const { courseId, slideId, submissionId, reviewId, comment, } = action;

			const submissions = state.submissionsByCourses[courseId][slideId];
			const submission = submissions[submissionId];
			const { manualCheckingReviews } = submission;
			const newReviews = JSON.parse(JSON.stringify(manualCheckingReviews));
			const review = newReviews.find(r => r.id === reviewId);
			review.comments[review.comments.length - 1] = comment;

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
			}
		}
		case COURSES__EXERCISE_ADD_REVIEW_COMMENT + FAIL: {
			const { courseId, slideId, submissionId, reviewId, error, } = action;

			const submissions = state.submissionsByCourses[courseId][slideId];
			const submission = submissions[submissionId];
			const { manualCheckingReviews } = submission;

			const newReviews = JSON.parse(JSON.stringify(manualCheckingReviews));
			const review = newReviews.find(r => r.id === reviewId);
			review.comments.pop();

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
			}
		}
		case COURSES__EXERCISE_DELETE_REVIEW_COMMENT + START: {
			const { courseId, slideId, submissionId, reviewId, commentId, } = action;

			const submissions = state.submissionsByCourses[courseId][slideId];
			const submission = submissions[submissionId];
			const { manualCheckingReviews, } = submission;

			const newReviews = JSON.parse(JSON.stringify(manualCheckingReviews));
			const review = newReviews.find(r => r.id === reviewId);
			review.comments.find(c => c.id === commentId).isDeleted = true;

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
			}
		}
		case COURSES__EXERCISE_DELETE_REVIEW_COMMENT + SUCCESS: {
			const { courseId, slideId, submissionId, reviewId, commentId, } = action;

			const submissions = state.submissionsByCourses[courseId][slideId];
			const submission = submissions[submissionId];
			const { manualCheckingReviews, } = submission;

			const newReviews = JSON.parse(JSON.stringify(manualCheckingReviews));
			const review = newReviews.find(r => r.id === reviewId);
			review.comments = review.comments.filter(c => c.id !== commentId);

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
			}
		}
		case COURSES__EXERCISE_DELETE_REVIEW_COMMENT + FAIL: {
			const { courseId, slideId, submissionId, reviewId, commentId, error, } = action;

			const submissions = state.submissionsByCourses[courseId][slideId];
			const submission = submissions[submissionId];
			const { manualCheckingReviews, } = submission;

			const newReviews = JSON.parse(JSON.stringify(manualCheckingReviews));
			const review = newReviews.find(r => r.id === reviewId);
			review.comments.find(c => c.id === commentId).isDeleted = false;

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
			}
		}
		default:
			return state;
	}
}
