import { connect } from "react-redux";
import { sendFlashcardResult, loadFlashcards } from '../../actions/course';

import UnitFlashcards from "../../components/flashcards/UnitPage/UnitPage";
import Course from "../../components/course/Course";
import { rateTypes } from "../../consts/rateTypes";

const mapStateToProps = (state, { match }) => {
	const { courseId, slideId } = match.params;

	const data = state.courses;
	const courseInfo = data.fullCoursesInfo[courseId];
	const unitId = Course.findUnitIdBySlide(slideId, courseInfo);

	const statistics = {
		[rateTypes.notRated]: 0,
		[rateTypes.rate1]: 0,
		[rateTypes.rate2]: 0,
		[rateTypes.rate3]: 0,
		[rateTypes.rate4]: 0,
		[rateTypes.rate5]: 0,
	};

	const unitInfo = data.flashcardsByUnits[unitId];
	const flashcards = [];

	if (unitInfo) {
		for (const id of unitInfo.flashcardsIds) {
			const flashcard = data.flashcardsByCourses[courseId][id];
			statistics[flashcard.rate]++;
			flashcards.push(flashcard);
		}
	}

	return {
		courseId,
		unitTitle: unitInfo ? unitInfo.unitTitle : null,
		flashcards: flashcards,
		flashcardsLoading: data.flashcardsLoading,
		totalFlashcardsCount: flashcards.length,
		statistics,
	}
};
const mapDispatchToProps = (dispatch) => ({
	loadFlashcards: (courseId) => dispatch(loadFlashcards(courseId)),
	sendFlashcardRate: (courseId, unitId, flashcardId, rate, newTLast) => dispatch(sendFlashcardResult(courseId, unitId, flashcardId, rate, newTLast)),
});


const connected = connect(mapStateToProps, mapDispatchToProps)(UnitFlashcards);
export default connected;

