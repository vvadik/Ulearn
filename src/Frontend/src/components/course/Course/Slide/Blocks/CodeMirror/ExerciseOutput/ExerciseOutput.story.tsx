import React from "react";
import type { Story } from "@storybook/react";
import { ExerciseOutput, OutputTypeProps } from "./ExerciseOutput";

import {
	AutomaticExerciseCheckingResult as CheckingResult,
	AutomaticExerciseCheckingProcessStatus as ProcessStatus,
	SolutionRunStatus
} from "../../../../../../../models/exercise";

export default {
	title: 'Exercise/ExerciseOutput'
};

const Template: Story<OutputTypeProps> = (args) => <ExerciseOutput { ...args } />;

export const SolutionRunStatusCompilationError = Template.bind({});
SolutionRunStatusCompilationError.args = {
	solutionRunStatus: SolutionRunStatus.CompilationError,
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
	solutionRunStatus: SolutionRunStatus.InternalServerError,
	message: "InternalServerError text",
	expectedOutput: null,
	automaticChecking: null
};

export const SolutionRunStatusSuccess = Template.bind({});
SolutionRunStatusSuccess.args = {
	solutionRunStatus: SolutionRunStatus.Success,
	message: null,
	expectedOutput: null,
	automaticChecking: {
		output: "Success text",
		processStatus: ProcessStatus.Done,
		result: CheckingResult.RightAnswer,
		reviews: null
	}
};

export const SolutionRunStatusCompilationErrorFromChecker = Template.bind({});
SolutionRunStatusCompilationErrorFromChecker.args = {
	solutionRunStatus: SolutionRunStatus.Success,
	message: null,
	expectedOutput: null,
	automaticChecking: {
		output: "CompilationError text",
		processStatus: ProcessStatus.Done,
		result: CheckingResult.CompilationError,
		reviews: null
	}
};

export const TableWrongAnswer = Template.bind({});
TableWrongAnswer.args = {
	solutionRunStatus: SolutionRunStatus.Success,
	message: null,
	expectedOutput: "1\n2\n      3\nНазвание теста: MoveManipulatorTo_ActuallyBringsManipulatorToDesiredLocation(-207.950052059698d,109.42692431548d,62.2635171658309d)",
	automaticChecking: {
		output: "3\n2\n3",
		processStatus: ProcessStatus.Done,
		result: CheckingResult.WrongAnswer,
		reviews: null
	}
};
