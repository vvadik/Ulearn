import React, { useState } from "react";

import cn from "classnames";

import { ThemeContext, Tooltip } from "ui";
import { Lightbulb } from "icons";
import IControlWithText from "./IControlWithText";
import { darkFlat } from "src/uiTheme.js";

import ShowControlsTextContext from "./ShowControlsTextContext";

import styles from './Controls.less';

import texts from "../Exercise.texts";

export interface Props extends IControlWithText {
	hints: string[],
	onAllHintsShowed: () => void,
}

function ShowHintButton({
	hints,
	onAllHintsShowed,
}: Props): React.ReactElement {
	const [{ showedHintsCount, isTooltipOpened }, setState] = useState(
		{ showedHintsCount: 1, isTooltipOpened: false, });

	return (
		<span className={ styles.exerciseControls } onClick={ showTooltip }>
			<span className={ styles.exerciseControlsIcon }>
				<Tooltip
					onCloseRequest={ closeTooltip }
					allowedPositions={ ["bottom left"] }
					pos={ "bottom left" }
					trigger={ isTooltipOpened ? "opened" : "closed" }
					render={ renderHints }
				>
					<Lightbulb/>
				</Tooltip>
			</span>
			<ShowControlsTextContext.Consumer>
			{
				(showControlsText) => showControlsText && texts.controls.hints.text
			}
			</ShowControlsTextContext.Consumer>
		</span>
	);

	function showTooltip() {
		setState({ showedHintsCount, isTooltipOpened: true });
	}

	function closeTooltip() {
		setState({ showedHintsCount, isTooltipOpened: false });
	}

	function renderHints() {
		const noHintsLeft = showedHintsCount === hints.length;
		const hintClassName = cn(styles.exerciseControls, { [styles.noHintsLeft]: noHintsLeft });

		return (
			<ul className={ styles.hintsWrapper }>
				{ hints.slice(0, showedHintsCount)
					.map((h, i) =>
						<li key={ i }>
							<span className={ styles.hintBulb }><Lightbulb/></span>
							{ h }
						</li>
					) }
				<ThemeContext.Provider value={ darkFlat }>
					<Tooltip
						pos={ "bottom left" }
						trigger={ "hover&focus" }
						render={ renderNoHintsLeft }
					>
						<a onClick={ showHint } className={ hintClassName }>
							<span>{ texts.controls.hints.showHintText }</span>
						</a>
					</Tooltip>
				</ThemeContext.Provider>
			</ul>
		);
	}

	function showHint(e: React.MouseEvent) {
		e.stopPropagation();
		setState({ showedHintsCount: Math.min(showedHintsCount + 1, hints.length), isTooltipOpened });
		if(showedHintsCount + 1 >= hints.length) {
			onAllHintsShowed();
		}
	}

	function renderNoHintsLeft() {
		const noHintsLeft = showedHintsCount === hints.length;

		return noHintsLeft ? <span>{ texts.controls.hints.hint }</span> : null;
	}
}

export default ShowHintButton;
