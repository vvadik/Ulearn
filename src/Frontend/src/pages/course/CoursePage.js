import { connect } from "react-redux";
import { loadCourse, loadCourseErrors, changeCurrentCourseAction } from "src/actions/course";
import { loadUserProgress, userProgressUpdate } from "src/actions/userProgress";
import Course from '../../components/course/Course';
import { withRouter } from "react-router-dom";
import queryString from "query-string";

const mapStateToProps = (state, { match }) => {
	const params = queryString.parse(window.location.search);
	const slideIdInQuery = params.slideId;
	const courseId = match.params.courseId.toLowerCase();
	const slideSlugOrAction = match.params.slideSlugOrAction;

	let slideId;
	let isLti = false;
	if(slideIdInQuery) {
		const action = slideSlugOrAction;
		slideId = slideIdInQuery;
		isLti = action.toLowerCase() === "ltislide" || action.toLowerCase() === 'acceptedalert' || params.isLti;
	} else {
		const slideSlug = slideSlugOrAction;
		slideId = slideSlug.split('_').pop();
	}

	const courseInfo = state.courses.fullCoursesInfo[courseId];
	let courseErrors = state.courses.fullCoursesErrors[courseId];
	const isReview = params.CheckQueueItemId !== undefined;
	const isNavMenuVisible = !isLti && !isReview && courseErrors === null;
	return {
		courseId,
		slideId,
		courseInfo,
		courseErrors,
		isNavMenuVisible,
		isSlideReady: state.courses.isSlideReady,
		units: mapCourseInfoToUnits(courseInfo),
		user: state.account,
		progress: state.userProgress.progress[courseId],
		navigationOpened: state.navigation.opened,
		courseLoadingErrorStatus: state.courses.courseLoadingErrorStatus,
		isHijacked: state.userProgress.isHijacked,
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
