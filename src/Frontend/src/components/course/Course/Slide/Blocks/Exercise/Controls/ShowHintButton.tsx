import React from "react";

import classNames from "classnames";

import { Tooltip } from "ui";
import { Lightbulb } from "icons";
import IControlWithText from "./IControlWithText";

import styles from './Controls.less';

import texts from "../Exercise.texts";

interface Props extends IControlWithText {
	countOfHints: number,
	showedHintsCount: number,

	showHint: () => void,
}

function ShowHintButton({
	countOfHints,
	showHint,
	showedHintsCount,
	showControlsText
}: Props): React.ReactElement {
	const noHintsLeft = showedHintsCount === countOfHints;
	const hintClassName = classNames(styles.exerciseControls, { [styles.noHintsLeft]: noHintsLeft });

	return (
		<span className={ hintClassName } onClick={ showHint }>
			<Tooltip
				pos={ "bottom center" }
				trigger={ "hover&focus" }
				render={ () => noHintsLeft ? <span>{ texts.controls.hints.hint }</span> : null }
			>
			<span className={ styles.exerciseControlsIcon }>
				<Lightbulb/>
			</span>
				{ showControlsText && texts.controls.hints.text }
			</Tooltip>
		</span>
	);
}

export default ShowHintButton;
