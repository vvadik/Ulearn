import React, { useState, } from "react";
import { Button, Switcher, Toggle, Gapped, } from "ui";

import styles from './ScoreControls.less';
import texts from './ScoreControls.texts';


const defaultScores = ['0', '25', '50', '75', '100'];

export interface Props {
	scores?: string[];
	exerciseTitle: string;
	prevReviewScore?: number | 0 | 25 | 50 | 75 | 100;
	toggleChecked?: boolean;

	onSubmit: (score: number) => void;
	onToggleChange: (value: boolean) => void;
}

interface State {
	score?: number;
	scoreSaved: boolean;
	toggleChecked: boolean;
}

function ScoreControls({
	scores = defaultScores,
	exerciseTitle,
	onSubmit,
	onToggleChange,
	prevReviewScore,
	toggleChecked,
}: Props): React.ReactElement {
	const [state, setState] = useState<State>({ scoreSaved: false, toggleChecked: !!toggleChecked });

	return (
		<Gapped gap={ 24 } vertical>
			{ state.scoreSaved && state.score !== undefined
				? renderControlsAfterSubmit(state.score)
				: renderControls(scores, state.score, prevReviewScore)
			}
			{ renderKeepReviewingToggle(state.toggleChecked) }
		</Gapped>
	);

	function renderControlsAfterSubmit(score: number) {
		return (
			<Gapped gap={ 16 } vertical={ false }>
				<span className={ styles.successLabel }>
					{ texts.getScoreText(score) }
				</span>
				<Button
					size={ "medium" }
					use={ "link" }
					onClick={ resetScore }
				>
					{ texts.changeScoreText }
				</Button>
			</Gapped>
		);
	}

	function renderControls(scores: string[], score?: number, prevReviewScore?: number,) {
		return (
			<Gapped gap={ 24 } vertical={ false } className={ styles.controlsWrapper }>
				{ renderSwitcherWithLastReviewMarker(scores, score, prevReviewScore,) }
				<Button
					size={ 'large' }
					disabled={ score === undefined }
					use={ 'primary' }
					onClick={ onSubmitClick }
				>
					{ texts.submitButtonText }
				</Button>
			</Gapped>
		);
	}

	function renderSwitcherWithLastReviewMarker(scores: string[], score?: number, prevReviewScore?: number,) {
		if(prevReviewScore === undefined) {
			return (
				<Switcher
					className={ styles.scoresLabel }
					label={ texts.scoresText }
					size={ "medium" }
					value={ score?.toString() }
					items={ scores }
					onValueChange={ onValueChange }/>
			);
		}
		/*
			injecting/changing something down here? check injectPrevReviewScore first or you can break something
		*/
		return (
			<span ref={ (ref) =>
				prevReviewScore !== undefined
				&& ref?.children[0]
				&& injectPrevReviewScore(ref, scores.findIndex(s => s === prevReviewScore.toString())) }>
				<Switcher
					className={ styles.scoresLabel }
					label={ texts.scoresText }
					size={ "medium" }
					value={ score?.toString() }
					items={ scores }
					onValueChange={ onValueChange }/>
				</span>
		);
	}

	//hardcoded function which injecting an last review span in parent span using absolute position
	//[1] parent span should not have any childs except switcher, or it will not render last review span
	//[2] switcher should be the first child of span
	//[3] switcher should have buttons with correct positions
	function injectPrevReviewScore(element: HTMLSpanElement, index: number) {
		if(element.children.length > 1) { //[1]
			return;
		}
		const buttons = element.children[0].getElementsByTagName('button'); //[2]
		const button = buttons[index];

		const lastReviewNode = document.createElement('span');
		lastReviewNode.className = styles.lastReviewWrapper;
		lastReviewNode.textContent = texts.lastReviewScoreText;

		element.appendChild(lastReviewNode);

		//[3]
		lastReviewNode.style.top = `${ button.offsetHeight + 6 }px`; // 6 = 2px borders + 4px margin
		lastReviewNode.style.left = `${ button.offsetLeft + (button.offsetWidth - lastReviewNode.offsetWidth) / 2 }px`;
	}

	function renderKeepReviewingToggle(toggleChecked: boolean,) {
		return (
			<Toggle checked={ toggleChecked } captionPosition={ "right" } onValueChange={ onToggleValueChange }>
				<span className={ styles.toggleLabel }>
					{ texts.getCodeReviewToggleText(exerciseTitle) }
				</span>
			</Toggle>
		);
	}

	function resetScore() {
		setState({
			...state,
			scoreSaved: false,
		});
	}

	function onValueChange(score: string) {
		setState({
			...state,
			score: parseInt(score),
		});
	}

	function onToggleValueChange(value: boolean) {
		onToggleChange(value);
		setState({
			...state,
			toggleChecked: value,
		});
	}

	function onSubmitClick() {
		if(state.score !== undefined) {
			onSubmit(state.score);
		}
		setState({
			...state,
			scoreSaved: true,
		});
	}
}

export default ScoreControls;
