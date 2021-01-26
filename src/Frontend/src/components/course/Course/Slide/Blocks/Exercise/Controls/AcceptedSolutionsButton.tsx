import React, { createRef, RefObject } from "react";

import { Button, Modal, Tooltip } from "ui";
import { EyeOpened } from "icons";
import DownloadedHtmlContent from "src/components/common/DownloadedHtmlContent.js";
import IControlWithText from "./IControlWithText";

import ShowControlsTextContext from "./ShowControlsTextContext";

import styles from './Controls.less';

import texts from "../Exercise.texts";


interface State {
	showAcceptedSolutions: boolean,
}

export interface Props extends IControlWithText {
	acceptedSolutionsUrl: string,
	isShowAcceptedSolutionsAvailable: boolean,

	onVisitAcceptedSolutions: () => void,
}

export default class AcceptedSolutionsButton
	extends React.Component<Props, State> {
	private tooltip: RefObject<Tooltip> = createRef();

	render = (): React.ReactNode => {
		return (
			<React.Fragment>
				<span className={ styles.exerciseControls } onClick={ this.showAcceptedSolutionsWarning }>
					<Tooltip
						ref={ this.tooltip }
						pos={ "bottom left" }
						trigger={ 'click' }
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
			</React.Fragment>
		);
	};

	showAcceptedSolutionsWarning = (): void => {
		const { isShowAcceptedSolutionsAvailable } = this.props;

		if(isShowAcceptedSolutionsAvailable) {
			this.showAcceptedSolutions();
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

	showAcceptedSolutions = (e?: React.MouseEvent<HTMLButtonElement, MouseEvent>): void => {
		const { onVisitAcceptedSolutions, } = this.props;

		onVisitAcceptedSolutions();

		if(e) {
			e.stopPropagation();
		}

		this.tooltip.current?.hide();
	};
}
