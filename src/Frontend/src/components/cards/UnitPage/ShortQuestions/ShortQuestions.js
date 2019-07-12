import React, {Component} from 'react';
import PropTypes from "prop-types";
import styles from './shortQuestions.less';
import Toggle from "@skbkontur/react-ui/Toggle";

class ShortQuestions extends Component {
	constructor(props) {
		super(props);

		this.state = {
			showAllAnswers: false,
			questionsWithAnswers: this.props.questionsWithAnswers
		};
	}

	render() {
		const {showAllAnswers} = this.state;

		return (
			<div className={styles.questionsContainer}>
				{this.renderQuestions()}
				<div className={styles.toggleContainer}>
					<Toggle onChange={this.handleToggleChange} checked={showAllAnswers}/>
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
			<ol className={styles.questionsTextContainer}>
				{
					questionsWithAnswers.map(({question, answer, showAnswer}, index) =>
						<li
							className={styles.questionText}
							key={index}
							onClick={() => this.handleQuestionClick(index)}>
							{question}
							{
								(showAllAnswers || showAnswer) &&
								<p className={styles.answerText}>
									{answer}
								</p>
							}
						</li>)
				}
			</ol>);
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

		for (const question of questionsWithAnswers) {
			question.showAnswer = false;
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
