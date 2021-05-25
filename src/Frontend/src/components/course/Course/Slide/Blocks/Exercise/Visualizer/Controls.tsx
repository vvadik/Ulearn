import React, { MouseEventHandler } from "react";
import { ControlButton } from "./ControlButton";
import texts from './Visualizer.texts';

interface ControlsProps {
	run: MouseEventHandler;
	next: MouseEventHandler;
	previous: MouseEventHandler;

	currentStep: number;
	totalSteps: number;
}

export const Controls =
	({ run, next, previous, currentStep, totalSteps } : ControlsProps) :
		React.ReactElement =>
	 (
		<div>
			<ControlButton
				use={ "primary" }
				onClick={ run }
				text={ texts.controls.run }
				disabled={ false }
			/>
			<ControlButton
				use={ "default" }
				onClick={ previous }
				disabled={ currentStep === 0 }
				text={ texts.controls.back }
			/>
			<ControlButton
				use={ "default" }
				onClick={ next }
				disabled={ currentStep === totalSteps - 1 || totalSteps === 0 }
				text={ texts.controls.next }
			/>
		</div>
	);
