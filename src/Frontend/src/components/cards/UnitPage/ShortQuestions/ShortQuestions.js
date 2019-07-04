import React, {Component} from 'react';
import PropTypes from "prop-types";
import styles from './shortQuestions.less';
import Toggle from "@skbkontur/react-ui/Toggle";
import classNames from 'classnames';

class ShortQuestions extends Component {
	constructor(props) {
		super(props);
		this.state = {
			showAllAnswers: false,
			indexesOfAnswersToShow: []
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
		const {questions, answers} = this.props;
		const {showAllAnswers, indexesOfAnswersToShow} = this.state;
		const shouldAnswerBeRendered = (index) => showAllAnswers || indexesOfAnswersToShow.some(i => i === index);

		return (
			questions.map((question, index) =>
				<li
					className={styles.questionText}
					key={index}
					onClick={() => this.handleQuestionClick(index)}>
					<div>
						{question}
					</div>
					{
						shouldAnswerBeRendered(index) &&
						<div className={styles.answerText}>
							{answers[index]}
						</div>
					}
				</li>)
		);
	}

	handleQuestionClick = (i) => {
		const {indexesOfAnswersToShow} = this.state;
		const index = indexesOfAnswersToShow.indexOf(i);

		if (index !== -1) {
			indexesOfAnswersToShow.splice(index, 1);
		} else {
			indexesOfAnswersToShow.push(i);
		}

		this.setState({
			indexesOfAnswersToShow: indexesOfAnswersToShow
		})
	};

	handleToggleChange = () => {
		this.setState({
			showAllAnswers: !this.state.showAllAnswers,
			indexesOfAnswersToShow: []
		});
	}
}

ShortQuestions.propTypes = {
	questions: PropTypes.arrayOf(PropTypes.string).isRequired,
	answers: PropTypes.arrayOf(PropTypes.string).isRequired,
	className: PropTypes.string
};

export default ShortQuestions;
