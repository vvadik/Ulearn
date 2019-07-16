import React, {Component} from 'react';
import PropTypes from "prop-types";

import UnitCard from "./UnitCard/UnitCard";
import Guides from "../Guides/Guides";
import ProgressBar from "../ProgressBar/ProgressBar";
import ShortQuestions from "./ShortQuestions/ShortQuestions";
import Flashcards from "../Flashcards/Flashcards";

import styles from './unitPage.less';
import {guides} from '../consts';
import Loader from "@skbkontur/react-ui/Loader";


class UnitPage extends Component {
	constructor(props) {
		super(props);

		this.state = {
			haveProgress: false,
			showFlashcards: false
		}
	}

	componentDidMount() {
		const {courseId, unitId, flashcardsPack, loadFlashcardsPack, loadStatistics} = this.props;

		document.getElementsByTagName('main')[0].classList.add(styles.pageContainer);

		if (!flashcardsPack) {
			loadFlashcardsPack(courseId, unitId);
		}

		loadStatistics(courseId, unitId);
	}

	componentWillUnmount() {
		document.getElementsByTagName('main')[0].classList.remove(styles.pageContainer);
	}

	render() {
		const {unitTitle, totalFlashcardsCount, flashcardsPack, courseId, statistics, sendFlashcardRate, statisticsLoading} = this.props;
		const haveProgress = statistics && statistics.notRated !== totalFlashcardsCount;
		const dataLoaded = statistics && flashcardsPack && !statisticsLoading;
		const {showFlashcards} = this.state;

		return (
			<Loader active={!dataLoaded} type={'big'}>
				<h3 className={styles.title}>
					Вопросы для самопроверки
				</h3>
				{dataLoaded &&
				<UnitCard
					haveProgress={haveProgress}
					totalFlashcardsCount={totalFlashcardsCount}
					unitTitle={unitTitle}
					handleStartButton={() => this.showFlashcards()}
				/>}
				{this.renderFooter(haveProgress && dataLoaded)}
				{showFlashcards &&
				<Flashcards
					onClose={() => this.hideFlashcards()}
					flashcards={flashcardsPack}
					courseId={courseId}
					sendFlashcardRate={sendFlashcardRate}
				/>}
			</Loader>
		);
	}

	renderFooter(shouldRenderProgress) {
		const {statistics, totalFlashcardsCount} = this.props;

		if (shouldRenderProgress) {
			return (
				<div>
					<p className={styles.progressBarTitle}>
						Результаты последнего прохождения
					</p>
					<ProgressBar
						statistics={statistics}
						totalFlashcardsCount={totalFlashcardsCount}
					/>
					<ShortQuestions
						className={styles.shortQuestions}
						questionsWithAnswers={this.mapFlashcardsToQuestionWithAnswers()}
					/>
				</div>
			);
		}

		return <Guides guides={guides}/>;
	}

	showFlashcards() {
		this.setState({
			showFlashcards: true
		});
	}

	hideFlashcards() {
		const {courseId, unitId, loadStatistics} = this.props;

		loadStatistics(courseId, unitId);

		this.setState({
			showFlashcards: false
		});
	}

	mapFlashcardsToQuestionWithAnswers() {
		const {flashcardsPack} = this.props;

		return flashcardsPack
			.filter(flashcard => flashcard.rate !== 'notRated')
			.map(flashcard => {
				return {
					question: flashcard.question,
					answer: flashcard.answer,
				}
			})
	}
}

UnitPage.propTypes = {
	courseId: PropTypes.string,
	unitTitle: PropTypes.string,
	flashcardsPack: PropTypes.arrayOf(PropTypes.shape({
		id: PropTypes.string,
		question: PropTypes.string,
		answer: PropTypes.string,
		unitTitle: PropTypes.string,
		rate: PropTypes.string,
		unitId: PropTypes.string
	})),
	totalFlashcardsCount: PropTypes.number,
	statistics: PropTypes.shape({
		notRated: PropTypes.number,
		rate1: PropTypes.number,
		rate2: PropTypes.number,
		rate3: PropTypes.number,
		rate4: PropTypes.number,
		rate5: PropTypes.number
	}),
	statisticsLoading: PropTypes.bool,

	loadFlashcardsPack: PropTypes.func,
	loadStatistics: PropTypes.func,
	sendFlashcardRate: PropTypes.func,
};

export default UnitPage;
