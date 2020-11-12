import React from "react";
import PropTypes from "prop-types";

import { Warning } from "@skbkontur/react-icons";
import { checkingResults, processStatuses, solutionRunStatuses } from "src/consts/exercise";

import texts from "./ExerciseOutput.texts";
import styles from "./ExerciseOutput.less";

const OutputType = {
	compilationError: 'CompilationError',
	wrongAnswer: 'WrongAnswer',
	serverError: 'ServerError',
	serverMessage: 'ServerMessage',
	success: 'Success'
};

const outputTypeToStyleAndHeader = {
	[OutputType.compilationError]: { style: styles.compilationErrorOutput, header: texts.headers.compilationError },
	[OutputType.wrongAnswer]: { style: styles.wrongAnswerOutput, header: texts.headers.wrongAnswer },
	[OutputType.serverError]: { style: styles.serverErrorOutput, header: texts.headers.serverError },
	[OutputType.serverMessage]: { style: styles.serverErrorOutput, header: texts.headers.serverMessage },
	[OutputType.success]: { style: styles.output, header: texts.headers.output },
}

export function HasOutput(message, automaticChecking, expectedOutput) {
	if(message)
		return true;
	if(!automaticChecking)
		return false;
	return automaticChecking.output
		|| (automaticChecking.checkingResults === checkingResults.wrongAnswer && expectedOutput)
}

export class ExerciseOutput extends React.Component {
	render() {
		const { expectedOutput } = this.props;
		const { outputType, body } = this.getOutputTypeAndBody();
		const { style, header } = outputTypeToStyleAndHeader[outputType];
		const showIcon = outputType !== OutputType.success;
		const isWrongAnswer = outputType === OutputType.wrongAnswer
		const isSimpleTextOutput = expectedOutput === null || !isWrongAnswer;

		return (
			<div className={ style }>
				<span className={ styles.outputHeader }>
					{ <React.Fragment>{ showIcon
						? <Warning/>
						: null } { header }</React.Fragment>
					} 
				</span>
				{ isSimpleTextOutput
					? ExerciseOutput.renderSimpleTextOutput(body)
					: ExerciseOutput.renderOutputLines(body, expectedOutput)
				}
			</div>
		);
	}

	getOutputTypeAndBody = () => {
		const { solutionRunStatus, message, automaticChecking } = this.props;
		switch (solutionRunStatus) {
			case solutionRunStatuses.compilationError:
				return { outputType: OutputType.compilationError, body: message };
			case solutionRunStatuses.submissionCheckingTimeout:
			case solutionRunStatuses.ignored:
				return { outputType: OutputType.serverMessage, body: message }
			case solutionRunStatuses.internalServerError:
				return { outputType: OutputType.serverError, body: message }
			case solutionRunStatuses.success:
				if(automaticChecking) {
					return this.getOutputTypeAndBodyFromAutomaticChecking();
				} else {
					console.error(new Error(`automaticChecking is null when solutionRunStatuses is ${ solutionRunStatus }`));
					return { outputType: OutputType.success, body: message }
				}
			default:
				console.error(new Error(`solutionRunStatus has unknown value ${ solutionRunStatus }`));
				return { outputType: OutputType.serverMessage, body: message }
		}
	}

	getOutputTypeAndBodyFromAutomaticChecking = () => {
		const { automaticChecking } = this.props;
		let outputType;
		let output = automaticChecking.output;
		switch (automaticChecking.processStatus) {
			case processStatuses.done:
				outputType = this.getOutputTypeByCheckingResults();
				break;
			case processStatuses.serverError:
				outputType = OutputType.serverError;
				break;
			case processStatuses.waiting:
				outputType = OutputType.serverMessage;
				break;
			case processStatuses.running:
				outputType = OutputType.serverMessage;
				break;
			case processStatuses.waitingTimeLimitExceeded:
				outputType = OutputType.serverError;
				break;
			default:
				console.error(new Error(`processStatuses has unknown value ${ automaticChecking.processStatus }`));
				return OutputType.serverMessage
		}
		return { outputType: outputType, body: output }
	}

	getOutputTypeByCheckingResults = () => {
		const { automaticChecking } = this.props;
		switch (automaticChecking.checkingResults) {
			case checkingResults.compilationError:
				return OutputType.compilationError;
			case checkingResults.wrongAnswer:
				return OutputType.wrongAnswer;
			case checkingResults.rightAnswer:
				return OutputType.success;
			case checkingResults.notChecked:
				return OutputType.serverMessage;
			default:
				console.error(new Error(`checkingResults has unknown value ${ automaticChecking.checkingResults }`));
				return OutputType.serverMessage
		}
	}

	static
	renderSimpleTextOutput = (output) => {
		const lines = output.split('\n');
		return <div className={ styles.outputTextWrapper }>
			{ lines.map((text, i) =>
				<pre key={ i } className={ styles.outputParagraph }>
					{ text }
				</pre>)
			}
		</div>
	}

	static
	renderOutputLines = (output, expectedOutput) => {
		const actualOutputLines = output.match(/[^\r\n]+/g);
		const expectedOutputLines = expectedOutput.match(/[^\r\n]+/g);
		const length = Math.max(actualOutputLines.length, expectedOutputLines.length);
		const lines = [];
		for (let i = 0; i < length; i++) {
			const actual = i < actualOutputLines.length ? actualOutputLines[i] : null;
			const expected = i < expectedOutputLines.length ? expectedOutputLines[i] : null;
			lines.push({ actual, expected });
		}

		return (
			<table className={ styles.outputTable }>
				<thead>
				<tr>
					<th/>
					<th>{ texts.output.userOutput }</th>
					<th>{ texts.output.expectedOutput }</th>
				</tr>
				</thead>
				<tbody>
				{ lines.map(({ actual, expected }, i) =>
					<tr key={ i }
						className={ actual === expected ? styles.outputMatchColor : styles.outputNotMatchColor }>
						<td><span>{ i + 1 }</span></td>
						<td><span>{ actual }</span></td>
						<td><span>{ expected }</span></td>
					</tr>
				) }
				</tbody>
			</table>
		);
	}
}

ExerciseOutput.propTypes = {
	solutionRunStatus: PropTypes.string.isRequired, // Success, если не посылка прямо сейчас
	message: PropTypes.string,
	expectedOutput: PropTypes.string,
	automaticChecking: PropTypes.object,
}
