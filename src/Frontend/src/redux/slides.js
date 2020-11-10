import {
	COURSES__SLIDE_LOAD,
	COURSES__EXERCISE_ADD_SUBMISSION,
	COURSES__EXERCISE_ADD_REVIEW_COMMENT,
	START, SUCCESS, FAIL,
} from '../consts/actions';
import blockTypes from "src/components/course/Course/Slide/blockTypes";

const initialCoursesSlidesState = {
	slidesByCourses: {},
	lastSubmission: null,
	submissionsByCourses: {},
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
			const { courseId, slideId, result } = action;
			const { slidesByCourses } = state;

			const newState = {
				...state,
				slideLoading: false,
				slideError: null,
			};

			const exerciseBlock = result.find(block => block.$type === blockTypes.exercise);

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
					[slideId]: result,
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

			const newState = {
				...state,
				lastSubmission: { slideId, courseId, ...result },
				submissionsByCourses: {
					...state.submissionsByCourses,
					[courseId]: {
						...state.submissionsByCourses[courseId],
					}
				},
			};

			if(submission) {
				newState.submissionsByCourses[courseId][slideId] = {
					...state.submissionsByCourses[courseId][slideId],
					[submission.id]: submission,
				};
			}

			return newState;
		}
		case COURSES__EXERCISE_ADD_REVIEW_COMMENT + START: {
			const { courseId, slideId, submissionId, reviewId, comment, } = action;

			const submissions = state.submissionsByCourses[courseId][slideId];
			const submission = submissions[submissionId];
			const { manualCheckingReviews } = submission;

			const newReviews = JSON.parse(JSON.stringify(manualCheckingReviews));
			const review = newReviews.find(r => r.id === reviewId);
			review.comments.push(comment);

			return {
				...state,
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
			review.comments.pop();
			review.comments.push(comment);

			return {
				...state,
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
