import {connect} from "react-redux";
import {loadFlashcardsInfo, loadFlashcardsPack, sendFlashcardResult, loadStatistics} from '../../actions/course';

import UnitFlashcards from "../../components/flashcards/UnitPage/UnitPage";

const mapStateToProps = (state, {match}) => {
	const {courseId, unitId} = match.params;
	const data = state.courses;

	console.log(data);
	const statistics = unitId in data.flashcardsStatisticsByUnits
		? data.flashcardsStatisticsByUnits[unitId].statistics
		: undefined;
	const totalFlashcardsCount = unitId in data.flashcardsStatisticsByUnits
		? data.flashcardsStatisticsByUnits[unitId].totalFlashcardsCount
		: undefined;
	const flashcardsPack = courseId in data.flashcardsPackByCourses
		? data.flashcardsPackByCourses[courseId].filter(flashcard => flashcard.unitId === unitId)
		: undefined;
	const unitTitle = data.fullCoursesInfo[courseId].units.find(unit => unit.id === unitId).title;

	return {
		unitTitle,
		courseId,
		unitId,
		statisticsLoading: data.flashcardsStatisticsByUnitsLoading,
		statistics,
		totalFlashcardsCount,
		flashcardsPack,
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

