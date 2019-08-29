import React, {Component} from 'react';
import PropTypes from "prop-types";

import ProgressBar from "../ProgressBar/ProgressBar";
import OpenedFlashcard from "./OpenedFlashcard/OpenedFlashcard";
import Toast from "@skbkontur/react-ui/Toast";

import classNames from 'classnames';

import {rateTypes} from "../../../consts/rateTypes";

import styles from './flashcards.less';
import {sortFlashcardsInAuthorsOrder} from "./flashcardsStirrer/flashcardsStirrer";
import countFlashcardsStatistics from "../countFlashcardsStatistics";

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

class CourseToolFlashcards extends Component {
	constructor(props) {
		super(props);
		const {flashcards} = this.props;

		let sessionFlashcards = flashcards;
		this.state = {
			onUnitRepeating: false,
			currentFlashcardId: 0,
			currentFlashcard: sessionFlashcards[0],
			sessionFlashcards ,
			totalFlashcardsCount: sessionFlashcards.length,
		}
		console.log(this.state.sessionFlashcards)
	}

	componentDidMount() {
		document.querySelector('body')
			.classList.add(styles.overflow);

		//this.takeNextFlashcard();
	}

	componentWillUnmount() {
		document.querySelector('body')
			.classList.remove(styles.overflow);
	}

	render() {
		const { statistics, totalFlashcardsCount, currentFlashcard } = this.state;

		return (
			<div ref={ (ref) => this.overlay = ref } className={ styles.overlay } onClick={ this.handleOverlayClick }>

				<div className={ modalsClassNames.first } ref={ (ref) => this.firstModal = ref }>
					{ currentFlashcard && this.renderOpenedFlashcard() }
				</div>

				<div className={ modalsClassNames.second } ref={ (ref) => this.secondModal = ref }/>
				<div className={ modalsClassNames.third } ref={ (ref) => this.thirdModal = ref }/>
				<div className={ modalsClassNames.fourth }/>

			</div>
		)
	}
	handleResultsClick = (rate) => {

		this.setState({
			currentFlashcardId: this.state.currentFlashcardId+1,
		}, this.takeNextFlashcard);
	};




	handleOverlayClick = (e) => {
		const { onClose } = this.props;

		if (e.target === this.overlay) {
			onClose();
		}
	};

	takeNextFlashcard() {
		this.state.currentFlashcardId = this.state.currentFlashcardId % this.state.totalFlashcardsCount;
		this.state.currentFlashcard = this.state.sessionFlashcards[this.state.currentFlashcardId];
		this.animateCards();
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
	renderOpenedFlashcard() {
		const { currentFlashcard } = this.state;
		const { onClose } = this.props;
		return (
			<OpenedFlashcard
				onShowAnswer={ this.resetCardsAnimation }
				question={ currentFlashcard.question }
				answer={ currentFlashcard.answer }
				unitTitle={ currentFlashcard.unitTitle }
				theorySlides={ [] }
				onHandlingResultsClick={ this.handleResultsClick }
				onClose={ onClose }
			/>
		);
	}
	resetCardsAnimation = () => {
		const { firstModal, secondModal, thirdModal } = this;

		if (!firstModal) {
			return;
		}

		firstModal.className = modalsClassNames.first;
		secondModal.className = modalsClassNames.second;
		thirdModal.className = modalsClassNames.third;
	}


}
CourseToolFlashcards.propTypes = {
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

export default CourseToolFlashcards;