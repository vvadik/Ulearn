import { connect } from "react-redux";
import { loadCourse } from "../../actions/course";
import { loadUserProgress, userProgressUpdate } from "../../actions/userProgress";
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
		isLti = action.toUpperCase() === "LTISLIDE";
	} else {
		const slideSlug = slideSlugOrAction;
		slideId = slideSlug.split('_').pop();
	}

	const courseInfo = state.courses.fullCoursesInfo[courseId];
	const isReview = params.CheckQueueItemId !== undefined;
	const isNavMenuVisible = !isLti && !isReview;
	return {
		courseId,
		slideId,
		courseInfo,
		isNavMenuVisible,
		units: mapCourseInfoToUnits(courseInfo),
		user: state.account,
		progress: state.userProgress.progress[courseId],
		navigationOpened: state.navigation.opened,
		courseLoadingErrorStatus: state.courses.courseLoadingErrorStatus,
	};
};
const mapDispatchToProps = (dispatch) => ({
	loadCourse: (courseId) => dispatch(loadCourse(courseId)),
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
