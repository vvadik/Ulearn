import { connect } from "react-redux";
import { loadCourse, loadCourseErrors, changeCurrentCourseAction } from "src/actions/course";
import { loadUserProgress, userProgressUpdate } from "src/actions/userProgress";
import Course from '../../components/course/Course';
import { withRouter } from "react-router-dom";
import getSlideInfo from "src/utils/getSlideInfo";

const mapStateToProps = (state, { match, location, }) => {
	const {
		slideId,
		isReview,
		isLti,
		isAcceptedAlert,
		isAcceptedSolutions,
	} = getSlideInfo(location);
	const courseId = match.params.courseId.toLowerCase();

	const courseInfo = state.courses.fullCoursesInfo[courseId];

	const loadedCourseIds = {}
	for (const courseId of Object.keys(state.courses.fullCoursesInfo)) {
		loadedCourseIds[courseId] = true;
	}

	const isNavigationVisible = !isLti && !isReview && (courseInfo == null || courseInfo.tempCourseError == null);

	return {
		courseId,
		slideId,
		courseInfo,
		loadedCourseIds,
		pageInfo: { isNavigationVisible, isReview, isLti, isAcceptedSolutions, isAcceptedAlert, },
		isSlideReady: state.courses.isSlideReady,
		units: mapCourseInfoToUnits(courseInfo),
		user: state.account,
		progress: state.userProgress.progress[courseId],
		navigationOpened: state.navigation.opened,
		courseLoadingErrorStatus: state.courses.courseLoadingErrorStatus,
		isHijacked: state.userProgress.isHijacked,
		isStudentMode: state.instructor.isStudentMode,
	};
};
const mapDispatchToProps = (dispatch) => ({
	enterToCourse: (courseId) => dispatch(changeCurrentCourseAction(courseId)),
	loadCourse: (courseId) => dispatch(loadCourse(courseId)),
	loadCourseErrors: (courseId) => dispatch(loadCourseErrors(courseId)),
	loadUserProgress: (courseId, userId) => dispatch(loadUserProgress(courseId, userId)),
	updateVisitedSlide: (courseId, slideId) => dispatch(userProgressUpdate(courseId, slideId)),
});


const connected = connect(mapStateToProps, mapDispatchToProps)(Course);
export default withRouter(connected);


function mapCourseInfoToUnits(courseInfo) {
	if(!courseInfo || !courseInfo.units) {
		return null;
	}
	return courseInfo.units.reduce((acc, item) => {
		acc[item.id] = item;
		return acc;
	}, {});
}
