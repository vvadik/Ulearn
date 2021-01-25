import { SlideUserProgress } from "./userProgress";
import { CourseInfo } from "./course";
import {
	ExerciseAutomaticCheckingResponse,
	ReviewCommentResponse,
	ReviewInfo,
	RunSolutionResponse,
	SubmissionInfo
} from "./exercise";
import { CourseAccessType, CourseRoleType, SystemAccessType } from "../consts/accessType";
import { AccountProblemType } from "../consts/accountProblemType";
import BlockTypes from "../components/course/Course/Slide/blockTypes";
import { Block } from "./slide";
import { Flashcard, UnitFlashcardsInfo } from "./flashcards";

interface RootState {
	userProgress: UserProgressState;
	courses: CourseState;
	slides: SlidesState;
	account: AccountState;
}

interface UserProgressState {
	loading: boolean;
	progress: { [courseId: string]: { [slideId: string]: SlideUserProgress } };
}

interface CourseState {
	fullCoursesInfo: { [courseId: string]: CourseInfo }

	flashcardsByCourses: { [courseId: string]: { [flashcardId: string]: Flashcard } }
	flashcardsByUnits: { [unitId: string]: UnitFlashcardsInfo };
	flashcardsLoading: boolean,
	// TODO не все поля
}

interface SlidesState {
	submissionsByCourses: { [courseId: string]: { [slideId: string]: { [submissionId: number]: SubmissionInfoRedux } } },
	submissionError: string,
	lastCheckingResponse: RunSolutionResponse,
	slidesByCourses: { [courseId: string]: { [slideId: string]: Block<BlockTypes>[] } },
	slideLoading: boolean,
	// TODO не все поля
}

interface AccountState {
	accountLoaded: boolean;
	isAuthenticated: boolean;
	isSystemAdministrator: boolean;
	accountProblems: [{ title: string, description: string, problemType: AccountProblemType }];
	systemAccesses: [SystemAccessType];
	roleByCourse: { [courseId: string]: CourseRoleType };
	accessesByCourse: { [courseId: string]: CourseAccessType };
	groupsAsStudent: [GroupAsStudentInfo];
	gender: string | null;
	isHijacked: boolean;
	id: string | null;
	login: string | null;
	firstName: string | null;
	lastName: string | null;
	visibleName: string | null;
	avatarUrl: string | null;
}

interface ReviewCommentResponseRedux extends ReviewCommentResponse {
	isDeleted: boolean,
	isLoading: boolean,
}

interface ReviewInfoRedux extends ReviewInfo {
	comments: ReviewCommentResponseRedux[];
}

interface ExerciseAutomaticCheckingResponseRedux extends ExerciseAutomaticCheckingResponse {
	reviews: ReviewInfoRedux[] | null;
}

interface SubmissionInfoRedux extends SubmissionInfo {
	automaticChecking: ExerciseAutomaticCheckingResponseRedux | null; // null если задача не имеет автоматических тестов, это не отменяет возможности ревью.
	manualCheckingReviews: ReviewInfoRedux[];
}

export { RootState, UserProgressState, CourseState, AccountState, SubmissionInfoRedux, ReviewInfoRedux, };
