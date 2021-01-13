import React from "react";

import { Button, Modal, Tooltip } from "ui";
import { EyeOpened } from "icons";
import ShowAfterDelay from "src/components/ShowAfterDelay/ShowAfterDelay";
import DownloadedHtmlContent from "src/components/common/DownloadedHtmlContent.js";
import IControlWithText from "./IControlWithText";

import ShowControlsTextContext from "./ShowControlsTextContext";

import styles from './Controls.less';

import texts from "../Exercise.texts";


interface State {
	showAcceptedSolutionsWarning: boolean,
	showAcceptedSolutions: boolean,
}

export interface Props extends IControlWithText {
	acceptedSolutionsUrl: string,
	isShowAcceptedSolutionsAvailable: boolean,

	onVisitAcceptedSolutions: () => void,
}

export default class AcceptedSolutionsButton
	extends React.Component<Props, State> {
	state = {
		showAcceptedSolutionsWarning: false,
		showAcceptedSolutions: false,
	};

	render = (): React.ReactNode => {
		const { showAcceptedSolutionsWarning, showAcceptedSolutions, } = this.state;

		return (
			<React.Fragment>
				<span className={ styles.exerciseControls } onClick={ this.showAcceptedSolutionsWarning }>
					<Tooltip
						onCloseClick={ this.hideAcceptedSolutionsWarning }
						pos={ "bottom left" }
						trigger={ showAcceptedSolutionsWarning ? "opened" : "closed" }
						render={ this.renderAcceptedSolutionsHint }>
						<span className={ styles.exerciseControlsIcon }>
							<EyeOpened/>
						</span>
						<ShowControlsTextContext.Consumer>
						{
							(showControlsText) => showControlsText && texts.controls.acceptedSolutions.text
						}
						</ShowControlsTextContext.Consumer>
					</Tooltip>
				</span>
				{ showAcceptedSolutions && this.renderAcceptedSolutions() }
			</React.Fragment>
		);
	};

	showAcceptedSolutionsWarning = (): void => {
		const { isShowAcceptedSolutionsAvailable } = this.props;

		if(isShowAcceptedSolutionsAvailable) {
			this.showAcceptedSolutions();
		} else {
			this.setState({
				showAcceptedSolutionsWarning: true,
			});
		}
	};

	renderAcceptedSolutionsHint = (): React.ReactNode => {
		return (
			<span>
				{ texts.controls.acceptedSolutions.buildWarning() }
				<Button use={ "danger" } onClick={ this.showAcceptedSolutions }>
					{ texts.controls.acceptedSolutions.continue }
				</Button>
			</span>);
	};

	hideAcceptedSolutionsWarning = (): void => {
		this.setState({
			showAcceptedSolutionsWarning: false,
		});
	};

	showAcceptedSolutions = (e?: React.MouseEvent<HTMLButtonElement, MouseEvent>): void => {
		const { onVisitAcceptedSolutions, } = this.props;

		onVisitAcceptedSolutions();

		this.setState({
			showAcceptedSolutions: true,
		});

		if(e) {
			e.stopPropagation();
		}

		this.hideAcceptedSolutionsWarning();
	};

	closeAcceptedSolutions = (): void => {
		this.setState({
			showAcceptedSolutions: false,
		});
	};

	renderAcceptedSolutions = (): React.ReactNode => {
		const { acceptedSolutionsUrl, } = this.props;

		return (
			<ShowAfterDelay>
				<Modal onClose={ this.closeAcceptedSolutions }>
					<Modal.Header>
						{ texts.acceptedSolutions.title }
					</Modal.Header>
					<Modal.Body>
						{ texts.acceptedSolutions.content }
						<DownloadedHtmlContent url={ acceptedSolutionsUrl }/>
					</Modal.Body>
				</Modal>
			</ShowAfterDelay>
		);
	};
}
