import { connect } from "react-redux";
import { withRouter } from "react-router-dom";

import UnitFlashcards from "src/components/flashcards/UnitPage/UnitPage";
import Course from "src/components/course/Course";

import { sendFlashcardResult, loadFlashcards } from 'src/actions/flashcards';
import getFlashcardsWithTheorySlides from "./getFlashcardsWithTheorySlides";

const mapStateToProps = (state, { match }) => {
	let { courseId, slideSlugOrAction } = match.params;
	courseId = courseId.toLowerCase();
	const slideId = slideSlugOrAction.split('_').pop();
	const data = state.courses;
	const courseInfo = data.fullCoursesInfo[courseId];
	const infoByUnits = data.flashcardsInfoByCourseByUnits[courseId] ? Object.values(data.flashcardsInfoByCourseByUnits[courseId]) : [];
	const unitId = Course.findUnitIdBySlideId(slideId, courseInfo);
	const unitInfo = data.flashcardsInfoByCourseByUnits[courseId] ? data.flashcardsInfoByCourseByUnits[courseId][unitId] : {};

	if(!courseInfo) {
		return {
			courseId,
			unitId,
			unitTitle: unitInfo ? unitInfo.unitTitle : null,
			infoByUnits,
			flashcards: [],
			flashcardsLoading: data.flashcardsLoading,
		}
	}

	const courseSlides = courseInfo.units
		.reduce((slides, unit) => ([...slides, ...unit.slides]), []);
	const courseFlashcards = data.flashcardsByCourses[courseId];
	const flashcards = courseFlashcards ? getFlashcardsWithTheorySlides(infoByUnits, courseFlashcards, courseSlides) : [];

	return {
		courseId,
		unitId,
		unitTitle: unitInfo ? unitInfo.unitTitle : null,
		infoByUnits,
		flashcards,
		flashcardsLoading: data.flashcardsLoading,
	}
};
const mapDispatchToProps = (dispatch) => ({
	loadFlashcards: (courseId) => dispatch(loadFlashcards(courseId)),
	sendFlashcardRate: (courseId, unitId, flashcardId, rate, newTLast) => dispatch(sendFlashcardResult(courseId, unitId, flashcardId, rate, newTLast)),
});


const connected = connect(mapStateToProps, mapDispatchToProps)(UnitFlashcards);
export default withRouter(connected);

