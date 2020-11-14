import React from "react";

import PropTypes from 'prop-types';

import styles from "./CongratsModal.less"

import texts from "./CongratsModal.texts";
import CupIcon from "src/components/course/Course/Slide/Blocks/CodeMirror/CongratsModal/CupIcon";


class CongratsModal extends React.Component {
	componentDidMount() {
		document.querySelector('body')
			.classList.add(styles.bodyOverflow);
	}

	componentWillUnmount() {
		document.querySelector('body')
			.classList.remove(styles.bodyOverflow);
	}

	render() {
		const { waitingForManualChecking, score, showAcceptedSolutions, onClose, } = this.props;

		return (
			<div ref={ (ref) => this.overlay = ref }
				 className={ styles.overlay }
				 onClick={ this.handleCongratsOverlayClick }>
				<div className={ styles.modal }>
					<button className={ styles.modalCloseButton } onClick={ onClose }>
						&times;
					</button>
					<div className={ styles.iconWrapper }>
						{ <CupIcon/> }
						{ score > 0 && this.renderScore(score) }
					</div>
					<h2 className={ styles.title }>
						{ texts.title }
					</h2>
					<section className={ styles.text }>
						{ texts.getBodyText(waitingForManualChecking, score) }
					</section>

					{/* this.renderSelfCheckContent() */ }

					<div className={ styles.closeButtonWrapper }>
						<button className={ styles.closeButton }
								onClick={ showAcceptedSolutions ? this.onCloseClick : onClose }>
							{ showAcceptedSolutions ? texts.closeButtonForAcceptedSolutions : texts.closeButton }
						</button>
					</div>
				</div>
			</div>
		);
	}

	renderScore = (score) => {
		return (
			<div className={ styles.scoreTextWrapper }>+{ score }</div>
		);
	}

	onCloseClick = () => {
		const { onClose, showAcceptedSolutions, } = this.props;


		showAcceptedSolutions();
		onClose();
	}

	renderSelfCheckContent = () => {
		return (
			<React.Fragment>
				<div className={ styles.delimiter }/>
				<h2 className={ styles.title }>
					{ texts.selfCheckTitle }
				</h2>
				<section className={ styles.text }>
					{ texts.selfCheckContent }
				</section>
			</React.Fragment>
		);
	}

	handleCongratsOverlayClick = (e) => {
		if(e.target === this.overlay) {
			this.props.onClose();
		}
	}
}

CongratsModal.propTypes = {
	onClose: PropTypes.func.isRequired,
	waitingForManualChecking: PropTypes.bool,
	score: PropTypes.number,
	showAcceptedSolutions: PropTypes.func,
}

export default CongratsModal;