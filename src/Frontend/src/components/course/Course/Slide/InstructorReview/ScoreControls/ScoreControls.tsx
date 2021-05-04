import React, { useState, } from "react";
import { Button, Switcher, Toggle, Gapped, } from "ui";

import styles from './ScoreControls.less';
import texts from './ScoreControls.texts';


const defaultScores = ['0', '25', '50', '75', '100'];

export interface Props {
	scores?: string[];
	exerciseTitle: string;
	prevReviewScore?: number;

	onSubmit: (score: number) => void;
	onToggleChange: (value: boolean) => void;
}

interface State {
	score?: number;
	scoreSaved: boolean;
}

function ScoreControls({
	scores = defaultScores,
	exerciseTitle,
	onSubmit,
	onToggleChange,
	prevReviewScore,
}: Props): React.ReactElement {
	const [state, setState] = useState<State>({ scoreSaved: false, });

	return (
		<Gapped gap={ 24 } vertical>
			{ state.scoreSaved && state.score
				? renderControlsAfterSubmit(state.score)
				: renderControls(scores, state.score, prevReviewScore)
			}
			{ renderKeepReviewingToggle() }
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
			<Gapped gap={ 24 } vertical={ false }>
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
		if(!prevReviewScore) {
			return (
				<Switcher
					className={ styles.scoresLabel }
					label={ texts.scoresText }
					size={ "large" }
					value={ score?.toString() }
					items={ scores }
					onValueChange={ onValueChange }/>
			);
		}
		/*
			injecting/changing something down here? check injectPrevReviewScore first or you can break something
		*/
		return (
			<span ref={ (ref) => prevReviewScore
				&& ref?.children[0]
				&& injectPrevReviewScore(ref, scores.findIndex(s => s === prevReviewScore.toString())) }>
				<Switcher
					className={ styles.scoresLabel }
					label={ texts.scoresText }
					size={ "large" }
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

		const position = buttons[index].getBoundingClientRect(); //[3]

		const lastReviewNode = document.createElement('span');
		lastReviewNode.className = styles.lastReviewWrapper;
		lastReviewNode.textContent = texts.lastReviewScoreText;
		lastReviewNode.style.top = `${ (position.top + position.height) }px`;
		lastReviewNode.style.left = `${ position.left }px`;

		element.appendChild(lastReviewNode);
	}

	function renderKeepReviewingToggle() {
		return (
			<Toggle captionPosition={ "right" } onValueChange={ onToggleValueChange }>
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
	}

	function onSubmitClick() {
		if(state.score) {
			onSubmit(state.score);
		}
		setState({
			...state,
			scoreSaved: true,
		});
	}
}

export default ScoreControls;
