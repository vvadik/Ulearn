import React from "react";
import {ExerciseOutput} from "./ExerciseOutput";

import { checkingResults, processStatuses, solutionRunStatuses } from "src/consts/exercise";

export default {
	title: 'Exercise/ExerciseOutput'
};

const Template = (args) => <ExerciseOutput {...args} />;

export const SolutionRunStatusCompilationError = Template.bind({});
SolutionRunStatusCompilationError.args = {
	solutionRunStatus: solutionRunStatuses.compilationError,
	message: "CompilationError text",
	expectedOutput: "",
	automaticChecking: null
};

export const SolutionRunStatusInternalServerError = Template.bind({});
SolutionRunStatusInternalServerError.args = {
	solutionRunStatus: solutionRunStatuses.internalServerError,
	message: "InternalServerError text",
	expectedOutput: null,
	automaticChecking: null
};

export const SolutionRunStatusSuccess = Template.bind({});
SolutionRunStatusSuccess.args = {
	solutionRunStatus: solutionRunStatuses.success,
	message: null,
	expectedOutput: null,
	automaticChecking: {
		output: "Success text",
		processStatus: processStatuses.done,
		checkingResults: checkingResults.rightAnswer
	}
};

export const SolutionRunStatusCompilationErrorFromChecker = Template.bind({});
SolutionRunStatusCompilationErrorFromChecker.args = {
	solutionRunStatus: solutionRunStatuses.success,
	message: null,
	expectedOutput: null,
	automaticChecking: {
		output: "CompilationError text",
		processStatus: processStatuses.done,
		checkingResults: checkingResults.compilationError
	}
};

export const TableWrongAnswer = Template.bind({});
TableWrongAnswer.args = {
	solutionRunStatus: solutionRunStatuses.success,
	message: null,
	expectedOutput: "1\n2\n3\n4",
	automaticChecking: {
		output: "3\n2\n3",
		processStatus: processStatuses.done,
		checkingResults: checkingResults.wrongAnswer
	}
};
