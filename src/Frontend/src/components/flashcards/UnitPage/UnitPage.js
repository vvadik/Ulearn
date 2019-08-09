import React, { Component } from 'react';
import PropTypes from "prop-types";

import UnitCard from "./UnitCard/UnitCard";
import Guides from "../Guides/Guides";
import ProgressBar from "../ProgressBar/ProgressBar";
import ShortQuestions from "./ShortQuestions/ShortQuestions";
import Flashcards from "../Flashcards/Flashcards";

import styles from './unitPage.less';
import { guides } from '../consts';
import Loader from "@skbkontur/react-ui/Loader";
import { rateTypes } from "../../../consts/rateTypes";
import countFlashcardsStatistics from "../../../utils/countFlashcardsStatistics";


class UnitPage extends Component {
	constructor(props) {
		super(props);
		const { flashcards, unitId } = this.props;

		const unitFlashcards = flashcards.filter(fc => fc.unitId === unitId);

		this.state = {
			unitFlashcards,
			showFlashcards: false,
			statistics: countFlashcardsStatistics(unitFlashcards),
			totalFlashcardsCount: unitFlashcards.length,
		}
	}

	componentWillReceiveProps(nextProps, nextContext) {
		const { flashcards, unitId } = nextProps;

		const unitFlashcards = flashcards.filter(flashcard => flashcard.unitId === unitId);

		this.setState({
			unitFlashcards,
			statistics: countFlashcardsStatistics(unitFlashcards),
			totalFlashcardsCount: unitFlashcards.length,
		});
	}

	componentDidMount() {
		const { courseId, flashcards, loadFlashcards } = this.props;

		window.scrollTo(0, 0);

		if (flashcards.length === 0) {
			loadFlashcards(courseId);
		}
	}

	render() {
		const { courseId, unitTitle, flashcards, flashcardsLoading, sendFlashcardRate, unitId, infoByUnits } = this.props;
		const { statistics, totalFlashcardsCount, showFlashcards } = this.state;
		const haveProgress = flashcards && statistics[rateTypes.notRated] !== totalFlashcardsCount;
		const completedUnit = flashcards && statistics[rateTypes.notRated] === 0;
		const dataLoaded = flashcards && !flashcardsLoading;

		return (
			<Loader active={ flashcardsLoading } type={ 'big' }>
				<h3 className={ styles.title }>
					Вопросы для самопроверки
				</h3>
				{ unitTitle &&
				<UnitCard
					haveProgress={ completedUnit }
					totalFlashcardsCount={ totalFlashcardsCount }
					unitTitle={ unitTitle }
					handleStartButton={ () => this.showFlashcards() }
				/> }
				{ this.renderFooter(haveProgress && dataLoaded) }
				{ showFlashcards &&
				<Flashcards
					infoByUnits={ infoByUnits }
					unitId={ unitId }
					onClose={ () => this.hideFlashcards() }
					flashcards={ flashcards }
					courseId={ courseId }
					sendFlashcardRate={ sendFlashcardRate }
				/> }
			</Loader>
		);
	}

	renderFooter(shouldRenderProgress) {
		const { statistics, totalFlashcardsCount } = this.state;

		if (shouldRenderProgress) {
			return (
				<div>
					<p className={ styles.progressBarTitle }>
						Результаты последнего прохождения
					</p>
					<ProgressBar
						statistics={ statistics }
						totalFlashcardsCount={ totalFlashcardsCount }
					/>
					<ShortQuestions
						className={ styles.shortQuestions }
						questionsWithAnswers={ this.mapFlashcardsToQuestionWithAnswers() }
					/>
				</div>
			);
		}

		return <Guides guides={ guides }/>;
	}

	showFlashcards() {
		this.setState({
			showFlashcards: true
		});
	}

	hideFlashcards() {
		this.setState({
			showFlashcards: false
		});
	}

	mapFlashcardsToQuestionWithAnswers() {
		const { unitFlashcards } = this.state;

		return unitFlashcards
			.filter(({ rate }) => rate !== rateTypes.notRated)
			.map(({ question, answer, }) => {
				return { question, answer, }
			});
	}
}

UnitPage.propTypes = {
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

export default UnitPage;
