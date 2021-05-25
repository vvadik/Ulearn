import React from 'react';
import VisualizerStatus from "./VusualizerStatus";

import texts from './Visualizer.texts';

export interface Props {
	totalSteps: number;
	currentStep: number;
	status: VisualizerStatus;
}

function StepsCounter(props: Props): React.ReactElement<Props> {
	return (
		<div>
			<p>
				{ texts.stepsCounter.currentStepNumber(props.currentStep, props.totalSteps) }
				<b> { texts.stepsCounter.status(props.status) === null ?
					'' : texts.stepsCounter.status(props.status) }</b>
			</p>
		</div>
	);
}

export default StepsCounter;
