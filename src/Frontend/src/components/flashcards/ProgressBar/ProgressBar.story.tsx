import React from "react";
import ProgressBar from "./ProgressBar.js";

export default {
	title: "Cards/ProgressBar",
};

export const Scores11251018 = (): React.ReactNode => (
	<ProgressBar
		statistics={ {
			notRated: 1,
			rate1: 1,
			rate2: 2,
			rate3: 5,
			rate4: 10,
			rate5: 18,
		} }
		totalFlashcardsCount={ 37 }
	/>
);

Scores11251018.storyName = "scores: 1,1,2,5,10,18";

export const Scores1000000 = (): React.ReactNode => (
	<ProgressBar
		statistics={ {
			notRated: 10,
			rate1: 0,
			rate2: 0,
			rate3: 0,
			rate4: 0,
			rate5: 0,
		} }
		totalFlashcardsCount={ 10 }
	/>
);

Scores1000000.storyName = "scores: 10,0,0,0,0,0";

export const Scores222222 = (): React.ReactNode => (
	<ProgressBar
		statistics={ {
			notRated: 2,
			rate1: 2,
			rate2: 2,
			rate3: 2,
			rate4: 2,
			rate5: 2,
		} }
		totalFlashcardsCount={ 12 }
	/>
);

Scores222222.storyName = "scores: 2,2,2,2,2,2";
