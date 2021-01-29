import { connect } from "react-redux";

import CourseFlashcardsPage from "src/components/flashcards/CourseFlashcardsPage/CourseFlashcardsPage";

import { sendFlashcardResult, loadFlashcards } from 'src/actions/flashcards';
import getFlashcardsWithTheorySlides from "./getFlashcardsWithTheorySlides";

const mapStateToProps = (state, { match }) => {
	let { courseId } = match.params;
	courseId = courseId.toLowerCase();

	const data = state.courses;
	const courseInfo = data.fullCoursesInfo[courseId];
	const infoByUnits = Object.values(data.flashcardsByUnits);

	if(!courseInfo) {
		return {
			courseId,
			infoByUnits,
			flashcards: [],
			flashcardsLoading: data.flashcardsLoading,
		}
	}

	const courseSlides = courseInfo.units
		.reduce((slides, unit) => ([...slides, ...unit.slides]), []);
	const courseFlashcards = data.flashcardsByCourses[courseId];
	const flashcards = getFlashcardsWithTheorySlides(infoByUnits, courseFlashcards, courseSlides);

	return {
		courseId,
		infoByUnits,
		flashcards,
		flashcardsLoading: data.flashcardsLoading,
	}
};
const mapDispatchToProps = (dispatch) => ({
	loadFlashcards: (courseId) => dispatch(loadFlashcards(courseId)),
	sendFlashcardRate: (courseId, unitId, flashcardId, rate, newTLast) => dispatch(sendFlashcardResult(courseId, unitId, flashcardId, rate, newTLast)),
});

const connected = connect(mapStateToProps, mapDispatchToProps)(CourseFlashcardsPage);
export default connected;

