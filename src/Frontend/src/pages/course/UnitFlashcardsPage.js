import { connect } from "react-redux";
import { sendFlashcardResult, loadFlashcards } from '../../actions/course';

import UnitFlashcards from "../../components/flashcards/UnitPage/UnitPage";
import Course from "../../components/course/Course";

const mapStateToProps = (state, { match }) => {
	const { courseId, slideId } = match.params;

	const data = state.courses;
	const courseInfo = data.fullCoursesInfo[courseId];
	const unitId = Course.findUnitIdBySlide(slideId, courseInfo);

	const flashcardsByUnits = data.flashcards[courseId] || [];
	const unitWithFlashcards = flashcardsByUnits.find(unit => unit.unitId === unitId);
	const flashcards = unitWithFlashcards ? unitWithFlashcards.flashcards : [];
	const unitTitle = unitWithFlashcards ? unitWithFlashcards.unitTitle : null;
	const totalFlashcardsCount = flashcards ? flashcards.length : 0;

	const statistics = {
		notRated: 0,
		rate1: 0,
		rate2: 0,
		rate3: 0,
		rate4: 0,
		rate5: 0,
	};

	for (const flashcard of flashcards) {
		statistics[flashcard.rate]++;
	}

	return {
		courseId,
		unitTitle,
		flashcards,
		flashcardsLoading: data.flashcardsLoading,
		totalFlashcardsCount,
		statistics,
	}
};
const mapDispatchToProps = (dispatch) => ({
	loadFlashcards: (courseId) => dispatch(loadFlashcards(courseId)),
	sendFlashcardRate: (courseId, unitId, flashcardId, rate) => dispatch(sendFlashcardResult(courseId, unitId, flashcardId, rate)),
});


const connected = connect(mapStateToProps, mapDispatchToProps)(UnitFlashcards);
export default connected;

