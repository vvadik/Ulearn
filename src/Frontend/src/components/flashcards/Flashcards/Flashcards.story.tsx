import React from "react";
import Flashcards from "./Flashcards.js";
import { flashcards, infoByUnits } from "src/components/flashcards/storyData";


export default {
	title: "Cards",
};

export const Default = (): React.ReactNode =>
	<Flashcards
		infoByUnits={ infoByUnits }
		unitId={ flashcards[0].unitId }
		courseId={ '' }
		flashcards={ flashcards }
		onClose={ () => ({}) }
		sendFlashcardRate={ () => ({}) }
	/>;

Default.storyName = "default";
