import React, { Component } from "react";
import PropTypes from "prop-types";

import Button from "@skbkontur/react-ui/Button";
import Results from "../Results/Results";

import styles from "./frontFlashcard.less";

import translateCode from "../../../../codeTranslator/translateCode";
import Link from "react-router-dom/es/Link";


class FrontFlashcard extends Component {
	constructor(props) {
		super(props);

		this.state = {
			showAnswer: false,
		}
	}

	componentDidMount() {
		document.addEventListener('keyup', this.handleKeyUp);
		translateCode(this.modal, { codeMirror: { withMarginAuto: true } });
	}

	componentWillUnmount() {
		document.removeEventListener('keyup', this.handleKeyUp);
	}

	componentDidUpdate(prevProps, prevState, snapshot) {
		translateCode(this.modal, { codeMirror: { withMarginAuto: true } });
	}

	handleKeyUp = (e) => {
		const { onClose } = this.props;
		const { showAnswer } = this.state;
		const code = e.key;
		const spaceChar = ' ';
		const escapeChar = 'Escape';

		if (code === spaceChar) {
			this.showAnswer();
		} else if (code >= 1 && code <= 5 && showAnswer) {
			this.handleResultsClick(code);
		} else if (code === escapeChar) {
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

		let content, control;

		if (showAnswer) {
			content = FrontFlashcard.renderBackContent(question, answer, this.renderLinksToTheorySlides());
			control = this.renderBackControl();
		} else {
			content = FrontFlashcard.renderFrontContent(question);
			control = this.renderFrontControl();
		}

		return (
			<div className={ styles.wrapper } ref={ ref => this.modal = ref }>

				<button tabIndex={ 1 } className={ styles.closeButton } onClick={ onClose }>
					&times;
				</button>
				<h5 className={ styles.unitTitle }>
					{ unitTitle }
				</h5>
				{ content }
				{ control }
			</div>
		);
	}

	renderLinksToTheorySlides() {
		const { theorySlides } = this.props;
		const slidesCount = theorySlides.length;

		if (slidesCount === 0) {
			return;
		}

		const sourcePluralForm = slidesCount > 1
			? 'Источники'
			: 'Источник';

		return (
			<p className={ styles.theoryLinks }>{ sourcePluralForm }: {
				theorySlides.map(({ slug, title }, index) =>
					<Link to={ slug }
						  key={ slug }>
						Слайд «{ title }»{ index < slidesCount - 1 && ', ' }
					</Link>) }
			</p>)
	}

	static renderFrontContent(question) {
		return (
			<div className={ styles.questionFront }
				 dangerouslySetInnerHTML={ { __html: question } }
			/>
		);
	}

	renderFrontControl() {
		return (
			<div className={ styles.showAnswerButtonContainer }>
				<Button size='large' use='primary' onClick={ () => this.showAnswer() }>
					Показать ответ
				</Button>
			</div>
		);
	}

	showAnswer() {
		this.props.onShowAnswer();

		this.setState({
			showAnswer: true
		});
	}

	static renderBackContent(question, answer, linksToTheorySlides) {
		return (
			<div className={ styles.backTextContainer }>
				<div className={ styles.questionBack } dangerouslySetInnerHTML={ { __html: question } }/>
				<div dangerouslySetInnerHTML={ { __html: answer } }/>
				{ linksToTheorySlides }
			</div>
		);
	}

	renderBackControl() {
		return (
			<Results handleClick={ this.handleResultsClick }/>
		);
	}
}

FrontFlashcard.propTypes = {
	question: PropTypes.string,
	answer: PropTypes.string,
	unitTitle: PropTypes.string,
	theorySlides: PropTypes.arrayOf(
		PropTypes.shape({
			slug: PropTypes.string,
			title: PropTypes.string,
		}),
	),

	onClose: PropTypes.func,
	onShowAnswer: PropTypes.func,
	onHandlingResultsClick: PropTypes.func,
};

export default FrontFlashcard;
