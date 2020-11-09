import React from "react";
import ExerciseOutput from "./ExerciseOutput";

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
	message: "Success text",
	expectedOutput: null,
	automaticChecking: null
};

