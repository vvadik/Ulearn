import React from "react";
import Flashcards from "./Flashcards";
import flashcardsExample from "./flashcardsExample";


export default {
	title: "Cards",
};

export const Default = () => <Flashcards flashcards={flashcardsExample} />;

Default.storyName = "default";
