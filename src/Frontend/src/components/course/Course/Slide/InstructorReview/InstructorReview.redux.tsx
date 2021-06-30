import { connect } from "react-redux";

import InstructorReview, { ApiFromRedux, PropsFromRedux } from "./InstructorReview";

import { buildUserInfo } from "src/utils/courseRoles";

import { Dispatch } from "redux";
import { MatchParams } from "src/models/router";
import { RootState } from "src/redux/reducers";
import {
	getAntiplagiarismStatus,
	getFavouriteReviews,
	getStudentInfo,
	getStudentSubmissions
} from "src/api/instructor";
import { getCourseGroupsRedux } from "src/api/groups";
import { getDataIfLoaded } from "src/redux";

const mapStateToProps = ({ instructor, account, groups, }: RootState,
	{ courseId, userId, slideId, }: MatchParams & { userId: string; }
): PropsFromRedux => {
	const student = getDataIfLoaded(instructor.studentsById[userId]);
	const studentSubmissions = getDataIfLoaded(instructor
		.submissionsByCourseId[courseId]
		?.bySlideId[slideId]
		?.byStudentId[userId]);
	const courseGroups = getDataIfLoaded(groups.groupsByCourseId[courseId])?.byGroupId;
	const studentGroups = courseGroups ?
		getDataIfLoaded(groups.groupsIdsByUserId[userId])
			?.map(groupId => courseGroups[groupId])
		: undefined;

	return {
		user: buildUserInfo(account, courseId,),

		comments: [],
		groups: studentGroups,
		student,
		studentSubmissions,
	};
};

const mapDispatchToProps = (dispatch: Dispatch): ApiFromRedux => {
	return {
		getStudentInfo: (studentId: string,) => getStudentInfo(studentId)(dispatch),
		getStudentSubmissions: (studentId: string, courseId: string, slideId: string,) =>
			getStudentSubmissions(studentId, courseId, slideId)(dispatch),
		getAntiplagiarismStatus: (submissionId: string,) => getAntiplagiarismStatus(submissionId)(dispatch),
		getFavouriteReviews: (courseId: string, slideId: string,) => getFavouriteReviews(courseId, slideId,)(dispatch),
		getCourseGroups: (courseId: string, userId: string,) => getCourseGroupsRedux(courseId, userId)(dispatch),
	};
};

const Connected = connect(mapStateToProps, mapDispatchToProps)(InstructorReview);
export default Connected;
