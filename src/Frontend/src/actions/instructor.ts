import {
	INSTRUCTOR__STUDENT_MODE_TOGGLE,
} from 'src/consts/actions';

export interface StudentModeAction {
	type: typeof INSTRUCTOR__STUDENT_MODE_TOGGLE,
	isStudentMode: boolean,
}

export const studentModeToggleAction = (isStudentMode: boolean): StudentModeAction => ({
	type: INSTRUCTOR__STUDENT_MODE_TOGGLE,
	isStudentMode,
});
