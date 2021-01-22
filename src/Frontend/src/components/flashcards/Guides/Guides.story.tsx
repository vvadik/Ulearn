import React from "react";
import Guides from "./Guides";
import { guides } from "src/components/flashcards/storyData";

export default {
	title: "Cards/Guides",
};

export const StandardGuides = () => (
	<Guides
		guides={ guides }
	/>
);

StandardGuides.storyName = "default";
