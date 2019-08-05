import { connect } from "react-redux";
import { sendFlashcardResult, loadFlashcards } from '../../actions/course';

import UnitFlashcards from "../../components/flashcards/UnitPage/UnitPage";
import Course from "../../components/course/Course";

const mapStateToProps = (state, { match }) => {
	const { courseId, slideId } = match.params;

	const data = state.courses;
	const courseInfo = data.fullCoursesInfo[courseId];
	const infoByUnits = Object.values(data.flashcardsByUnits);
	const unitId = Course.findUnitIdBySlide(slideId, courseInfo);
	const unitInfo = data.flashcardsByUnits[unitId];

	const courseSlides = courseInfo.units.reduce((slides, unit) => {
		return [...slides, ...unit.slides];
	}, []);

	const flashcards = [];

	for (const { flashcardsIds } of infoByUnits) {
		for (const id of flashcardsIds) {
			const flashcard = data.flashcardsByCourses[courseId][id];
			const { theorySlidesIds } = flashcard;
			const theorySlides = [];

			for (const slideId of theorySlidesIds) {
				const courseSlide = courseSlides.find(slide => slide.id === slideId);
				if (courseSlide) {
					theorySlides.push({
						slug: courseSlide.slug,
						title: courseSlide.title,
					});
				}
			}

			flashcards.push({
				...flashcard,
				theorySlides,
			});
		}
	}

	return {
		courseId,
		unitTitle: unitInfo ? unitInfo.unitTitle : null,
		unitId,
		flashcards,
		flashcardsLoading: data.flashcardsLoading,
	}
};
const mapDispatchToProps = (dispatch) => ({
	loadFlashcards: (courseId) => dispatch(loadFlashcards(courseId)),
	sendFlashcardRate: (courseId, unitId, flashcardId, rate, newTLast) => dispatch(sendFlashcardResult(courseId, unitId, flashcardId, rate, newTLast)),
});


const connected = connect(mapStateToProps, mapDispatchToProps)(UnitFlashcards);
export default connected;

