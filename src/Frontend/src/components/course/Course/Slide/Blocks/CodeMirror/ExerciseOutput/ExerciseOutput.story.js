import React from "react";
import ExerciseOutput from "./ExerciseOutput";

import { checkingResults } from "src/consts/exercise";

export default {
	title: 'Exercise/ExerciseOutput'
};

const Template = (args) => <ExerciseOutput {...args} />;

export const WrongAnswerWithText = Template.bind({});
WrongAnswerWithText.args = {
	output: "text",
	expectedOutput: "",
	checkingState: checkingResults.wrongAnswer
};
