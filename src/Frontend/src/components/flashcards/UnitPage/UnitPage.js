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


class UnitPage extends Component {
	constructor(props) {
		super(props);

		this.state = {
			showFlashcards: false,
		}
	}

	componentDidMount() {
		const { courseId, flashcards, loadFlashcards } = this.props;

		document.getElementsByTagName('main')[0].classList.add(styles.pageContainer);

		if (flashcards.length === 0) {
			loadFlashcards(courseId);
		}
	}

	componentWillUnmount() {
		document.getElementsByTagName('main')[0].classList.remove(styles.pageContainer);
	}

	render() {
		const { courseId, unitTitle, flashcards, flashcardsLoading, totalFlashcardsCount, statistics, sendFlashcardRate } = this.props;
		const haveProgress = flashcards && statistics[rateTypes.notRated] !== totalFlashcardsCount;
		const completedUnit = flashcards && statistics[rateTypes.notRated] === 0;
		const dataLoaded = flashcards && !flashcardsLoading;
		const { showFlashcards } = this.state;

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
					statistics={ statistics }
					totalFlashcardsCount={ totalFlashcardsCount }
					onClose={ () => this.hideFlashcards() }
					flashcards={ flashcards }
					courseId={ courseId }
					sendFlashcardRate={ sendFlashcardRate }
				/> }
			</Loader>
		);
	}

	renderFooter(shouldRenderProgress) {
		const { statistics, totalFlashcardsCount } = this.props;

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
		const { flashcards } = this.props;

		return flashcards
			.filter(({ rate }) => rate !== rateTypes.notRated)
			.map(({ question, answer, }) => {
				return { question, answer, }
			});
	}
}

UnitPage.propTypes = {
	courseId: PropTypes.string,
	unitTitle: PropTypes.string,
	flashcards: PropTypes.arrayOf(PropTypes.shape({
		id: PropTypes.string,
		question: PropTypes.string,
		answer: PropTypes.string,
		unitTitle: PropTypes.string,
		rate: PropTypes.string,
		unitId: PropTypes.string,
		lastRateIndex: PropTypes.number,
	})),
	totalFlashcardsCount: PropTypes.number,
	statistics: PropTypes.shape({
		[rateTypes.notRated]: PropTypes.number,
		[rateTypes.rate1]: PropTypes.number,
		[rateTypes.rate2]: PropTypes.number,
		[rateTypes.rate3]: PropTypes.number,
		[rateTypes.rate4]: PropTypes.number,
		[rateTypes.rate5]: PropTypes.number,
	}),

	loadFlashcards: PropTypes.func,
	sendFlashcardRate: PropTypes.func,
};

export default UnitPage;
