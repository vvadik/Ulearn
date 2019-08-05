import React, { Component } from 'react';
import PropTypes from "prop-types";

import ProgressBar from "../ProgressBar/ProgressBar";
import FrontFlashcard from "./FrontFlashcard/FrontFlashcard";

import { rateTypes } from "../../../consts/rateTypes";

import getNextFlashcard from "./flashcardsStirrer";
import classNames from 'classnames';

import styles from './flashcards.less';
import countFlashcardsStatistics from "../../../utils/countFlashcardsStatistics";
import Toast from "@skbkontur/react-ui/Toast";

const modalsStyles = {
	first: classNames(styles.modal),
	second: classNames(styles.modal, styles.secondModal),
	third: classNames(styles.modal, styles.thirdModal),
	fourth: classNames(styles.modal, styles.fourthModal)
};

const mapRateToRateType = {
	1: rateTypes.rate1,
	2: rateTypes.rate2,
	3: rateTypes.rate3,
	4: rateTypes.rate4,
	5: rateTypes.rate5,
};

class Flashcards extends Component {
	constructor(props) {
		super(props);
		const { flashcards, unitId } = this.props;

		let sessionFlashcards = [...flashcards];

		if (unitId) {
			sessionFlashcards = sessionFlashcards.filter(fc => fc.unitId === unitId);
		}

		const lastRateIndexes = sessionFlashcards.reduce(
			(lastRateIndexes, flashcard) => [...lastRateIndexes, flashcard.lastRateIndex]
			, []);
		const maxTLast = Math.max(...lastRateIndexes);

		this.state = {
			onUnit: unitId !== null,
			currentFlashcard: null,
			maxTLast: maxTLast,
			ratedFlashcardsCount: 0,
			sessionFlashcards,
			statistics: countFlashcardsStatistics(sessionFlashcards),
		}
	}

	componentDidMount() {
		document.getElementsByTagName('body')[0]
			.classList.add(styles.overflow);

		this.takeNextFlashcard(this.state.maxTLast);
	}

	componentWillUnmount() {
		document.getElementsByTagName('body')[0]
			.classList.remove(styles.overflow);
	}

	render() {
		const { statistics, sessionFlashcards, currentFlashcard } = this.state;

		return (
			<div ref={ (ref) => this.overlay = ref } className={ styles.overlay } onClick={ this.handleOverlayClick }>

				<div className={ modalsStyles.first } ref={ (ref) => this.firstModal = ref }>
					{ currentFlashcard &&
					<FrontFlashcard
						onShowAnswer={ () => this.resetCardsAnimation() }
						question={ currentFlashcard.question }
						answer={ currentFlashcard.answer }
						unitTitle={ currentFlashcard.unitTitle }
						theorySlides={ currentFlashcard.theorySlides }
						onHandlingResultsClick={ (rate) => this.handleResultsClick(rate) }
						onClose={ this.props.onClose }
					/> }
				</div>

				<div className={ modalsStyles.second } ref={ (ref) => this.secondModal = ref }/>
				<div className={ modalsStyles.third } ref={ (ref) => this.thirdModal = ref }/>
				<div className={ modalsStyles.fourth }/>

				{ Flashcards.renderControlGuides() }

				<div className={ styles.progressBarContainer }>
					<ProgressBar statistics={ statistics } totalFlashcardsCount={ sessionFlashcards.length }/>
				</div>
			</div>
		)
	}

	handleOverlayClick = (e) => {
		const { onClose } = this.props;

		if (e.target === this.overlay) {
			onClose();
		}
	};

	static renderControlGuides() {
		return (
			<p className={ styles.controlGuideContainer }>
				Используйте клавиатуру:
				<span>пробел</span> — показать ответ,
				<span>1</span>
				<span>2</span>
				<span>3</span>
				<span>4</span>
				<span>5</span> — поставить оценку
			</p>
		);
	}

	handleResultsClick = async (rate) => {
		const { currentFlashcard, maxTLast, statistics } = this.state;
		const { courseId, sendFlashcardRate } = this.props;

		const newStatistics = { ...statistics };
		const newRate = mapRateToRateType[rate];
		const newTLast = maxTLast + 1;

		newStatistics[currentFlashcard.rate]--;
		newStatistics[newRate]++;

		currentFlashcard.rate = newRate;
		currentFlashcard.lastRateIndex = newTLast;

		await sendFlashcardRate(courseId, currentFlashcard.unitId, currentFlashcard.id, newRate, newTLast);

		this.setState({
			maxTLast: newTLast,
			statistics: newStatistics,
		});

		this.takeNextFlashcard(newTLast);
	};

	takeNextFlashcard(tLast) {
		const { ratedFlashcardsCount, sessionFlashcards, onUnit } = this.state;

		if (onUnit && Flashcards.hasFinishedUnit(sessionFlashcards, ratedFlashcardsCount)) {
			this.startRepeating(tLast);
		} else {
			this.setState({
				currentFlashcard: getNextFlashcard(sessionFlashcards, tLast),
			})
		}

		this.setState({
			ratedFlashcardsCount: ratedFlashcardsCount + 1,
		});

		this.animateCards();
	}

	static hasFinishedUnit(unitFlashcards, ratedFlashcardsCount) {
		return unitFlashcards.every(fc => fc.rate !== rateTypes.notRated)
			&& ratedFlashcardsCount >= unitFlashcards.length;
	}

	startRepeating(tLast) {
		const filteredFlashcards = [...this.props.flashcards.filter(fc => fc.rate !== rateTypes.notRated)];

		Toast.push('Переход к повторению по курсу');

		this.setState({
			onUnit: false,
			sessionFlashcards: filteredFlashcards,
			statistics: countFlashcardsStatistics(filteredFlashcards),
			currentFlashcard: getNextFlashcard(filteredFlashcards, tLast),
		});
	}

	animateCards() {
		const { firstModal, secondModal, thirdModal } = this;
		const animationDuration = 700;

		firstModal.className = classNames(modalsStyles.second, styles.move);
		secondModal.className = classNames(modalsStyles.third, styles.move);
		thirdModal.className = classNames(modalsStyles.fourth, styles.move);

		setTimeout(() => {
			this.resetCardsAnimation();
		}, animationDuration - animationDuration / 10);
	}

	resetCardsAnimation() {
		const { firstModal, secondModal, thirdModal } = this;

		if (!firstModal) {
			return;
		}

		firstModal.className = modalsStyles.first;
		secondModal.className = modalsStyles.second;
		thirdModal.className = modalsStyles.third;
	}
}


Flashcards.propTypes = {
	courseId: PropTypes.string,
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

	onClose: PropTypes.func,
	sendFlashcardRate: PropTypes.func,
};

export default Flashcards;
