import React, {Component} from 'react';
import PropTypes from "prop-types";
import styles from './shortQuestions.less';
import Toggle from "@skbkontur/react-ui/Toggle";

class ShortQuestions extends Component {
	constructor(props) {
		super(props);
		this.state = {
			showAnswers: false
		};
	}

	render() {
		return (
			<div className={styles.questionsContainer}>
				<ol className={styles.questionsTextContainer}>
					{this.renderQuestions()}
				</ol>
				<div className={styles.toggleContainer}>
					<Toggle onChange={this.handleToggleChange} checked={this.state.showAnswers}/>
					<span className={styles.toggleText}>
						Показать ответы
					</span>
				</div>
			</div>
		)
	}

	renderQuestions() {
		const {questions, answers} = this.props;

		return (
			questions.map((question, index) =>
				<li className={styles.questionText} key={index}>
					<div>
						{question}
					</div>
					{
						this.state.showAnswers &&
						<div className={styles.answer}>
							{answers[index]}
						</div>
					}
				</li>)
		);
	}

	handleToggleChange = () => {
		this.setState({
			showAnswers: !this.state.showAnswers
		});
	}


}

ShortQuestions.propTypes = {
	questions: PropTypes.arrayOf(PropTypes.string).isRequired,
	answers: PropTypes.arrayOf(PropTypes.string).isRequired
};

export default ShortQuestions;
