import React from 'react';
import { Controlled, } from "react-codemirror2";
import Modal from "../../../../../../modal/Modal";

import CodeMirror, { EditorConfiguration, } from "codemirror";
require('codemirror/addon/selection/active-line.js');
require('codemirror/addon/selection/mark-selection.js');
import 'codemirror/lib/codemirror.css';
import 'codemirror/theme/darcula.css';

import { loadLanguageStyles } from "../ExerciseUtils";
import { Language } from "../../../../../../../consts/languages";
import DataArea from "./DataArea";
import StepsCounter from "./StepsCounter";
import { Loader } from "@skbkontur/react-ui";

import './visualizer.css';
import Controls from "./Controls";
import JSONView from './react-json-view/src/js/index';
import VisualizerStatus from "./VusualizerStatus";

export interface Props {
	code: string;
	input: string;
}

function getCodeMirrorOptions(): EditorConfiguration {
	return {
		mode: loadLanguageStyles(Language.python3),
		lineNumbers: true,
		scrollbarStyle: 'native',
		lineWrapping: true,
		readOnly: false,
		matchBrackets: true,
		tabSize: 4,
		indentUnit: 4,
		indentWithTabs: true,
		theme: 'darcula',
		gutters: ["CodeMirror-linenumbers", "arrow"]
	};
}

interface State {
	code: string;
	input: string;
	output: string;

	variables: Record<string, any>;

	totalSteps: number;
	currentStep: number;

	trace: Array<Record<string, any>>;

	editor: CodeMirror.Editor | null;
	status: VisualizerStatus;
}

class Visualizer extends React.Component<Props, State> {
	constructor(props) {
		super(props);
		this.state = {
			editor: null,
			code: props.code,
			input: props.input,
			output: "",
			totalSteps: 0,
			currentStep: 0,
			trace: [],
			status: VisualizerStatus.Ok,
			variables: {}
		};
		this.updateInput = this.updateInput.bind(this);
		this.previousStep = this.previousStep.bind(this);
		this.nextStep = this.nextStep.bind(this);
		this.getRuntimeData = this.getRuntimeData.bind(this);
	}

	nextStep() : void {
		this.showStep(this.state.currentStep + 1);
	}

	previousStep() : void {
		this.showStep(this.state.currentStep - 1);
	}

	getRuntimeData() : void {
		this.setState({status: VisualizerStatus.Loading});
		const xhr = new XMLHttpRequest();
		xhr.addEventListener('load', () => {
			this.run(xhr.responseText)
		});
		xhr.open('POST', 'https://python-visualizer-api.vercel.app/run');
		xhr.send(JSON.stringify({code: this.state.code, input_data: this.state.input}));
	}

	run(trace: string) : void {
		this.setState({status: VisualizerStatus.Ok, output: ''});
		const steps = JSON.parse(trace).message.trace;
		this.setState({trace: steps, totalSteps: steps.length, currentStep: 0});
	}

	showStep(stepNumber: number) : void {
		const currentStep = this.state.trace[stepNumber];
		const lineNumber = parseInt(currentStep["line"]);
		this.setArrow(lineNumber);
		this.setState({
			output: currentStep.stdout,
			variables: this.getVariables(currentStep),
			currentStep: stepNumber,
		});
	}

	setArrow(lineNumber: number) : void {
		this.state.editor?.clearGutter("arrow");
		const arrow = document.createElement("div");
		arrow.innerHTML = "→";
		arrow.style.color = "#FF9E59";
		this.state.editor?.setGutterMarker(lineNumber - 1, "arrow", arrow);
	}

	getVariables(trace) {
		let globals = trace["globals"];
		/*for (let variable of globals) {
			if (variable === "LIST") {
				variable = "list";
			}
		}*/
		return {
			"Глобальные": globals,
			"Локальные": trace["stack_locals"],
		};
	}

	updateInput(e) : void {
		this.setState({input: e.target.value});
	}

	render() : React.ReactElement {
		return (
			<div>
				<Modal header={ "Визуализатор" } onClose={ () => {return 0;} }>
					<Loader active={this.state.status === VisualizerStatus.Loading}>
						<StepsCounter
							totalSteps={this.state.totalSteps}
							currentStep={this.state.currentStep}
							status={this.state.status}
						/>

						<div className={"main"}>
							<div id={"code-mirror"}>
								<Controlled
									options={getCodeMirrorOptions()}
									onBeforeChange={ (editor, data, value) =>
									{this.setState({code: value});} }
									onChange={ (editor, data, value) =>
									{this.setState({code: value});} }
									value={ this.state.code }
									editorDidMount={editor => {
										this.setState({editor: editor});
									}}
								/>
							</div>

							<div className={"variables"}>
								<JSONView src={this.state.variables} />
							</div>
						</div>

						<div className={"fields"}>
						<DataArea
							input={this.state.input}
							output={this.state.output}
							updateInput={this.updateInput}
						/>
						</div>

						<div className={"actions"}>
						<Controls
							run={this.getRuntimeData}
							next={this.nextStep}
							previous={this.previousStep}
							currentStep={this.state.currentStep}
							totalSteps={this.state.totalSteps}
						/>
						</div>
					</Loader>
				</Modal>
			</div>
		);
	}
}

export default Visualizer;
