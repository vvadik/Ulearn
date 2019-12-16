import { connect } from "react-redux";
import { loadCourse } from "../../actions/course";
import { loadUserProgress, userProgressUpdate } from "../../actions/user";
import Course from '../../components/course/Course';
import { withRouter } from "react-router-dom";
import queryString from "query-string";

const mapStateToProps = (state, { match }) => {
	const params = queryString.parse(window.location.search);
	const slideIdInQuery = params.slideId;
	const courseId = match.params.courseId.toLowerCase();
	const slideSlug = match.params.slideSlug;
	const slideId = slideIdInQuery ? params.slideId : slideSlug.split('_').pop();
	const courseInfo = state.courses.fullCoursesInfo[courseId];

	const isLti = slideIdInQuery === "LtiSlide";
	const isReview = params.CheckQueueItemId !== undefined;
	const isNavMenuVisible = !isLti && !isReview;
	return {
		courseId,
		slideId,
		courseInfo,
		isNavMenuVisible,
		units: mapCourseInfoToUnits(courseInfo),
		isAuthenticated: state.account.isAuthenticated,
		progress: state.user.progress[courseId],
		navigationOpened: state.navigation.opened,
	};
};
const mapDispatchToProps = (dispatch) => ({
	loadCourse: (courseId) => dispatch(loadCourse(courseId)),
	loadUserProgress: (courseId) => dispatch(loadUserProgress(courseId)),
	updateVisitedSlide: (courseId, slideId) => dispatch(userProgressUpdate(courseId, slideId)),
});


const connected = connect(mapStateToProps, mapDispatchToProps)(Course);
export default withRouter(connected);


function mapCourseInfoToUnits(courseInfo) {
	if (!courseInfo || !courseInfo.units) {
		return null;
	}
	return courseInfo.units.reduce((acc, item) => {
		acc[item.id] = item;
		return acc;
	}, {});
}
