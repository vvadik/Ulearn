import { connect } from "react-redux";

import CourseFlashcards from "../../components/flashcards/CoursePage/CoursePage";

import { sendFlashcardResult, loadFlashcards } from '../../actions/course';

const mapStateToProps = (state, { match }) => {
	let { courseId } = match.params;
	courseId = courseId.toLowerCase();

	const data = state.courses;
	const courseInfo = data.fullCoursesInfo[courseId];
	const infoByUnits = Object.values(data.flashcardsByUnits);

	const courseSlides = courseInfo.units.reduce((slides, unit) => {
		return [...slides, ...unit.slides];
	}, []);

	const flashcards = [];

	for (const { unlocked, flashcardsIds } of infoByUnits) {
		if (!unlocked) {
			continue;
		}

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
		infoByUnits,
		flashcards,
		flashcardsLoading: data.flashcardsLoading,
	}
};
const mapDispatchToProps = (dispatch) => ({
	loadFlashcards: (courseId) => dispatch(loadFlashcards(courseId)),
	sendFlashcardRate: (courseId, unitId, flashcardId, rate, newTLast) => dispatch(sendFlashcardResult(courseId, unitId, flashcardId, rate, newTLast)),
});

const connected = connect(mapStateToProps, mapDispatchToProps)(CourseFlashcards);
export default connected;

