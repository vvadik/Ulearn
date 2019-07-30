import React, { Component } from 'react';
import PropTypes from "prop-types";

import Toggle from "@skbkontur/react-ui/Toggle";

import translateCode from "../../../../codeTranslator/translateCode";

import styles from './shortQuestions.less';

class ShortQuestions extends Component {
	constructor(props) {
		super(props);
		const { questionsWithAnswers } = this.props;

		this.state = {
			showAllAnswers: false,
			questionsWithAnswers: [...questionsWithAnswers],
		};
	}

	componentDidMount() {
		translateCode(this.list);
	}

	componentDidUpdate(prevProps, prevState, snapshot) {
		const { questionsWithAnswers } = this.props;

		translateCode(this.list);


		if (this.state.questionsWithAnswers.length !== questionsWithAnswers.length) {
			this.setState({ questionsWithAnswers, });
		}
	}

	render() {
		const { showAllAnswers } = this.state;

		return (
			<div className={ styles.questionsContainer }>
				{ this.renderQuestions() }
				<div className={ styles.toggleContainer }>
					<Toggle onChange={ this.handleToggleChange } checked={ showAllAnswers }/>
					<span className={ styles.toggleText }>
						Показать ответы
					</span>
				</div>
			</div>
		)
	}

	renderQuestions() {
		const { showAllAnswers, questionsWithAnswers } = this.state;

		return (
			<ol ref={ (ref) => this.list = ref } className={ styles.questionsTextContainer }>
				{ questionsWithAnswers.map(({ question, answer, showAnswer, }, index) =>
					<li className={ styles.listElement }
						key={ index }
						onClick={ () => this.handleQuestionClick(index) }>
						<div dangerouslySetInnerHTML={ { __html: question } }/>
						{
							(showAllAnswers || showAnswer) &&
							<div className={ styles.answerText } dangerouslySetInnerHTML={ { __html: answer } }/>
						}
					</li>)
				}
			</ol>);
	}


	handleQuestionClick = (questionIndex) => {
		const { questionsWithAnswers } = this.state;
		const newQuestionsWithAnswers = [...questionsWithAnswers];
		const questionWithAnswer = newQuestionsWithAnswers[questionIndex];

		questionWithAnswer.showAnswer = !questionWithAnswer.showAnswer;

		this.setState({
			questionsWithAnswers: newQuestionsWithAnswers,
		})
	};

	handleToggleChange = () => {
		const { questionsWithAnswers } = this.state;

		for (const question of questionsWithAnswers) {
			question.showAnswer = false;
		}

		this.setState({
			showAllAnswers: !this.state.showAllAnswers,
			questionsWithAnswers: questionsWithAnswers,
		});
	}
}

ShortQuestions.propTypes = {
	questionsWithAnswers: PropTypes.arrayOf(PropTypes.shape({
		question: PropTypes.string,
		answer: PropTypes.string,
	})),
	className: PropTypes.string,
};

export default ShortQuestions;
