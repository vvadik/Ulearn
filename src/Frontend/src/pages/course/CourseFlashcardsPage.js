import { connect } from "react-redux";

import CourseFlashcards from "../../components/flashcards/CoursePage/CoursePage";

import { sendFlashcardResult, loadFlashcards } from '../../actions/course';
import { rateTypes } from "../../consts/rateTypes";

const mapStateToProps = (state, { match }) => {
	const { courseId } = match.params;
	const data = state.courses;

	const flashcardsByUnits = data.flashcards[courseId] || [];

	const infoByUnits = [],
		allFlashcards = [],
		statistics = {
			[rateTypes.notRated]: 0,
			[rateTypes.rate1]: 0,
			[rateTypes.rate2]: 0,
			[rateTypes.rate3]: 0,
			[rateTypes.rate4]: 0,
			[rateTypes.rate5]: 0,
		};

	const courseUnits = data.fullCoursesInfo[courseId].units;
	for (const unit of flashcardsByUnits) {
		const { unitId, unitTitle, unlocked, flashcards } = unit;

		const unitSlides = courseUnits
			.find(unit => unit.id === unitId)
			.slides;

		infoByUnits.push({
			unitId,
			unitTitle,
			unlocked,
			cardsCount: flashcards.length,
			flashcardsSlideSlug: unitSlides[unitSlides.length - 1].slug,
		});

		if (unlocked) {
			allFlashcards.push(...flashcards);
			for (const flashcard of flashcards) {
				statistics[flashcard.rate]++;
			}
		}
	}

	return {
		courseId,
		infoByUnits,
		flashcards: allFlashcards,
		flashcardsLoading: data.flashcardsLoading,
		statistics,
		totalFlashcardsCount: allFlashcards.length,
	}
};
const mapDispatchToProps = (dispatch) => ({
	loadFlashcards: (courseId) => dispatch(loadFlashcards(courseId)),
	sendFlashcardRate: (courseId, unitId, flashcardId, rate) => dispatch(sendFlashcardResult(courseId, unitId, flashcardId, rate)),
});

const connected = connect(mapStateToProps, mapDispatchToProps)(CourseFlashcards);
export default connected;

