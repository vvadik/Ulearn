import {connect} from "react-redux";
import {loadFlashcardsInfo, loadFlashcardsPack, sendFlashcardResult, loadStatistics} from '../../actions/course';

import UnitFlashcards from "../../components/flashcards/UnitPage/UnitPage";

const mapStateToProps = (state, {match}) => {
	const {courseId, unitId} = match.params;
	const data = state.courses;

	const statistics = data.flashcardsStatisticsByUnits[unitId]
		? data.flashcardsStatisticsByUnits[unitId].statistics
		: undefined;
	const totalFlashcardsCount = data.flashcardsStatisticsByUnits[unitId]
		? data.flashcardsStatisticsByUnits[unitId].totalFlashcardsCount
		: undefined;
	const flashcardsPack = data.flashcardsPackByCourses[courseId]
		? data.flashcardsPackByCourses[courseId].filter(flashcard => flashcard.unitId === unitId)
		: undefined;


	return {
		unitTitle: data.fullCoursesInfo[courseId].units.find(unit => unit.id === unitId).title,
		courseId,
		unitId,
		statisticsLoading: data.flashcardsStatisticsByUnitsLoading,
		statistics: statistics,
		totalFlashcardsCount: totalFlashcardsCount,
		flashcardsPack: flashcardsPack,
	}
};
const mapDispatchToProps = (dispatch) => ({
	loadFlashcardsInfo: (courseId) => dispatch(loadFlashcardsInfo(courseId)),
	loadFlashcardsPack: (courseId, unitId, count, flashcardOrder, rate) => dispatch(loadFlashcardsPack(courseId, unitId, count, flashcardOrder, rate)),
	loadStatistics: (courseId, unitId) => dispatch(loadStatistics(courseId, unitId)),
	sendFlashcardRate: (courseId, flashcardId, rate) => dispatch(sendFlashcardResult(courseId, flashcardId, rate)),
});


const connected = connect(mapStateToProps, mapDispatchToProps)(UnitFlashcards);
export default connected;

