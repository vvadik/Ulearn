import {
	INSTRUCTOR__STUDENT_MODE_TOGGLE,
	StudentModeAction,
} from 'src/actions/instructor.types';

export const studentModeToggleAction = (isStudentMode: boolean): StudentModeAction => ({
	type: INSTRUCTOR__STUDENT_MODE_TOGGLE,
	isStudentMode,
});
