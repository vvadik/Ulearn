import React from "react";
import ScoreControls, { Props } from "./ScoreControls";
import type { Story } from "@storybook/react";

export const Default: Story<Props> = (args: Props) => <ScoreControls { ...args }/>;

Default.args = {
	exerciseTitle: 'Angry Bird',
	prevReviewScore: 25,

	onSubmit: () => ({}),
	onToggleChange: () => ({}),
};


export default {
	title: 'Exercise/InstructorReview/ScoreControls',
};
