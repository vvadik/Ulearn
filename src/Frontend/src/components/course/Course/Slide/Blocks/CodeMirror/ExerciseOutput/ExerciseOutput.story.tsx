import React from "react";
import type { Story } from "@storybook/react";
import { ExerciseOutput, OutputTypeProps } from "./ExerciseOutput";

import { checkingResults, processStatuses, solutionRunStatuses } from "src/consts/exercise.js";

export default {
	title: 'Exercise/ExerciseOutput'
};

const Template: Story<OutputTypeProps> = (args) => <ExerciseOutput { ...args } />;

export const SolutionRunStatusCompilationError = Template.bind({});
SolutionRunStatusCompilationError.args = {
	solutionRunStatus: solutionRunStatuses.compilationError,
	message: "Как минимум один из тестов не пройден!\n"
		+ "Название теста: MoveManipulatorTo_ActuallyBringsManipulatorToDesiredLocation(-207.950052059698d,109.42692431548d,62.2635171658309d)\n"
		+ "Сообщение:\n"
		+ "  actual x\n"
		+ "  Expected: -207.95005205969795d +/- 9.9999999999999995E-07d",
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
	expectedOutput: "1\n2\n      3\nНазвание теста: MoveManipulatorTo_ActuallyBringsManipulatorToDesiredLocation(-207.950052059698d,109.42692431548d,62.2635171658309d)",
	automaticChecking: {
		output: "3\n2\n3",
		processStatus: processStatuses.done,
		result: checkingResults.wrongAnswer
	}
};
