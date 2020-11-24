import { SlideUserProgress } from "src/models/userProgress";
import { CourseInfo } from "src/models/course";

interface RootState {
	userProgress: UserProgressState;
	courses: CourseState;
}

interface UserProgressState {
	loading: boolean;
	progress: { [courseId: string]: { [slideId: string]: SlideUserProgress } };
}

interface CourseState {
	fullCoursesInfo: { [courseId: string]: CourseInfo }
	// TODO не все поля
}

export { RootState, UserProgressState, CourseState }
