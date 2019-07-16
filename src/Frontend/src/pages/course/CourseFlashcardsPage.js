import {connect} from "react-redux";
import {loadFlashcardsInfo, loadFlashcardsPack, sendFlashcardResult} from '../../actions/course';

import CourseFlashcards from "../../components/flashcards/CoursePage/CoursePage";

const mapStateToProps = (state, {match}) => {
	const {courseId} = match.params;

	return {
		courseId,
		flashcardsInfo: state.courses.flashcardsInfo[courseId],
		flashcardsPack: state.courses.flashcardsPackByCourses[courseId],
		unitsInfo: state.courses.fullCoursesInfo[courseId].units,
	}
};
const mapDispatchToProps = (dispatch) => ({
	loadFlashcardsInfo: (courseId) => dispatch(loadFlashcardsInfo(courseId)),
	loadFlashcardsPack: (courseId, unitId, count, flashcardOrder, rate) => dispatch(loadFlashcardsPack(courseId, unitId, count, flashcardOrder, rate)),
	sendFlashcardRate: (courseId, flashcardId, rate) => dispatch(sendFlashcardResult(courseId, flashcardId, rate)),
});


const connected = connect(mapStateToProps, mapDispatchToProps)(CourseFlashcards);
export default connected;

