import React, { Component } from 'react';
import PropTypes from "prop-types";

import OpenedFlashcard from "../Flashcards/OpenedFlashcard/OpenedFlashcard";

import styles from './../Flashcards/flashcards.less';

import override from './courseToolUnitPage.less';
import classnames from 'classnames';
import getFlashcardsWithTheorySlides from "src/pages/course/getFlashcardsWithTheorySlides";
import { loadFlashcards, sendFlashcardResult } from "src/actions/course";
import { connect } from "react-redux";
import CourseFlashcardsPage from "src/components/flashcards/CourseFlashcardsPage/CourseFlashcardsPage";


class PreviewUnitPageFromAllCourse extends Component {
	render() {
		const { flashcards, loadFlashcards, courseId, flashcardsLoading } = this.props;

		if(flashcardsLoading) {
			return null;
		}

		if(flashcards.length === 0) {
			loadFlashcards(courseId);
			return null;
		}

		const allFlashcards = [];
		let unitFlashcards = [flashcards[0]];
		let currentUnit = flashcards[0].unitTitle;

		for (let i = 1; i < flashcards.length; i++) {
			const { unitTitle, } = flashcards[i];

			if(currentUnit !== unitTitle) {
				currentUnit = unitTitle;

				allFlashcards.push({ flashcards: unitFlashcards, unitTitle });

				unitFlashcards = [];
			}

			unitFlashcards.push(flashcards[i]);
		}

		return allFlashcards.map(({ flashcards, unitTitle }) =>
			<React.Fragment key={ unitTitle }>
				<h2>{ unitTitle }</h2>
				{ flashcards.map(({ nitTitle, answer, question, theorySlides }, i) => (
					<div className={ classnames(styles.modal, override.modal) } key={ i }>
						<OpenedFlashcard
							unitTitle={ unitTitle }
							answer={ answer }
							question={ question }
							theorySlides={ theorySlides || [] }

							onShowAnswer={ this.mockFunction }
							onClose={ this.mockFunction }
							onHandlingResultsClick={ this.mockFunction }
						/>
					</div>))
				}
			</React.Fragment>
		);
	}

	mockFunction = () => {
	}
}

const mapStateToProps = (state, { match }) => {
	let { courseId } = match.params;
	courseId = courseId.toLowerCase();

	const data = state.courses;
	const courseInfo = data.fullCoursesInfo[courseId];
	const infoByUnits = Object.values(data.flashcardsByUnits);

	if(!courseInfo) {
		return {
			courseId,
			flashcardsLoading: data.flashcardsLoading,
			flashcards: [],
		}
	}

	const courseSlides = courseInfo.units
		.reduce((slides, unit) => ([...slides, ...unit.slides]), []);
	const courseFlashcards = data.flashcardsByCourses[courseId];
	const flashcards = getFlashcardsWithTheorySlides(infoByUnits, courseFlashcards, courseSlides);

	return {
		courseId,
		flashcardsLoading: data.flashcardsLoading,
		flashcards,
	}
};

const mapDispatchToProps = (dispatch) => ({
	loadFlashcards: (courseId) => dispatch(loadFlashcards(courseId)),
});

PreviewUnitPageFromAllCourse.propTypes = {
	courseId: PropTypes.string,
	flashcardsLoading: PropTypes.bool,
	flashcards: PropTypes.arrayOf(PropTypes.shape({
		question: PropTypes.string,
		answer: PropTypes.string,
		unitTitle: PropTypes.string,
		theorySlides: PropTypes.arrayOf(
			PropTypes.shape({
				slug: PropTypes.string,
				title: PropTypes.string,
			}),
		),
	})),
	loadFlashcards: PropTypes.func,
};

const connected = connect(mapStateToProps, mapDispatchToProps)(PreviewUnitPageFromAllCourse);
export default connected;
