import React, { Component } from "react";
import PropTypes from "prop-types";

import Button from "@skbkontur/react-ui/Button";
import Results from "../Results/Results";

import styles from "../flashcards.less";

import translateCode from "../../../../codeTranslator/translateCode";


class FrontFlashcard extends Component {
	constructor(props) {
		super(props);

		this.state = {
			showAnswer: false,
		}
	}

	componentDidMount() {
		document.addEventListener('keyup', this.handleKeyUp);
		translateCode(this.modal);
	}

	componentWillUnmount() {
		document.removeEventListener('keyup', this.handleKeyUp);
	}

	componentDidUpdate(prevProps, prevState, snapshot) {
		translateCode(this.modal);
	}

	handleKeyUp = (e) => {
		const { onClose } = this.props;
		const { showAnswer } = this.state;
		const code = e.key;
		const spaceChar = ' ';

		if (code === spaceChar) {
			this.showAnswer();
		} else if (code >= 1 && code <= 5 && showAnswer) {
			this.handleResultsClick(code);
		} else if (code === 'Escape') {
			onClose();
		}
	};

	handleResultsClick = (rate) => {
		const { onHandlingResultsClick } = this.props;

		onHandlingResultsClick(rate);

		this.setState({
			showAnswer: false,
		})
	};

	render() {
		const { unitTitle, question, answer, onClose } = this.props;
		const { showAnswer } = this.state;

		return (
			<div ref={ ref => this.modal = ref }>
				<button tabIndex={ 1 } className={ styles.closeButton } onClick={ onClose }>
					&times;
				</button>
				<h5 className={ styles.unitTitle }>
					{ unitTitle }
				</h5>
				{ showAnswer ? this.renderBackFlashcard(question, answer) : this.renderFrontFlashcard(question) }
			</div>
		);
	}

	renderFrontFlashcard(question) {
		return (
			<div className={ styles.frontTextContainer }>
				<div className={ styles.questionFront }
					 dangerouslySetInnerHTML={ { __html: question } }/>
				<div className={ styles.showAnswerButtonContainer }>
					<Button size='large' use='primary' onClick={ () => this.showAnswer() }>
						Показать ответ
					</Button>
				</div>
			</div>
		);
	}

	showAnswer() {
		this.props.onShowAnswer();
		this.setState({
			showAnswer: true
		});
	}

	renderBackFlashcard(question, answer) {
		return (
			<div>
				<div className={ styles.backTextContainer }>
					<div className={ styles.questionBack } dangerouslySetInnerHTML={ { __html: question } }/>
					<div dangerouslySetInnerHTML={ { __html: answer } }/>
				</div>
				<Results handleClick={ this.handleResultsClick }/>
			</div>
		);
	}
}

FrontFlashcard.propTypes = {
	question: PropTypes.string,
	answer: PropTypes.string,
	unitTitle: PropTypes.string,
	onClose: PropTypes.func,
	onShowAnswer: PropTypes.func,
	onHandlingResultsClick: PropTypes.func,
};

export default FrontFlashcard;
