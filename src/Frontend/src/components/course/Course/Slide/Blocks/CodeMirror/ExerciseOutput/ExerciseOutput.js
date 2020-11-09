import React from "react";
import PropTypes from "prop-types";

import { Warning } from "icons";
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

class ExerciseOutput extends React.Component {
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
					: ExerciseOutput.renderOutputLines(body, expectedOutput, isWrongAnswer)
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
			case solutionRunStatuses.success:
				break;
			case solutionRunStatuses.internalServerError:
			default:
				return { outputType: OutputType.serverError, body: message }
		}

		// solutionRunStatuses.success
		return { outputType: OutputType.success, body: automaticChecking.output } // TODO
	}

	static
	renderSimpleTextOutput = (output) => {
		return (<p className={ styles.oneLineErrorOutput }>
			{ output }
		</p>);
	}

	static
	renderOutputLines = (output, expectedOutput, submitContainsError) => {
		const lines = output
			.split('\n')
			.map((line, index) => ({
				actual: line,
				expected: expectedOutput[index],
			}));

		if(submitContainsError) {
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
							className={ actual === expected ? styles.outputLineColor : styles.outputErrorLineColor }>
							<td>{ i + 1 }</td>
							<td>{ actual }</td>
							<td>{ expected }</td>
						</tr>
					) }
					</tbody>
				</table>
			);
		}

		return expectedOutput.map((text, i) =>
			<p key={ i } className={ styles.oneLineOutput }>
				{ text }
			</p>);
	}
}

ExerciseOutput.propTypes = {
	solutionRunStatus: PropTypes.string, // Success, если не посылка прямо сейчас
	message: PropTypes.string, // CanBeNull

	expectedOutput: PropTypes.string, // CanBeNull

	automaticChecking: PropTypes.object, // CanBeNull
}

export default ExerciseOutput;
