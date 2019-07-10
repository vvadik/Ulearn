import React, {Component} from 'react';
import PropTypes from "prop-types";
import styles from './flashcards.less';
import classNames from 'classnames';
import Button from "@skbkontur/react-ui/Button";
import Results from "./Results/Results";
import ProgressBar from "../ProgressBar/ProgressBar";

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
		const statistics = {...defaultStatistics};

		for (const card of flashcards) {
			statistics[card.rate]++;
		}

		this.state = {
			showAnswer: false,
			currentIndex: 0,
			statistics: statistics,
			flashcards: flashcards
		}
	}

	render() {
		const {flashcards, showAnswer, currentIndex, statistics} = this.state;

		return (
			<div className={styles.overlay}>
				<div id={'modal'} className={modalsStyles.first}>
					{this.renderFlashcard(flashcards[currentIndex], showAnswer)}
				</div>
				<div id={'secondModal'} className={modalsStyles.second}/>
				<div id={'thirdModal'} className={modalsStyles.third}/>
				<div id={'fourthModal'} className={modalsStyles.fourth}/>
				{Flashcards.renderControlGuides()}
				<div className={styles.progressBarContainer}>
					<ProgressBar statistics={statistics} totalFlashcardsCount={flashcards.length}/>
				</div>
			</div>
		)
	}

	componentDidMount() {
		document.addEventListener('keypress', this.handleKeyPress);
	}

	handleKeyPress = (e) => {
		const code = e.key;
		const spaceChar = ' ';

		if (code === spaceChar) {
			this.showAnswer();
		} else if (code >= 1 && code <= 5 && this.state.showAnswer) {
			this.handleResultsClick(code);
		}
	};

	componentWillUnmount() {
		document.removeEventListener('keypress', this.handleKeyPress);
	}

	renderFlashcard({unitTitle = '', question = '', answer = ''}, showAnswer) {
		return (
			<div>
				<button tabIndex={1} className={styles.closeButton}>
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
				<h4 className={styles.questionFront}>
					{question}
				</h4>
				<div className={styles.showAnswerButtonContainer}>
					<Button size='large' use='primary' onClick={() => this.showAnswer()}>
						Показать ответ
					</Button>
				</div>
			</div>
		);
	}

	showAnswer() {
		Flashcards.resetAnimation();
		this.setState({
			showAnswer: true
		});
	}

	renderBackFlashcard(question, answer) {
		return (
			<div>
				<div className={styles.backTextContainer}>
					<p className={styles.questionBack}>
						{question}
					</p>
					{answer}
				</div>
				<Results handleClick={this.handleResultsClick}/>
			</div>
		);
	}

	static renderControlGuides() {
		return (
			<div className={styles.controlGuideContainer}>
				Используйте клавиатуру:
				<span>пробел</span> — показать ответ,
				<span>1</span>
				<span>2</span>
				<span>3</span>
				<span>4</span>
				<span>5</span> — поставить оценку
			</div>
		);
	}

	handleResultsClick = (rate) => {
		const {currentIndex, flashcards, statistics} = this.state;
		const currentCard = flashcards[currentIndex];
		const newRate = `rate${rate}`;

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
		const animationDuration = 700;
		const first = document.getElementById('modal');
		const second = document.getElementById('secondModal');
		const third = document.getElementById('thirdModal');

		first.className = classNames(modalsStyles.second, styles.move);
		second.className = classNames(modalsStyles.third, styles.move);
		third.className = classNames(modalsStyles.fourth, styles.move);

		setTimeout(() => {
			Flashcards.resetAnimation();
		}, animationDuration - animationDuration / 10);
	}

	static resetAnimation() {
		const first = document.getElementById('modal');
		const second = document.getElementById('secondModal');
		const third = document.getElementById('thirdModal');

		first.className = classNames(modalsStyles.first);
		second.className = classNames(modalsStyles.second);
		third.className = classNames(modalsStyles.third);
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
	})).isRequired
};

export default Flashcards;
