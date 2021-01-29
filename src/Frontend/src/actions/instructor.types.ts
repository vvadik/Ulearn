export const INSTRUCTOR__STUDENT_MODE_TOGGLE = 'INSTRUCTOR__STUDENT_MODE_TOGGLE';

export interface StudentModeAction {
	type: typeof INSTRUCTOR__STUDENT_MODE_TOGGLE,
	isStudentMode: boolean,
}
