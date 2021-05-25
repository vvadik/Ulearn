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

function Controls({ run, next, previous, currentStep, totalSteps } : Props) : React.ReactElement {
	return (
		<div>
			<RunButton onClick={ run } text={ texts.controls.run } />
			<StepButton
				onClick={ previous }
				disabled={ currentStep === 0 }
				text={ texts.controls.back }
			/>
			<StepButton
				onClick={ next }
				disabled={ currentStep === totalSteps - 1 }
				text={ texts.controls.next }
			/>
		</div>
	);
}
export default Controls;
