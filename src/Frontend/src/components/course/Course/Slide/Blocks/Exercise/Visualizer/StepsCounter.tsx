import React from 'react';

export interface Props {
	totalSteps: number;
	currentStep: number;
}

function StepsCounter(props: Props): React.ReactElement<Props> {
	const totalSteps = props.totalSteps;
	const currentStep = props.currentStep;

	return (
		<div>
			<p>Шаг {totalSteps === 0 ? currentStep : currentStep + 1} из {totalSteps}</p>
		</div>
	);
}

export default StepsCounter;
