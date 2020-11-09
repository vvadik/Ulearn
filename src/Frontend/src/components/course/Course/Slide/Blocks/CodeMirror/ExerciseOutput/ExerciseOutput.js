import React from "react";
import PropTypes from "prop-types";

import { Error, } from "icons";

import { checkingResults } from "src/consts/exercise";

import texts from "./ExerciseOutput.texts";
import styles from "./ExerciseOutput.less";

class ExerciseOutput extends React.Component {
	constructor(props) {
		super(props);
	}

	render() {
		const { output, expectedOutput, checkingState, } = this.props;
		const submitContainsError = checkingState === checkingResults.wrongAnswer; // TODO

		const wrapperClasses = submitContainsError ? styles.wrongOutput : styles.output;
		const headerClasses = submitContainsError ? styles.wrongOutputHeader : styles.outputHeader; // TODO убрать явное задание отдельного класса для header

		return (
			<div className={ wrapperClasses }>
				<span className={ headerClasses }>
					{ submitContainsError
						? <React.Fragment><Error/>{ texts.wrongAnswer }</React.Fragment> // TODO
						: texts.output.output }
				</span>
				{ ExerciseOutput.renderOutputLines(output, expectedOutput, submitContainsError) }
			</div>
		);
	}

	static
	getHeaderTextAndStyle = (checkingState) => {
		// TODO
	}

	static
	renderOutputLines = (output, expectedOutput, submitContainsError) => {
		if(!expectedOutput) {
			return (<p className={ styles.oneLineErrorOutput }>
				{ output }
			</p>);
		}

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
	output: PropTypes.string,
	expectedOutput: PropTypes.string,
	checkingState: PropTypes.string,
}

export default ExerciseOutput;
