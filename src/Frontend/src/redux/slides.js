import {
	COURSES__SLIDE_LOAD,
	COURSES__EXERCISE_ADD_SUBMISSIONS,
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
		case COURSES__EXERCISE_ADD_SUBMISSIONS: {
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
		default:
			return state;
	}
}
