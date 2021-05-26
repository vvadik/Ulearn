import React, { MouseEventHandler } from "react";
import { ControlButton } from "./ControlButton";
import texts from './Visualizer.texts';
import { VisualizerStatus } from "./VusualizerStatus";

interface ControlsProps {
	run: MouseEventHandler;
	next: MouseEventHandler;
	previous: MouseEventHandler;

	visualizerStatus: VisualizerStatus;

	currentStep: number;
	totalSteps: number;
}

export const Controls =
	({ run, next, previous, visualizerStatus, currentStep, totalSteps } : ControlsProps) :
		React.ReactElement =>
	 (
		<div>
			<ControlButton
				use={ visualizerStatus === VisualizerStatus.Ready ||
				visualizerStatus === VisualizerStatus.Blocked ? "primary" : "default" }
				onClick={ run }
				text={ visualizerStatus === VisualizerStatus.Ready ?
					texts.controls.run : texts.controls.rerun }
				disabled={ false }
			/>
			<ControlButton
				use={ visualizerStatus === VisualizerStatus.Ready ? "default" : "primary" }
				onClick={ previous }
				disabled={ visualizerStatus === VisualizerStatus.Blocked || currentStep === 0 }
				text={ texts.controls.back }
			/>
			<ControlButton
				use={ visualizerStatus === VisualizerStatus.Ready ? "default" : "primary" }
				onClick={ next }
				disabled={ visualizerStatus === VisualizerStatus.Blocked || currentStep === totalSteps - 1 || totalSteps === 0 }
				text={ texts.controls.next }
			/>
		</div>
	);
