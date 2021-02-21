import {
	INSTRUCTOR__STUDENT_MODE_TOGGLE,
	StudentModeAction,
} from 'src/actions/instructor.types';

export interface InstructorState {
	isStudentMode: boolean,
}

const initialInstructorState: InstructorState = {
	isStudentMode: false,
};

export default function instructor(state = initialInstructorState, action: StudentModeAction): InstructorState {
	switch (action.type) {
		case INSTRUCTOR__STUDENT_MODE_TOGGLE:
			return {
				...state,
				isStudentMode: action.isStudentMode,
			};
		default:
			return state;
	}
}
