import React, {Component} from 'react';
import PropTypes from "prop-types";
import styles from './flashcards.less';
import classNames from 'classnames';
import ProgressBar from "../ProgressBar/ProgressBar";
import FrontFlashcard from "./FrontFlashcard/FrontFlashcard";

const modalsStyles = {
	first: classNames(styles.modal),
	second: classNames(styles.modal, styles.secondModal),
	third: classNames(styles.modal, styles.thirdModal),
	fourth: classNames(styles.modal, styles.fourthModal)
};

class Flashcards extends Component {
	constructor(props) {
		super(props);
		const {flashcards, statistics} = this.props;

		this.state = {
			currentIndex: 0,
			statistics: {...statistics},
			flashcards: [...flashcards],
		}
	}

	componentDidMount() {
		document.getElementsByTagName('body')[0].classList.add(styles.overflow);
	}

	componentWillUnmount() {
		document.getElementsByTagName('body')[0].classList.remove(styles.overflow);
	}

	render() {
		const {flashcards, currentIndex, statistics} = this.state;
		const {totalFlashcardsCount} = this.props;
		const {question, answer, unitTitle} = flashcards[currentIndex];

		return (
			<div ref={(ref) => this.overlay = ref} className={styles.overlay} onClick={this.handleOverlayClick}>
				<div ref={(ref) => this.firstModal = ref} className={modalsStyles.first}>
					<FrontFlashcard
						onShowAnswer={() => this.resetCardsAnimation()}
						question={question}
						answer={answer}
						unitTitle={unitTitle}
						onHandlingResultsClick={this.handleResultsClick}
						onClose={this.props.onClose}
					/>
				</div>
				<div ref={(ref) => this.secondModal = ref} className={modalsStyles.second}/>
				<div ref={(ref) => this.thirdModal = ref} className={modalsStyles.third}/>
				<div className={modalsStyles.fourth}/>
				{Flashcards.renderControlGuides()}
				<div className={styles.progressBarContainer}>
					<ProgressBar statistics={statistics} totalFlashcardsCount={totalFlashcardsCount}/>
				</div>
			</div>
		)
	}

	handleOverlayClick = (e) => {
		const {onClose} = this.props;

		if (e.target === this.overlay) {
			onClose();
		}
	};

	static renderControlGuides() {
		return (
			<p className={styles.controlGuideContainer}>
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
		const {currentIndex, flashcards, statistics} = this.state;
		const {courseId, sendFlashcardRate} = this.props;
		const currentCard = flashcards[currentIndex];
		const newRate = `rate${rate}`;

		sendFlashcardRate(courseId, currentCard.id, newRate);

		statistics[currentCard.rate]--;
		statistics[newRate]++;
		currentCard.rate = newRate;

		this.setState({
			statistics: statistics
		});
		this.takeNextFlashcard();
	};

	takeNextFlashcard() {
		const {currentIndex, flashcards} = this.state;
		let newIndex = currentIndex + 1;

		console.log(newIndex, flashcards.length);
		if (newIndex === flashcards.length) {
			newIndex = 0;
		}

		this.setState({
			currentIndex: newIndex,
		});

		this.animateCards();
	}

	animateCards() {
		const {firstModal, secondModal, thirdModal} = this;
		const animationDuration = 700;

		firstModal.className = classNames(modalsStyles.second, styles.move);
		secondModal.className = classNames(modalsStyles.third, styles.move);
		thirdModal.className = classNames(modalsStyles.fourth, styles.move);

		setTimeout(() => {
			this.resetCardsAnimation();
		}, animationDuration - animationDuration / 10);
	}

	resetCardsAnimation() {
		const {firstModal, secondModal, thirdModal} = this;

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
	courseId: PropTypes.string,
	onClose: PropTypes.func,
	sendFlashcardRate: PropTypes.func,
};

export default Flashcards;
