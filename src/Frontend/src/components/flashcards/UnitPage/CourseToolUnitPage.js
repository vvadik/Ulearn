import React, {Component} from 'react';
import PropTypes from "prop-types";

import OpenedFlashcard from "../Flashcards/OpenedFlashcard/OpenedFlashcard";

import styles from './../Flashcards/flashcards.less';

import override from './courseToolUnitPage.less';
import classnames from 'classnames';


class CourseToolUnitPage extends Component {
	render() {
		const {flashcards} = this.props;
		const unitFlashcards = [];
		const emptyDelegate = () => {
		};
		const flashcardClass = classnames(styles.modal, override.modal);

		for (let i = 0; i < flashcards.length; i++) {
			const {unitTitle, answer, question, theorySlides} = flashcards[i];
			unitFlashcards.push(
				<div className={flashcardClass} key={i}>
					<OpenedFlashcard
						unitTitle={unitTitle}
						answer={answer}
						question={question}
						theorySlides={theorySlides || []}

						onShowAnswer={emptyDelegate}
						onClose={emptyDelegate}
						onHandlingResultsClick={emptyDelegate}
					/>
				</div>);
		}

		return unitFlashcards;
	}
}

CourseToolUnitPage.propTypes = {
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
};

export default CourseToolUnitPage;
