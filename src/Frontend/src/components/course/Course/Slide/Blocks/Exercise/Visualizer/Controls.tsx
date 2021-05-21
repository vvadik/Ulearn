import RunButton from "./RunButton";
import React, { MouseEventHandler } from "react";
import StepButton from "./StepButton";

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
				<RunButton onClick={this.props.run} text={"Запустить"} />
				<StepButton
					onClick={this.props.previous}
					disabled={this.props.currentStep === 0 || this.props.currentStep === 1}
					text={"Назад"}
				/>
				<StepButton
					onClick={this.props.next}
					disabled={this.props.currentStep === this.props.totalSteps}
					text={"Дальше"}
				/>
			</div>
		);
	}
}

export default Controls;
