import React, {Component} from 'react';
import PropTypes from "prop-types";
import styles from './flashcards.less';
import classNames from 'classnames';
import Button from "@skbkontur/react-ui/Button";
import Results from "./Results/Results";
import ProgressBar from "../ProgressBar/ProgressBar";
import translateTextareaToCode from "../../../codemirror/codemirror";

const modalsStyles = {
	first: classNames(styles.modal),
	second: classNames(styles.modal, styles.secondModal),
	third: classNames(styles.modal, styles.thirdModal),
	fourth: classNames(styles.modal, styles.fourthModal)
};

const defaultStatistics = {
	notRated: 0,
	rate1: 0,
	rate2: 0,
	rate3: 0,
	rate4: 0,
	rate5: 0
};

class Flashcards extends Component {
	constructor(props) {
		super(props);
		const {flashcards} = this.props;
		const statistics = Flashcards.countStatistics(flashcards);

		this.state = {
			showAnswer: false,
			currentIndex: 0,
			statistics: statistics,
			flashcards: flashcards,
		}
	}

	componentDidMount() {
		document.getElementsByTagName('body')[0].classList.add(styles.overflow);
		document.addEventListener('keyup', this.handleKeyUp);
		this.representTextAsCode();
	}

	static countStatistics(flashcards) {
		const statistics = {...defaultStatistics};

		for (const card of flashcards) {
			statistics[card.rate]++;
		}

		return statistics;
	}

	componentWillUnmount() {
		document.removeEventListener('keyup', this.handleKeyUp);
		document.getElementsByTagName('body')[0].classList.remove(styles.overflow);
	}

	componentDidUpdate(prevProps, prevState, snapshot) {
		const {showAnswer} = this.state;

		if (prevState.showAnswer === showAnswer) {
			return;
		}

		this.representTextAsCode();
		//<textarea class="code code-sample" data-lang="csharp" data-code="double.Parse"></textarea>
	}

	representTextAsCode() {
		for (const textarea of this.firstModal.querySelectorAll('textarea')) {
			translateTextareaToCode(textarea, {readOnly: true});
		}
	}

	render() {
		const {flashcards, showAnswer, currentIndex, statistics} = this.state;
		const totalFlashcardsCount = flashcards.length;
		const flashcard = flashcards[currentIndex];

		return (
			<div ref={(ref) => this.overlay = ref} className={styles.overlay} onClick={this.handleOverlayClick}>
				<div ref={(ref) => this.firstModal = ref} className={modalsStyles.first}>
					{flashcard && this.renderFlashcard(flashcard, showAnswer)}
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
		if (e.target === this.overlay) {
			this.props.onClose();
		}
	};

	handleKeyUp = (e) => {
		const code = e.key;
		const spaceChar = ' ';

		if (code === spaceChar) {
			this.showAnswer();
		} else if (code >= 1 && code <= 5 && this.state.showAnswer) {
			this.handleResultsClick(code);
		} else if (code === 'Escape') {
			this.props.onClose();
		}
	};

	renderFlashcard({unitTitle, question, answer}, showAnswer) {
		return (
			<div>
				<button tabIndex={1} className={styles.closeButton} onClick={this.props.onClose}>
					&times;
				</button>
				<h5 className={styles.unitTitle}>
					{unitTitle}
				</h5>
				{!showAnswer && this.renderFrontFlashcard(question)}
				{showAnswer && this.renderBackFlashcard(question, answer)}
			</div>
		);
	}

	renderFrontFlashcard(question) {
		return (
			<div className={styles.frontTextContainer}>
				<div className={styles.questionFront}
					 dangerouslySetInnerHTML={{__html: question}}/>
				<div className={styles.showAnswerButtonContainer}>
					<Button size='large' use='primary' onClick={() => this.showAnswer()}>
						Показать ответ
					</Button>
				</div>
			</div>
		);
	}

	showAnswer() {
		this.resetCardsAnimation();
		this.setState({
			showAnswer: true
		});
	}

	renderBackFlashcard(question, answer) {
		return (
			<div>
				<div className={styles.backTextContainer}>
					<div className={styles.questionBack}
						 dangerouslySetInnerHTML={{__html: question}}/>
					<div dangerouslySetInnerHTML={{__html: answer}}/>
				</div>
				<Results handleClick={this.handleResultsClick}/>
			</div>
		);
	}

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
		let {currentIndex, flashcards} = this.state;

		currentIndex++;

		if (currentIndex === flashcards.length) {
			currentIndex = 0;
		}

		this.setState({
			currentIndex: currentIndex,
			showAnswer: false
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
	courseId: PropTypes.string,
	onClose: PropTypes.func,
	sendFlashcardRate: PropTypes.func,
};

export default Flashcards;
