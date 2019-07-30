import { connect } from "react-redux";
import { loadCourse } from "../../actions/course";
import { loadUserProgress, userProgressUpdate } from "../../actions/user";

import Course from '../../components/course/Course';

const mapStateToProps = (state, { match }) => {
	const courseId = match.params.courseId;
	const slideId = match.params.slideId;
	const courseInfo = state.courses.fullCoursesInfo[courseId];
	return {
		courseId,
		slideId,
		courseInfo,
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
export default connected;


function mapCourseInfoToUnits(courseInfo) {
	if (!courseInfo || !courseInfo.units) {
		return null;
	}
	return courseInfo.units.reduce((acc, item) => {
		acc[item.id] = item;
		return acc;
	}, {});
}
