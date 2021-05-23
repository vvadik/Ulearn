import React from 'react';
import VisualizerStatus from "./VusualizerStatus";

export interface Props {
	totalSteps: number;
	currentStep: number;
	status: VisualizerStatus;
}

function StepsCounter(props: Props): React.ReactElement<Props> {
	return (
		<div>
			<p>
				Шаг {props.totalSteps === 0 ?
				props.currentStep : props.currentStep + 1} из {props.totalSteps}
				<b>{getStatus(props.status) === null ? '' : getStatus(props.status)}</b>
			</p>
		</div>
	);
}

function getStatus(status: VisualizerStatus) : string | null {
	if (status === VisualizerStatus.Ok) {
		return null;
	}
	if (status === VisualizerStatus.Return) {
		return "Завершение функции";
	}
	if (status === VisualizerStatus.Error) {
		return "Произошла ошибка";
	}
	return null;
}

export default StepsCounter;
