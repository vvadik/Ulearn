import React, { Component } from 'react';
import PropTypes from "prop-types";

import Loader from "@skbkontur/react-ui/Loader";
import UnitCard from "./UnitCard/UnitCard";
import Guides from "../Guides/Guides";
import ProgressBar from "../ProgressBar/ProgressBar";
import ShortQuestions from "./ShortQuestions/ShortQuestions";
import Flashcards from "../Flashcards/Flashcards";

import { guides } from '../consts';
import { rateTypes } from "../../../consts/rateTypes";

import countFlashcardsStatistics from "../countFlashcardsStatistics";

import styles from './unitPage.less';
import CourseToolFlashcards from "../Flashcards/CourseToolFlashcards";


class CourseToolUnitPage extends Component {
	constructor(props) {
		super(props);
		const { flashcards, unitId } = this.props;

		const unitFlashcards = flashcards;

		this.state = {
			unitFlashcards,
			showFlashcards: false,
			totalFlashcardsCount: unitFlashcards.length,
		}
	}


	componentDidMount() {
		const { courseId, flashcards } = this.props;


	}

	render() {
		const { courseId, unitTitle, flashcards, unitId } = this.props;
		const {  showFlashcards } = this.state;
		const completedUnit = false;

		return (
			<div>
				{
				<UnitCard
					haveProgress={ completedUnit }
					totalFlashcardsCount={ flashcards.length}
					unitTitle={ unitTitle }
					handleStartButton={ this.showFlashcards }
				/> }
				{ showFlashcards &&
				<CourseToolFlashcards
					unitId={ unitId }
					onClose={ this.hideFlashcards }
					flashcards={ flashcards }
					courseId={ courseId }
				/> }
			</div>
		);
	}

	renderFooter(shouldRenderProgress) {
		const { statistics, totalFlashcardsCount, unitFlashcards } = this.state;

		if (!shouldRenderProgress) {
			return <Guides guides={ guides }/>;
		}

		return (
			<footer>
				<p className={ styles.progressBarTitle }>
					Результаты последнего прохождения
				</p>
				<ProgressBar
					statistics={ statistics }
					totalFlashcardsCount={ totalFlashcardsCount }
				/>
				<ShortQuestions
					className={ styles.shortQuestions }
					questionsWithAnswers={ CourseToolUnitPage.mapFlashcardsToQuestionWithAnswers(unitFlashcards) }
				/>
			</footer>
		);
	}

	showFlashcards = () => {
		this.setState({
			showFlashcards: true
		});
	};

	hideFlashcards = () => {
		this.setState({
			showFlashcards: false
		});
	};

	static mapFlashcardsToQuestionWithAnswers(unitFlashcards) {
		return unitFlashcards
			.filter(({ rate }) => rate !== rateTypes.notRated)
			.map(({ question, answer, }) => ({ question, answer, }));
	}
}

CourseToolUnitPage.propTypes = {
	courseId: PropTypes.string,
	unitTitle: PropTypes.string,
	unitId: PropTypes.string,
	flashcards: PropTypes.arrayOf(PropTypes.shape({
		id: PropTypes.string,
		question: PropTypes.string,
		answer: PropTypes.string,
		unitTitle: PropTypes.string,
		rate: PropTypes.string,
		unitId: PropTypes.string,
		lastRateIndex: PropTypes.number,
		theorySlides: PropTypes.arrayOf(
			PropTypes.shape({
				slug: PropTypes.string,
				title: PropTypes.string,
			}),
		),
	})),
	infoByUnits: PropTypes.arrayOf(PropTypes.shape({
		unitTitle: PropTypes.string,
		unlocked: PropTypes.bool,
		cardsCount: PropTypes.number,
		unitId: PropTypes.string,
		flashcardsSlideSlug: PropTypes.string,
	})),

	loadFlashcards: PropTypes.func,
	sendFlashcardRate: PropTypes.func,
};

export default CourseToolUnitPage;
