import {connect} from "react-redux";
import {loadFlashcardsInfo, loadFlashcardsPack, loadStatistics, sendFlashcardResult} from '../../actions/course';

import CourseFlashcards from "../../components/flashcards/CoursePage/CoursePage";

const mapStateToProps = (state, {match}) => {
	const {courseId} = match.params;
	const data = state.courses;

	const statistics = courseId in data.flashcardsStatisticsByUnits
		? data.flashcardsStatisticsByUnits[courseId].statistics
		: undefined;
	const totalFlashcardsCount = courseId in data.flashcardsStatisticsByUnits
		? data.flashcardsStatisticsByUnits[courseId].totalFlashcardsCount
		: undefined;

	return {
		courseId,
		totalFlashcardsCount,
		statistics,
		flashcardsInfo: data.flashcardsInfo[courseId],
		flashcardsInfoLoading: data.flashcardsInfoLoading,
		flashcardsPack: data.flashcardsPackByCourses[courseId],
		unitsInfo: data.fullCoursesInfo[courseId].units,
	}
};
const mapDispatchToProps = (dispatch) => ({
	loadFlashcardsInfo: (courseId) => dispatch(loadFlashcardsInfo(courseId)),
	loadFlashcardsPack: (courseId, unitId, count, flashcardOrder, rate) => dispatch(loadFlashcardsPack(courseId, unitId, count, flashcardOrder, rate)),
	sendFlashcardRate: (courseId, flashcardId, rate) => dispatch(sendFlashcardResult(courseId, flashcardId, rate)),
	loadStatistics: (courseId) => dispatch(loadStatistics(courseId)),
});


const connected = connect(mapStateToProps, mapDispatchToProps)(CourseFlashcards);
export default connected;

