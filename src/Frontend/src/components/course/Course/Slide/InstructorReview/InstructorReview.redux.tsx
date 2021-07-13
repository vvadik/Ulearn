import { connect } from "react-redux";

import InstructorReview from "./InstructorReview";

import { buildUserInfo } from "src/utils/courseRoles";

import { Dispatch } from "redux";
import { RootState } from "src/redux/reducers";
import {
	getAntiplagiarismStatus,
	getFavouriteReviews,
	getStudentInfo,
	getStudentSubmissions
} from "src/api/instructor";
import { getCourseGroupsRedux } from "src/api/groups";
import { getDataIfLoaded } from "src/redux";
import { SlideContext } from "../Slide";
import { ShortGroupInfo } from "src/models/comments";
import { ReviewInfo } from "src/models/exercise";
import { FavouriteComment } from "./AddCommentForm/AddCommentForm";
import { AntiplagiarismInfo } from "src/models/instructor";
import { ApiFromRedux, PropsFromRedux } from "./InstructorReview.types";

const mapStateToProps = (
	{ instructor, account, groups, }: RootState,
	{ slideContext: { courseId, slideId, }, studentId }: {
		slideContext: SlideContext;
		studentId: string,
	}
): PropsFromRedux => {
	const student = getDataIfLoaded(instructor.studentsById[studentId]);
	const studentSubmissions = getDataIfLoaded(instructor
		.submissionsByCourseId[courseId]
		?.bySlideId[slideId]
		?.byStudentId[studentId]);
	let studentGroups: ShortGroupInfo[] | undefined;
	const reduxGroups = getDataIfLoaded(groups.groupsIdsByUserId[studentId])
		?.map(groupId => getDataIfLoaded(groups.groupById[groupId]));
	if(reduxGroups && reduxGroups.every(g => g !== undefined)) {
		studentGroups = reduxGroups.map(g => ({ ...g, courseId, })) as ShortGroupInfo[];
	}
	return {
		user: buildUserInfo(account, courseId,),
		favouriteReviews: [],
		studentGroups: studentGroups,
		student,
		studentSubmissions,
		antiplagiarismStatus: undefined,
		prohibitFurtherManualChecking: true,
	};
};

const mapDispatchToProps = (dispatch: Dispatch): ApiFromRedux => {
	return {
		addReview(submissionId: number, comment: string, startLine: number, startPosition: number, finishLine: number,
			finishPosition: number
		): Promise<ReviewInfo> {
			return Promise.resolve(undefined as unknown as ReviewInfo);
		},
		addReviewComment(submissionId: number, reviewId: number, comment: string): void {
		},
		deleteReviewOrComment(submissionId: number, reviewId: number, commentId: number | undefined): void {
		},
		getAntiPlagiarismStatus(): Promise<AntiplagiarismInfo> {
			return Promise.resolve(undefined as unknown as AntiplagiarismInfo);
		},
		onAddReview(comment: string): Promise<FavouriteComment> {
			return Promise.resolve(undefined as unknown as FavouriteComment);
		},
		onAddReviewToFavourite(comment: string): Promise<FavouriteComment> {
			return Promise.resolve(undefined as unknown as FavouriteComment);
		},
		onProhibitFurtherReviewToggleChange(value: boolean): void {
		},
		onScoreSubmit(score: number): void {
		},
		onToggleReviewFavourite(commentId: number): void {
		},
		getStudentInfo: (studentId: string,) => getStudentInfo(studentId)(dispatch),
		getStudentSubmissions: (studentId: string, courseId: string, slideId: string,) =>
			getStudentSubmissions(studentId, courseId, slideId)(dispatch),
		getAntiplagiarismStatus: (submissionId: string,) => getAntiplagiarismStatus(submissionId)(dispatch),
		getFavouriteReviews: (courseId: string, slideId: string,) => getFavouriteReviews(courseId, slideId,)(dispatch),
		getStudentGroups: (courseId: string, userId: string,) => getCourseGroupsRedux(courseId, userId)(dispatch)
	};
};

const Connected = connect(mapStateToProps, mapDispatchToProps)(InstructorReview);
export default Connected;
