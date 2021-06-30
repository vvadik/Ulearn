import {
	INSTRUCTOR__STUDENT_MODE_TOGGLE,
	INSTRUCTOR__STUDENT_INFO_LOAD_START,
	INSTRUCTOR__STUDENT_INFO_LOAD_SUCCESS,
	INSTRUCTOR__STUDENT_INFO_LOAD_FAIL,
	InstructorAction,
	StudentModeAction,
	StudentInfoLoadSuccessAction,
	StudentInfoLoadFailAction,
	StudentInfoLoadStartAction,
} from 'src/actions/instructor.types';
import { SubmissionInfo } from "src/models/exercise";
import { ShortUserInfo } from "src/models/users";
import { FavouriteReview } from "src/models/instructor";
import { ReduxData } from "./index";

export interface InstructorState {
	isStudentMode: boolean;

	studentsById: {
		[studentId: string]: ShortUserInfo | ReduxData;
	}

	submissionsByCourseId: {
		[courseId: string]: {
			bySlideId: {
				[slideId: string]: {
					byStudentId: {
						[studentId: string]: SubmissionInfo[] | ReduxData;
					}
				} | undefined;
			}
		} | undefined;
	};

	favouritesReviewsByCourseId: {
		[courseId: string]: {
			bySlideId: {
				[slideId: string]: FavouriteReview[] | ReduxData;
			};
		} | undefined;
	};
}

const initialInstructorState: InstructorState = {
	isStudentMode: false,
	studentsById: {},
	submissionsByCourseId: {},
	favouritesReviewsByCourseId: {},
};

export default function instructor(state = initialInstructorState, action: InstructorAction): InstructorState {
	switch (action.type) {
		case INSTRUCTOR__STUDENT_MODE_TOGGLE: {
			const { isStudentMode } = action as StudentModeAction;

			return {
				...state,
				isStudentMode,
			};
		}
		case INSTRUCTOR__STUDENT_INFO_LOAD_START: {
			const { studentId, } = action as StudentInfoLoadStartAction;

			return {
				...state,
				studentsById: {
					...state.studentsById,
					[studentId]: { isLoading: true },
				}
			};
		}
		case INSTRUCTOR__STUDENT_INFO_LOAD_SUCCESS: {
			const { studentId, ...studentInfo } = action as StudentInfoLoadSuccessAction;

			return {
				...state,
				studentsById: {
					...state.studentsById,
					[studentId]: studentInfo,
				}
			};
		}
		case INSTRUCTOR__STUDENT_INFO_LOAD_FAIL: {
			const { studentId, error, } = action as StudentInfoLoadFailAction;

			return {
				...state,
				studentsById: {
					...state.studentsById,
					[studentId]: { error, },
				}
			};
		}
		default:
			return state;
	}
}
