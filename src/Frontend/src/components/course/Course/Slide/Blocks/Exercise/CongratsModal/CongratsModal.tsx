import React from "react";

import styles from "./CongratsModal.less";

import texts from "./CongratsModal.texts";
import CupIcon from "./CupIcon";

interface CongratsModalProps {
	onClose: () => void,
	waitingForManualChecking: boolean,
	score: number,
	showAcceptedSolutions: boolean,
}

class CongratsModal extends React.Component<CongratsModalProps> {
	overlay: HTMLDivElement | null | undefined;

	componentDidMount(): void {
		document.querySelector('body')
			?.classList.add(styles.bodyOverflow);
	}

	componentWillUnmount(): void {
		document.querySelector('body')
			?.classList.remove(styles.bodyOverflow);
	}

	render(): React.ReactNode {
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
						{ texts.getBodyText(waitingForManualChecking, showAcceptedSolutions, score) }
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

	private renderScore = (score: number) => {
		return (
			<div className={ styles.scoreTextWrapper }>+{ score }</div>
		);
	};

	private onCloseClick = () => {
		const { onClose, } = this.props;
		onClose();
	};

	private renderSelfCheckContent = () => {
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
	};

	private handleCongratsOverlayClick = (e: React.MouseEvent<HTMLDivElement, MouseEvent>) => {
		if(e.target === this.overlay) {
			this.props.onClose();
		}
	};
}

export { CongratsModal, CongratsModalProps, };
