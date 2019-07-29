import { connect } from "react-redux";

import CourseFlashcards from "../../components/flashcards/CoursePage/CoursePage";

import { sendFlashcardResult, loadFlashcards } from '../../actions/course';
import { rateTypes } from "../../consts/rateTypes";

const mapStateToProps = (state, { match }) => {
	const { courseId } = match.params;
	const data = state.courses;

	const flashcards = [],
		statistics = {
			[rateTypes.notRated]: 0,
			[rateTypes.rate1]: 0,
			[rateTypes.rate2]: 0,
			[rateTypes.rate3]: 0,
			[rateTypes.rate4]: 0,
			[rateTypes.rate5]: 0,
		};

	const infoByUnits = Object.values(data.flashcardsByUnits);

	infoByUnits.forEach(({ unlocked, flashcardsIds }) => {
		if (unlocked) {
			for (const id of flashcardsIds) {
				const flashcard = data.flashcardsByCourses[courseId][id];
				flashcards.push(flashcard);
				statistics[flashcard.rate]++;
			}
		}
	});

	return {
		courseId,
		infoByUnits,
		flashcards,
		flashcardsLoading: data.flashcardsLoading,
		statistics,
		totalFlashcardsCount: flashcards.length,
	}
};
const mapDispatchToProps = (dispatch) => ({
	loadFlashcards: (courseId) => dispatch(loadFlashcards(courseId)),
	sendFlashcardRate: (courseId, unitId, flashcardId, rate, newTLast) => dispatch(sendFlashcardResult(courseId, unitId, flashcardId, rate, newTLast)),
});

const connected = connect(mapStateToProps, mapDispatchToProps)(CourseFlashcards);
export default connected;

