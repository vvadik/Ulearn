import { connect } from "react-redux";
import { Dispatch } from "redux";
import { RouteComponentProps, withRouter } from "react-router-dom";

import Course from 'src/components/course/Course';

import { loadCourse, loadCourseErrors, changeCurrentCourseAction } from "src/actions/course";
import { loadUserProgress, userProgressUpdate } from "src/actions/userProgress";

import getSlideInfo from "src/utils/getSlideInfo";

import { RootState } from "src/redux/reducers";
import { MatchParams } from "src/models/router";
import { CourseInfo, UnitInfo } from "src/models/course";

const mapStateToProps = (state: RootState, { match, location, }: RouteComponentProps<MatchParams>) => {
	const slideInfo = getSlideInfo(location);
	const courseId = match.params.courseId.toLowerCase();

	const courseInfo = state.courses.fullCoursesInfo[courseId];

	const loadedCourseIds: { [courseId: string]: boolean } = {};
	for (const courseId of Object.keys(state.courses.fullCoursesInfo)) {
		loadedCourseIds[courseId] = true;
	}
	const isNavigationVisible = slideInfo && !slideInfo.isLti && !slideInfo.isReview && (courseInfo == null || courseInfo.tempCourseError == null);
	const pageInfo = {
		isNavigationVisible: isNavigationVisible || false,
		isReview: slideInfo?.isReview || false,
		isLti: slideInfo?.isLti || false,
		isAcceptedSolutions: slideInfo?.isAcceptedSolutions || false,
		isAcceptedAlert: slideInfo?.isAcceptedAlert || false,
	};

	return {
		courseId,
		slideId: slideInfo?.slideId,
		courseInfo,
		loadedCourseIds,
		pageInfo: pageInfo,
		isSlideReady: state.slides.isSlideReady,
		units: mapCourseInfoToUnits(courseInfo),
		user: state.account,
		progress: state.userProgress.progress[courseId],
		navigationOpened: state.navigation.opened,
		courseLoadingErrorStatus: state.courses.courseLoadingErrorStatus,
		isHijacked: state.account.isHijacked,
		isStudentMode: state.instructor.isStudentMode,
	};
};
const mapDispatchToProps = (dispatch: Dispatch) => ({
	enterToCourse: (courseId: string) => dispatch(changeCurrentCourseAction(courseId)),
	loadCourse: (courseId: string) => loadCourse(courseId)(dispatch),
	loadCourseErrors: (courseId: string) => loadCourseErrors(courseId)(dispatch),
	loadUserProgress: (courseId: string, userId: string) => loadUserProgress(courseId, userId)(dispatch),
	updateVisitedSlide: (courseId: string, slideId: string) => userProgressUpdate(courseId, slideId)(dispatch),
});


const connected = connect(mapStateToProps, mapDispatchToProps)(Course);
export default withRouter(connected);


function mapCourseInfoToUnits(courseInfo: CourseInfo) {
	if(!courseInfo || !courseInfo.units) {
		return null;
	}
	return courseInfo.units.reduce((acc: { [unitId: string]: UnitInfo }, item) => {
		acc[item.id] = item;
		return acc;
	}, {});
}
