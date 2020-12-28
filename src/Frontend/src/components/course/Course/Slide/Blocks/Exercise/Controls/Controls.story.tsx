import React from "react";
import type { Story } from "@storybook/react";

import Controls, {
	AcceptedSolutionsButton,
	OutputButton,
	ResetButton,
	ShowHintButton,
	StatisticsHint,
	SubmitButton
} from "./Controls";

const defaultProps = {
	isEditable: false,
	hasOutput: false,
	hideSolutions: false,
	isShowAcceptedSolutionsAvailable: false,

	valueChanged: false,
	submissionLoading: false,

	hints: [],
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
					<SubmitButton
						valueChanged={ props.valueChanged }
						submissionLoading={ props.submissionLoading }
						onSendExerciseButtonClicked={ props.onSendExerciseButtonClicked }
					/>
					{ props.hints.length !== 0 &&
					<ShowHintButton
						countOfHints={ props.hints.length }
						showedHintsCount={ props.showedHintsCount }
						showHint={ props.showHint }
					/> }
					{ props.isEditable && <ResetButton onResetButtonClicked={ props.resetCodeAndCache }/> }
					{ (!props.isEditable && props.hasOutput) && <OutputButton
						showOutput={ props.showOutput }
						onShowOutputButtonClicked={ props.toggleOutput }
					/> }
					<StatisticsHint attemptsStatistics={ props.attemptsStatistics }/>
					{ (!props.hideSolutions && (props.hints.length === props.showedHintsCount || props.isShowAcceptedSolutionsAvailable))
					&& <AcceptedSolutionsButton
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
ListTemplate.args = { items: [{ props: defaultProps, header: 'default' }] };

export default {
	title: 'Exercise/Controls',
	component: Controls,
};
