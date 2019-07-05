import React, {Component} from 'react';
import PropTypes from "prop-types";
import styles from './shortQuestions.less';
import Toggle from "@skbkontur/react-ui/Toggle";
import classNames from 'classnames';

class ShortQuestions extends Component {
	constructor(props) {
		super(props);

		const questionsWithAnswers = this.props.questionsWithAnswers
			.map((questionWithAnswer) => {
				return {
					...questionWithAnswer,
					showAnswer: false
				}
			});

		this.state = {
			showAllAnswers: false,
			questionsWithAnswers: questionsWithAnswers
		};
	}

	render() {
		const shortQuestionsStyle = classNames(this.props.className, styles.questionsContainer);

		return (
			<div className={shortQuestionsStyle}>
				<ol className={styles.questionsTextContainer}>
					{this.renderQuestions()}
				</ol>
				<div className={styles.toggleContainer}>
					<Toggle onChange={this.handleToggleChange} checked={this.state.showAllAnswers}/>
					<span className={styles.toggleText}>
						Показать ответы
					</span>
				</div>
			</div>
		)
	}

	renderQuestions() {
		const {showAllAnswers, questionsWithAnswers} = this.state;

		return (
			questionsWithAnswers.map(({question, answer, showAnswer}, index) =>
				<li
					className={styles.questionText}
					key={index}
					onClick={() => this.handleQuestionClick(index)}>
					<div>
						{question}
					</div>
					{
						(showAllAnswers || showAnswer) &&
						<div className={styles.answerText}>
							{answer}
						</div>
					}
				</li>)
		);
	}


	handleQuestionClick = (questionIndex) => {
		const {questionsWithAnswers} = this.state;
		const questionWithAnswer = questionsWithAnswers[questionIndex];

		questionWithAnswer.showAnswer = !questionWithAnswer.showAnswer;

		this.setState({
			questionsWithAnswers: questionsWithAnswers
		})
	};

	handleToggleChange = () => {
		const {questionsWithAnswers} = this.state;

		for (let questionElement of questionsWithAnswers) {
			questionElement.showAnswer = false;
		}

		this.setState({
			showAllAnswers: !this.state.showAllAnswers,
			questionsWithAnswers: questionsWithAnswers
		});
	}
}

ShortQuestions.propTypes = {
	questionsWithAnswers: PropTypes.arrayOf(PropTypes.shape({
		question: PropTypes.string,
		answer: PropTypes.string
	})).isRequired,
	className: PropTypes.string
};

export default ShortQuestions;
