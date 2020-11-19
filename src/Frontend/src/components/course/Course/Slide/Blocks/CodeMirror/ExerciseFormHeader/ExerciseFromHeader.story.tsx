import React from "react";
import type { Story } from "@storybook/react";
import { ExerciseFormHeader, ExerciseFormHeaderProps } from "./ExerciseFormHeader";
import {
	AutomaticExerciseCheckingProcessStatus,
	AutomaticExerciseCheckingResult,
	SolutionRunStatus,
} from "src/models/exercise";

const Template: Story<ExerciseFormHeaderProps> = (args) => <ExerciseFormHeader { ...args } />;

const submissionInfo = {
	id: 1,
	code: "",
	timestamp: "",
	automaticChecking: {
		processStatus: AutomaticExerciseCheckingProcessStatus.Done,
		result: AutomaticExerciseCheckingResult.RightAnswer,
		output: null,
		reviews: null
	},
	manualCheckingPassed: false,
	manualCheckingReviews: []
}

export const Editable = Template.bind({});
Editable.args = {
	solutionRunStatus: null,
	selectedSubmission: submissionInfo,
	score: 10
}

export default {
	title: 'Exercise/ExerciseFromHeader',
	component: ExerciseFormHeader,
	argTypes: {
		solutionRunStatus: {
			control: {
				type: 'select',
				options: [...Object.values(SolutionRunStatus), null],
			},
		},
	}
};
