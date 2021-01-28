import {
	INSTRUCTOR__STUDENT_MODE_TOGGLE,
} from 'src/consts/actions';
import { StudentModeAction } from "src/actions/instructor";

export interface InstructorState {
	isStudentMode: boolean,
}

const initialInstructorState = {
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
