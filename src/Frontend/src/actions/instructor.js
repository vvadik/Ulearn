import {
	INSTRUCTOR__STUDENT_MODE_TOGGLE,
} from 'src/consts/actions';

const studentModeToggleAction = (isStudentMode) => ({
	type: INSTRUCTOR__STUDENT_MODE_TOGGLE,
	isStudentMode,
});

export const setStudentMode = (isStudentMode) => {
	return (dispatch) => {
		dispatch(studentModeToggleAction(isStudentMode));
	}
}
