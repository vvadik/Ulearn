import React from "react";

import { Warning } from "@skbkontur/react-icons";
import { checkingResults, processStatuses, solutionRunStatuses } from "src/consts/exercise.js";

import texts from "./ExerciseOutput.texts";
import styles from "./ExerciseOutput.less";

enum OutputType {
	CompilationError = "CompilationError",
	WrongAnswer = "WrongAnswer",
	ServerError = "ServerError",
	ServerMessage = "ServerMessage",
	Success = "Success"
}

const outputTypeToStyleAndHeader: EnumDictionary<OutputType, { style: string, header: string }> = {
	[OutputType.CompilationError]: { style: styles.compilationErrorOutput, header: texts.headers.compilationError },
	[OutputType.WrongAnswer]: { style: styles.wrongAnswerOutput, header: texts.headers.wrongAnswer },
	[OutputType.ServerError]: { style: styles.serverErrorOutput, header: texts.headers.serverError },
	[OutputType.ServerMessage]: { style: styles.serverErrorOutput, header: texts.headers.serverMessage },
	[OutputType.Success]: { style: styles.output, header: texts.headers.output },
}

function HasOutput(message: string, automaticChecking: any, expectedOutput: string): boolean {
	if(message)
		return true;
	if(!automaticChecking)
		return false;
	return automaticChecking.output
		|| (automaticChecking.checkingResults === checkingResults.wrongAnswer && expectedOutput)
}

interface OutputTypeProps {
	solutionRunStatus: string, // Success, если не посылка прямо сейчас
	message: string | null,
	expectedOutput: string | null,
	automaticChecking: any | null
}

class ExerciseOutput extends React.Component<OutputTypeProps> {
	render() {
		const { expectedOutput } = this.props;
		const { outputType, body } = this.getOutputTypeAndBody();
		const { style, header } = outputTypeToStyleAndHeader[outputType];
		const showIcon = outputType !== OutputType.Success;
		const isWrongAnswer = outputType === OutputType.WrongAnswer
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
					? ExerciseOutput.renderSimpleTextOutput(body ?? "")
					: ExerciseOutput.renderOutputLines(body ?? "", expectedOutput ?? "")
				}
			</div>
		);
	}

	getOutputTypeAndBody() {
		const { solutionRunStatus, message, automaticChecking } = this.props;
		switch (solutionRunStatus) {
			case solutionRunStatuses.compilationError:
				return { outputType: OutputType.CompilationError, body: message };
			case solutionRunStatuses.submissionCheckingTimeout:
			case solutionRunStatuses.ignored:
				return { outputType: OutputType.ServerMessage, body: message }
			case solutionRunStatuses.internalServerError:
				return { outputType: OutputType.ServerError, body: message }
			case solutionRunStatuses.success:
				if(automaticChecking) {
					return this.getOutputTypeAndBodyFromAutomaticChecking();
				} else {
					console.error(new Error(`automaticChecking is null when solutionRunStatuses is ${ solutionRunStatus }`));
					return { outputType: OutputType.Success, body: message }
				}
			default:
				console.error(new Error(`solutionRunStatus has unknown value ${ solutionRunStatus }`));
				return { outputType: OutputType.ServerMessage, body: message }
		}
	}

	getOutputTypeAndBodyFromAutomaticChecking(): { outputType: OutputType, body: string | null } {
		const { automaticChecking } = this.props;
		let outputType: OutputType;
		const output = automaticChecking.output;
		switch (automaticChecking.processStatus) {
			case processStatuses.done:
				outputType = this.getOutputTypeByCheckingResults();
				break;
			case processStatuses.serverError:
				outputType = OutputType.ServerError;
				break;
			case processStatuses.waiting:
				outputType = OutputType.ServerMessage;
				break;
			case processStatuses.running:
				outputType = OutputType.ServerMessage;
				break;
			case processStatuses.waitingTimeLimitExceeded:
				outputType = OutputType.ServerError;
				break;
			default:
				console.error(new Error(`processStatuses has unknown value ${ automaticChecking.processStatus }`));
				outputType = OutputType.ServerMessage
		}
		return { outputType: outputType, body: output }
	}

	getOutputTypeByCheckingResults(): OutputType {
		const { automaticChecking } = this.props;
		switch (automaticChecking.result) {
			case checkingResults.compilationError:
				return OutputType.CompilationError;
			case checkingResults.wrongAnswer:
				return OutputType.WrongAnswer;
			case checkingResults.rightAnswer:
				return OutputType.Success;
			case checkingResults.notChecked:
				return OutputType.ServerMessage;
			default:
				console.error(new Error(`checkingResults has unknown value ${ automaticChecking.result }`));
				return OutputType.ServerMessage
		}
	}

	static renderSimpleTextOutput(output: string) {
		const lines = output.split('\n');
		return <div className={ styles.outputTextWrapper }>
			{ lines.map((text, i) =>
				<span key={ i } className={ styles.outputParagraph }>
					{ text }
				</span>)
			}
		</div>
	}

	static renderOutputLines(output: string, expectedOutput: string) {
		const actualOutputLines = output.match(/[^\r\n]+/g) ?? [];
		const expectedOutputLines = expectedOutput.match(/[^\r\n]+/g) ?? [];
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

export { ExerciseOutput, OutputTypeProps, HasOutput }
