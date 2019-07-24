import React, { Component } from 'react';
import PropTypes from "prop-types";
import styles from './flashcards.less';
import classNames from 'classnames';
import ProgressBar from "../ProgressBar/ProgressBar";
import FrontFlashcard from "./FrontFlashcard/FrontFlashcard";
import { rateTypes } from "../../../consts/rateTypes";
import mixFlashcards from "./flashcardsStirrer";

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

		let maxTLast = Math.max(...this.props.flashcards.reduce((acc, fc) => [...acc, fc.lastRateIndex], []));

		this.state = {
			currentFlashcard: null,
			nextFlashcard: null,
			maxTLast: maxTLast,
		}
	}

	componentDidMount() {
		document.getElementsByTagName('body')[0].classList.add(styles.overflow);

		this.takeNextFlashcard(this.state.maxTLast);
	}

	componentWillUnmount() {
		document.getElementsByTagName('body')[0].classList.remove(styles.overflow);
	}

	render() {
		const flashcard = this.state.currentFlashcard;
		const { totalFlashcardsCount, statistics } = this.props;

		return (
			<div ref={ (ref) => this.overlay = ref } className={ styles.overlay } onClick={ this.handleOverlayClick }>
				<div ref={ (ref) => this.firstModal = ref } className={ modalsStyles.first }>
					{ flashcard &&
					<FrontFlashcard
						onShowAnswer={ () => this.resetCardsAnimation() }
						question={ flashcard.question }
						answer={ flashcard.answer }
						unitTitle={ flashcard.unitTitle }
						onHandlingResultsClick={ (rate) => this.handleResultsClick(rate) }
						onClose={ this.props.onClose }
					/> }
				</div>
				<div ref={ (ref) => this.secondModal = ref } className={ modalsStyles.second }/>
				<div ref={ (ref) => this.thirdModal = ref } className={ modalsStyles.third }/>
				<div className={ modalsStyles.fourth }/>
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
		const { currentFlashcard, maxTLast } = this.state;
		const { courseId, sendFlashcardRate } = this.props;
		const newTLast = maxTLast + 1;

		sendFlashcardRate(courseId, currentFlashcard.unitId, currentFlashcard.id, mapRateToRateType[rate], newTLast);

		this.setState({
			maxTLast: newTLast,
		});

		this.takeNextFlashcard(newTLast);
	};

	takeNextFlashcard(tLast) {
		const { flashcards } = this.props;

		const newSequence = mixFlashcards(flashcards, tLast);

		this.setState({ currentFlashcard: newSequence[0] });

		this.animateCards();
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

		firstModal.className = classNames(modalsStyles.first);
		secondModal.className = classNames(modalsStyles.second);
		thirdModal.className = classNames(modalsStyles.third);
	}
}


Flashcards.propTypes = {
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
	courseId: PropTypes.string,
	onClose: PropTypes.func,
	sendFlashcardRate: PropTypes.func,
};

export default Flashcards;
