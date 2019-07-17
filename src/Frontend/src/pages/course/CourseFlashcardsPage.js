import {connect} from "react-redux";
import {loadFlashcardsInfo, loadFlashcardsPack, sendFlashcardResult} from '../../actions/course';

import CourseFlashcards from "../../components/flashcards/CoursePage/CoursePage";

const mapStateToProps = (state, {match}) => {
	const {courseId} = match.params;
	const data = state.courses;

	return {
		courseId,
		flashcardsInfo: data.flashcardsInfo[courseId],
		flashcardsPack: data.flashcardsPackByCourses[courseId],
		unitsInfo: data.fullCoursesInfo[courseId].units,
	}
};
const mapDispatchToProps = (dispatch) => ({
	loadFlashcardsInfo: (courseId) => dispatch(loadFlashcardsInfo(courseId)),
	loadFlashcardsPack: (courseId, unitId, count, flashcardOrder, rate) => dispatch(loadFlashcardsPack(courseId, unitId, count, flashcardOrder, rate)),
	sendFlashcardRate: (courseId, flashcardId, rate) => dispatch(sendFlashcardResult(courseId, flashcardId, rate)),
});


const connected = connect(mapStateToProps, mapDispatchToProps)(CourseFlashcards);
export default connected;

