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

import { getVariables, VisualizerStep } from './helpers/parseTrace';

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
	activeLine: number | null;

	trace: Array<VisualizerStep>;

	editor: CodeMirror.Editor | null;
	status: VisualizerStatus;
}

interface RunData {
	message: {
		trace: Array<VisualizerStep>;
	};
}

class Visualizer extends React.Component<VisualizerProps, State> {
	private getRuntimeData = () : void => {
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

	constructor(props: VisualizerProps) {
		super(props);
		this.state = {
			editor: null,
			code: props.code,
			input: props.input,
			output: "",
			totalSteps: 0,
			currentStep: 0,
			activeLine: null,
			trace: [],
			status: VisualizerStatus.Ready,
			variables: {}
		};
	}

	showNextStep = () : void =>
		this.showStep(this.state.currentStep + 1);

	showPreviousStep = () : void =>
		this.showStep(this.state.currentStep - 1);

	run = (runData: RunData) : void => {
		this.setActiveLine(0);
		const steps = runData.message.trace;
		this.setState({
			trace: steps,
			totalSteps: steps.length,
			status: VisualizerStatus.Running,
			currentStep: 0,
			output: ''
		}, () => this.showStep(0));
	}

	showStep = (stepNumber: number) : void => {
		const currentStep = this.state.trace[stepNumber] as VisualizerStep;
		const lineNumber = parseInt(currentStep.line);
		const event = currentStep.event;
		let stdout = currentStep.stdout === undefined ? '' : currentStep.stdout;

		let newStatus = VisualizerStatus.Running;
		if (event === "exception" || event === "uncaught_exception") {
			newStatus = VisualizerStatus.Error;
			stdout += `\n========\n${ currentStep.exception_str }`
		}
		else if (event === "return") {
			newStatus = VisualizerStatus.Return;
		}

		this.setActiveLine(lineNumber);
		this.state.editor?.scrollIntoView({line: lineNumber - 1, ch: 0});
		this.setState({
			output: stdout,
			variables: getVariables(currentStep),
			currentStep: stepNumber,
			status: newStatus,
			activeLine: lineNumber,
		});
	}

	setActiveLine = (lineNumber: number) : void => {
		this.state.editor?.clearGutter("arrow");
		const arrow = document.createElement("div");
		arrow.innerHTML = "→";
		arrow.style.color = "#FF9E59";
		this.state.editor?.setGutterMarker(lineNumber - 1, "arrow", arrow);

		if (this.state.activeLine !== null) {
			this.state.editor?.removeLineClass(this.state.activeLine - 1, "background", "active-line");
		}
		this.state.editor?.addLineClass(lineNumber - 1, "background", "active-line");
	}

	updateInput = (value: string) : void =>
		this.setState({input: value});

	updateCode = (editor: CodeMirror.Editor, data: codemirror.EditorChange,
		value: string) : void =>
		this.setState({ code: value, status: VisualizerStatus.Blocked});

	setEditorToState = (editor: CodeMirror.Editor) : void =>
		this.setState({ editor: editor });

	renderEditor = () : React.ReactElement =>
		(
			<div className={ "main" }>
				<div id={ "code-mirror" }>
					<Controlled
						options={ codeMirrorOptions }
						onBeforeChange={ this.updateCode }
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
				<div className={ "actions" }>
				<Controls
					run={ this.getRuntimeData }
					next={ this.showNextStep }
					previous={ this.showPreviousStep }
					visualizerStatus={ this.state.status }
					currentStep={ this.state.currentStep }
					totalSteps={ this.state.totalSteps }
				/>
				</div>
			</div>
	);

	render() : React.ReactElement {
		return (
			<div>
				<Modal width={ "90vw" }>
					<Modal.Header sticky={ false }>{ texts.visualizer }</Modal.Header>
					<Modal.Body>
						<Loader active={ this.state.status === VisualizerStatus.Loading }>

							<StepsCounter
								totalSteps={ this.state.totalSteps }
								currentStep={ this.state.currentStep }
								status={ this.state.status }
							/>

							{ this.renderEditor() }

							<DataArea
								input={ this.state.input }
								output={ this.state.output }
								updateInput={ this.updateInput }
							/>
						</Loader>
					</Modal.Body>
					<Modal.Footer>
						{ this.renderControls() }
					</Modal.Footer>
				</Modal>
			</div>
		);
	}
}

export { Visualizer, VisualizerProps };
