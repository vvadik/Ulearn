import RunButton from "./RunButton";
import React, { MouseEventHandler } from "react";
import StepButton from "./StepButton";
import texts from './Visualizer.texts';

interface Props {
	run: MouseEventHandler;
	next: MouseEventHandler;
	previous: MouseEventHandler;

	currentStep: number;
	totalSteps: number;
}

class Controls extends React.Component<Props> {
	render() : React.ReactElement {
		return (
			<div>
				<RunButton onClick={ this.props.run } text={ texts.controls.run } />
				<StepButton
					onClick={ this.props.previous }
					disabled={ this.props.currentStep === 0 }
					text={ texts.controls.back }
				/>
				<StepButton
					onClick={ this.props.next }
					disabled={ this.props.currentStep === this.props.totalSteps - 1 }
					text={ texts.controls.next }
				/>
			</div>
		);
	}
}

export default Controls;
