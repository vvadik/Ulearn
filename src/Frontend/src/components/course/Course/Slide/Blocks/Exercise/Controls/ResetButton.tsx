import React from "react";

import IControlWithText from "./IControlWithText";
import { Refresh } from "icons";

import ShowControlsTextContext from "./ShowControlsTextContext";

import styles from './Controls.less';

import texts from "../Exercise.texts";

export interface Props extends IControlWithText {
	onResetButtonClicked: () => void,
}

function ResetButton({ onResetButtonClicked, }: Props): React.ReactElement {
	return (
		<span className={ styles.exerciseControls } onClick={ onResetButtonClicked }>
			<span className={ styles.exerciseControlsIcon }>
				<Refresh/>
			</span>
			<ShowControlsTextContext.Consumer>
			{
				(showControlsText) => showControlsText && texts.controls.reset.text
			}
			</ShowControlsTextContext.Consumer>
	</span>
	);
}

export default ResetButton;
