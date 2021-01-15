import React from "react";
import type { Story } from "@storybook/react";

import Controls from "./Controls";

const defaultProps = {
	isEditable: false,
	hasOutput: false,
	hideSolutions: false,
	isShowAcceptedSolutionsAvailable: false,

	valueChanged: false,
	submissionLoading: false,

	hints: ['123'],
	showedHintsCount: 0,

	showOutput: false,

	attemptsStatistics: {
		attemptedUsersCount: 0,
		usersWithRightAnswerCount: 0,
	},

	onSendExerciseButtonClicked: () => ({}),
	showHint: () => ({}),
	resetCodeAndCache: () => ({}),
	toggleOutput: () => ({}),
	onVisitAcceptedSolutions: () => ({}),
};

const ListTemplate: Story<{ items: { props: typeof defaultProps, header: string }[] }> = ({ items }) => {
	return <>
		{ items.map(({ header, props }) =>
			<>
				<p>{ header }</p>
				<Controls>
					<Controls.SubmitButton
						valueChanged={ props.valueChanged }
						submissionLoading={ props.submissionLoading }
						onSendExerciseButtonClicked={ props.onSendExerciseButtonClicked }
					/>
					{ props.hints.length !== 0 &&
					<Controls.ShowHintButton
						onAllHintsShowed={()=>{}}
						renderedHints={ props.hints }
					/> }
					{ props.isEditable && <Controls.ResetButton onResetButtonClicked={ props.resetCodeAndCache }/> }
					{ (!props.isEditable && props.hasOutput) && <Controls.OutputButton
						showOutput={ props.showOutput }
						onShowOutputButtonClicked={ props.toggleOutput }
					/> }
					<Controls.StatisticsHint attemptsStatistics={ props.attemptsStatistics }/>
					{ (!props.hideSolutions && (props.hints.length === props.showedHintsCount || props.isShowAcceptedSolutionsAvailable))
					&& <Controls.AcceptedSolutionsButton
						acceptedSolutionsUrl={ '' }
						onVisitAcceptedSolutions={ props.onVisitAcceptedSolutions }
						isShowAcceptedSolutionsAvailable={ props.isShowAcceptedSolutionsAvailable }
					/> }
				</Controls>
			</>
		) }
	</>;
};

export const AllControls = ListTemplate;
AllControls.args = { items: [{ props: defaultProps, header: 'default' }] };

export default {
	title: 'Exercise/Controls',
	component: Controls,
};
