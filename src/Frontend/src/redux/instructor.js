import {
	INSTRUCTOR__STUDENT_MODE_TOGGLE,
} from '../consts/actions';

const initialInstructorState = {
	isStudentMode: false,
};

export default function instructor(state = initialInstructorState, action) {
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
