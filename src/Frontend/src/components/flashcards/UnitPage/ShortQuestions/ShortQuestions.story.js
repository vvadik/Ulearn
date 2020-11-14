import React from "react";
import ShortQuestions from "./ShortQuestions";
import shortQuestionsExample from "./shortQuestionsExample";

export default {
	title: "Cards/UnitPage/ShortQuestions",
};

export const Default = () => (
	<ShortQuestions questionsWithAnswers={shortQuestionsExample} />
);

Default.storyName = "default";
