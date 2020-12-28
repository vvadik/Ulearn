import React from "react";

import IControlWithText from "./IControlWithText";
import { Refresh } from "icons";

import styles from './Controls.less';

import texts from "../Exercise.texts";

interface Props extends IControlWithText {
	onResetButtonClicked: () => void,
}

function ResetButton({ showControlsText, onResetButtonClicked, }: Props): React.ReactElement {
	return (
		<span className={ styles.exerciseControls } onClick={ onResetButtonClicked }>
			<span className={ styles.exerciseControlsIcon }>
				<Refresh/>
			</span>
			{ showControlsText && texts.controls.reset.text }
		</span>
	);
}

export default ResetButton;
