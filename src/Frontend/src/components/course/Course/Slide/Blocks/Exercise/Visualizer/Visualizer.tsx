import React from 'react';
import { Controlled, } from "react-codemirror2";

import CodeMirror, * as codemirror from "codemirror";
import { EditorConfiguration } from "codemirror";
import 'codemirror/lib/codemirror.css';
import 'codemirror/theme/darcula.css';

import { loadLanguageStyles } from "../ExerciseUtils";
import { Language } from "src/consts/languages";
import { DataArea } from "./DataArea";
import { StepsCounter } from "./StepsCounter";
import { Loader, Modal } from "@skbkontur/react-ui";

import './visualizer.css';
import { Controls } from "./Controls";
import JSONView from './react-json-view/src/js/index';
import { VisualizerStatus } from "./VusualizerStatus";

import parseGlobals from './helpers/parseTrace';

import texts from './Visualizer.texts';

const codeMirrorOptions: EditorConfiguration = {
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

interface VisualizerProps {
	code: string;
	input: string;
}

interface State {
	code: string;
	input: string;
	output: string;

	variables: {
		"Глобальные"?: any;
		"Локальные"?: any;
	};

	totalSteps: number;
	currentStep: number;

	trace: Array<VisualizerStep>;

	editor: CodeMirror.Editor | null;
	status: VisualizerStatus;
}

interface RunData {
	message: {
		trace: Array<VisualizerStep>;
	};
}

interface VisualizerStep {
	line: string;
	event: 'exception' | 'uncaught_exception' | 'return';
	stdout: string;
	globals: any;
	stack_locals: any;
}

class Visualizer extends React.Component<VisualizerProps, State> {
	constructor(props: VisualizerProps) {
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
	}

	nextStep = () : void =>
		this.showStep(this.state.currentStep + 1);

	previousStep = () : void =>
		this.showStep(this.state.currentStep - 1);

	getRuntimeData = () : void => {
		this.setState({status: VisualizerStatus.Loading});

		fetch('https://python-visualizer-api.vercel.app/run',
			{
					method: "POST",
					body: JSON.stringify({
						code: this.state.code,
						input_data: this.state.input
					})
				}
		)
			.then(r => r.json())
			.then(r => this.run(r));
	}

	run(runData: RunData) : void {
		this.setArrow(0);
		const steps = runData.message.trace;
		this.setState({
			trace: steps,
			totalSteps: steps.length,
			status: VisualizerStatus.Ok,
			currentStep: 0,
			output: ''
		}, () => this.showStep(0));
	}

	showStep(stepNumber: number) : void {
		const currentStep = this.state.trace[stepNumber];
		const lineNumber = parseInt(currentStep.line);
		const event = currentStep.event;
		let stdout = currentStep.stdout === undefined ? '' : currentStep.stdout;

		let newStatus = VisualizerStatus.Ok;
		if (event === "exception" || event === "uncaught_exception") {
			newStatus = VisualizerStatus.Error;
			stdout += `\n========\n${ currentStep.exception_str }`
		}
		else if (event === "return") {
			newStatus = VisualizerStatus.Return;
		}

		this.setArrow(lineNumber);
		this.setState({
			output: stdout,
			variables: this.getVariables(currentStep),
			currentStep: stepNumber,
			status: newStatus,
		});
	}

	setArrow = (lineNumber: number) : void => {
		this.state.editor?.clearGutter("arrow");
		const arrow = document.createElement("div");
		arrow.innerHTML = "→";
		arrow.style.color = "#FF9E59";
		this.state.editor?.setGutterMarker(lineNumber - 1, "arrow", arrow);
	}

	getVariables = (visualizerStep: VisualizerStep) : Record<string, any> => {
		return {
			"Глобальные": parseGlobals(visualizerStep.globals),
			"Локальные": visualizerStep.stack_locals,
		}
	};

	updateInput = (value: string) : void =>
		this.setState({input: value});

	updateCode = (editor: CodeMirror.Editor, data: codemirror.EditorChange,
		value: string) : void =>
		this.setState({ code: value });

	setEditorToState = (editor: CodeMirror.Editor) : void =>
		this.setState({ editor: editor });

	renderEditor = () : React.ReactElement =>
		(
			<div className={ "main" }>
				<div id={ "code-mirror" }>
					<Controlled
						options={ codeMirrorOptions }
						onBeforeChange={ this.updateCode }
						onChange={ this.updateCode }
						value={ this.state.code }
						editorDidMount={ this.setEditorToState }
					/>
				</div>

				<div className={ "variables" }>
					<JSONView src={ this.state.variables } />
				</div>
			</div>
	);

	renderControls = () : React.ReactElement =>
		(
			<div>
				<div className={ "fields" }>
					<DataArea
						input={ this.state.input }
						output={ this.state.output }
						updateInput={ this.updateInput }
					/>
				</div>

				<div className={ "actions" }>
				<Controls
					run={ this.getRuntimeData }
					next={ this.nextStep }
					previous={ this.previousStep }
					currentStep={ this.state.currentStep }
					totalSteps={ this.state.totalSteps }
				/>
				</div>
			</div>
	);

	render() : React.ReactElement {
		return (
			<div>
				<Modal>
					<Modal.Header>{ texts.visualizer }</Modal.Header>
					<Modal.Body>
						<Loader active={ this.state.status === VisualizerStatus.Loading }>

							<StepsCounter
								totalSteps={ this.state.totalSteps }
								currentStep={ this.state.currentStep }
								status={ this.state.status }
							/>

							{ this.renderEditor() }
							{ this.renderControls() }
						</Loader>
					</Modal.Body>
				</Modal>
			</div>
		);
	}
}

export { Visualizer, VisualizerProps };
