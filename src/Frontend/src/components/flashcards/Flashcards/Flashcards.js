import React, { Component } from 'react';
import PropTypes from "prop-types";

import ProgressBar from "../ProgressBar/ProgressBar";
import FrontFlashcard from "./FrontFlashcard/FrontFlashcard";
import Toast from "@skbkontur/react-ui/Toast";

import { sortFlashcardsInAuthorsOrder, getNextFlashcardRandomly } from "./flashcardsStirrer/flashcardsStirrer";
import countFlashcardsStatistics from "../../../utils/countFlashcardsStatistics";
import classNames from 'classnames';

import { rateTypes } from "../../../consts/rateTypes";

import styles from './flashcards.less';

const modalsClassNames = {
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
		const { flashcards, unitId, infoByUnits } = this.props;

		const onUnit = unitId !== undefined;
		let sessionFlashcards;
		let maxTLast = -1;

		if (onUnit) {
			sessionFlashcards = sortFlashcardsInAuthorsOrder(
				flashcards.filter(flashcard => flashcard.unitId === unitId)
			);
		} else {
			sessionFlashcards = Flashcards.getUnlockedCourseFlashcards(flashcards, infoByUnits);
			maxTLast = Flashcards.findMaxTLast(sessionFlashcards);
		}

		this.state = {
			onUnit,
			onUnitRepeating: false,
			currentFlashcard: null,
			maxTLast,
			sessionFlashcards,
			totalFlashcardsCount: sessionFlashcards.length,
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
		const { statistics, totalFlashcardsCount, currentFlashcard } = this.state;

		return (
			<div ref={ (ref) => this.overlay = ref } className={ styles.overlay } onClick={ this.handleOverlayClick }>

				<div className={ modalsClassNames.first } ref={ (ref) => this.firstModal = ref }>
					{ currentFlashcard && this.renderFrontFlashcard() }
				</div>

				<div className={ modalsClassNames.second } ref={ (ref) => this.secondModal = ref }/>
				<div className={ modalsClassNames.third } ref={ (ref) => this.thirdModal = ref }/>
				<div className={ modalsClassNames.fourth }/>

				{ Flashcards.renderControlGuides() }

				<div className={ styles.progressBarContainer }>
					<ProgressBar statistics={ statistics } totalFlashcardsCount={ totalFlashcardsCount }/>
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

	renderFrontFlashcard() {
		const { currentFlashcard } = this.state;
		const { onClose } = this.props;

		return (
			<FrontFlashcard
				onShowAnswer={ () => this.resetCardsAnimation() }
				question={ currentFlashcard.question }
				answer={ currentFlashcard.answer }
				unitTitle={ currentFlashcard.unitTitle }
				theorySlides={ currentFlashcard.theorySlides }
				onHandlingResultsClick={ (rate) => this.handleResultsClick(rate) }
				onClose={ onClose }
			/>
		);
	}

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

	handleResultsClick = (rate) => {
		const { currentFlashcard, maxTLast, statistics } = this.state;
		const { courseId, sendFlashcardRate } = this.props;

		const newStatistics = { ...statistics };
		const newRate = mapRateToRateType[rate];
		const newTLast = maxTLast + 1;

		newStatistics[currentFlashcard.rate]--;
		newStatistics[newRate]++;

		currentFlashcard.rate = newRate;
		currentFlashcard.lastRateIndex = newTLast;

		sendFlashcardRate(courseId, currentFlashcard.unitId, currentFlashcard.id, newRate, newTLast);

		this.setState({
			maxTLast: newTLast,
			statistics: newStatistics,
		}, this.takeNextFlashcard);
	};

	takeNextFlashcard() {
		const { sessionFlashcards, onUnit, onUnitRepeating, maxTLast } = this.state;

		if (onUnit) {
			if (sessionFlashcards.length === 0) {
				if (onUnitRepeating) {
					this.startCourseRepeating();
				} else {
					this.startUnitRepeating();
				}
			} else {
				this.setState({ currentFlashcard: sessionFlashcards.shift(), })
			}
		} else {
			this.setState({
				currentFlashcard: getNextFlashcardRandomly(sessionFlashcards, maxTLast),
			})
		}

		this.animateCards();
	}

	startUnitRepeating() {
		const { flashcards, unitId } = this.props;

		const failedUnitFlashcards = sortFlashcardsInAuthorsOrder(Flashcards.getUnitFlashcards(flashcards, unitId, true));

		if (failedUnitFlashcards.length > 0) {
			const firstFlashcard = failedUnitFlashcards.shift();

			this.setState({
				onUnitRepeating: true,
				currentFlashcard: firstFlashcard,
				sessionFlashcards: failedUnitFlashcards,
			})
		} else {
			this.startCourseRepeating();
		}
	}

	static getUnitFlashcards(courseFlashcards, unitId, onlyFailedFlashcards = false) {
		const unitFilter = onlyFailedFlashcards
			? (flashcard) => {
				return flashcard.unitId === unitId
					&& (flashcard.rate === rateTypes.rate1 || flashcard.rate === rateTypes.rate2)
			}
			: (flashcard) => {
				return flashcard.unitId === unitId
			};

		return courseFlashcards.filter(unitFilter);
	}

	startCourseRepeating() {
		const { flashcards, infoByUnits } = this.props;

		const unlockedCourseFlashcards = Flashcards.getUnlockedCourseFlashcards(flashcards, infoByUnits);
		const maxTLast = Flashcards.findMaxTLast(unlockedCourseFlashcards);

		Toast.push('Переход к повторению по курсу');

		this.setState({
			onUnit: false,
			onUnitRepeating: false,
			sessionFlashcards: unlockedCourseFlashcards,
			totalFlashcardsCount: unlockedCourseFlashcards.length,
			maxTLast,
			statistics: countFlashcardsStatistics(unlockedCourseFlashcards),
			currentFlashcard: getNextFlashcardRandomly(unlockedCourseFlashcards, maxTLast),
		});
	}

	static getUnlockedCourseFlashcards(flashcards, infoByUnits) {
		const unlocksByUnits = {};
		infoByUnits.forEach(({ unitId, unlocked }) => unlocksByUnits[unitId] = unlocked);

		return flashcards
			.filter(({ rate, unitId }) => rate !== rateTypes.notRated && unlocksByUnits[unitId]);
	}

	static findMaxTLast(flashcards) {
		let maxTLast = -1;

		for (const { lastRateIndex } of flashcards) {
			if (maxTLast < lastRateIndex) {
				maxTLast = lastRateIndex;
			}
		}

		return maxTLast;
	}

	animateCards() {
		const { firstModal, secondModal, thirdModal } = this;
		const animationDuration = 700;

		firstModal.className = classNames(modalsClassNames.second, styles.move);
		secondModal.className = classNames(modalsClassNames.third, styles.move);
		thirdModal.className = classNames(modalsClassNames.fourth, styles.move);

		setTimeout(() => {
			this.resetCardsAnimation();
		}, animationDuration - animationDuration / 10);
	}

	resetCardsAnimation() {
		const { firstModal, secondModal, thirdModal } = this;

		if (!firstModal) {
			return;
		}

		firstModal.className = modalsClassNames.first;
		secondModal.className = modalsClassNames.second;
		thirdModal.className = modalsClassNames.third;
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

	infoByUnits: PropTypes.arrayOf(PropTypes.shape({
		unlocked: PropTypes.bool,
		unitId: PropTypes.string,
	})),

	onClose: PropTypes.func,
	sendFlashcardRate: PropTypes.func,
};

export default Flashcards;
