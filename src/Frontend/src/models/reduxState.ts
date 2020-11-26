import { SlideUserProgress } from "./userProgress";
import { CourseInfo } from "./course";
import { SubmissionInfo } from "./exercise";

interface RootState {
	userProgress: UserProgressState;
	courses: CourseState;
	slides: SlidesState;
}

interface UserProgressState {
	loading: boolean;
	progress: { [courseId: string]: { [slideId: string]: SlideUserProgress } };
}

interface CourseState {
	fullCoursesInfo: { [courseId: string]: CourseInfo }
	// TODO не все поля
}

interface SlidesState {
	submissionsByCourses: { [courseId: string]: { [slideId: string]: { [submissionId: number]: SubmissionInfo } } }
	// TODO не все поля
}

export { RootState, UserProgressState, CourseState };
