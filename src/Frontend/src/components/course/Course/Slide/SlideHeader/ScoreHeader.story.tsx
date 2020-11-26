import React from "react";
import { Provider } from 'react-redux';
import type { Meta, Story } from "@storybook/react";
import configureStore from "redux-mock-store";

import { ScoreHeader, ScoreHeaderPropsFromRedux } from "./ScoreHeader";
import { RootState } from "src/models/reduxState";

const courseId = "courseId";
const slideId = "slideId";

const ListTemplate: Story<{ items: { props: ScoreHeaderPropsFromRedux, header: string }[] }>
	= ({ items }) => {
	return <>
		{ items.map((item) =>
			<>
				<p>{ item.header }</p>
				<Provider store={ GetStore(item.props) }>
					<ScoreHeader { ...item.props } />
				</Provider>
			</>
		) }
	</>;
};

const reduxProps: ScoreHeaderPropsFromRedux = {
	slideId,
	courseId,
	isInstructor: false,
	score: 10,
	isSkipped: false,
	waitingForManualChecking: false,
	prohibitFurtherManualChecking: false,
	maxScore: 50,
	hasReviewedSubmissions: false
};

export const AllHeaders = ListTemplate.bind({});
AllHeaders.args = {
	items: [
		{ props: { ...reduxProps, score: 50 }, header: "Полный балл" },
		{ props: { ...reduxProps, score: 0 }, header: "0 баллов" },
		{ props: { ...reduxProps, isSkipped: true }, header: "Пропущено" },
		{ props: { ...reduxProps, waitingForManualChecking: true }, header: "Ожидает ревью" },
		{ props: { ...reduxProps, waitingForManualChecking: true, score: 50 }, header: "Ожидает ревью и полный балл" },
		{ props: { ...reduxProps, prohibitFurtherManualChecking: true }, header: "Ревью запрещено" },
		{
			props: { ...reduxProps, prohibitFurtherManualChecking: true, score: 50 },
			header: "Ревью запрещено и полный балл"
		},
		{ props: { ...reduxProps, hasReviewedSubmissions: true }, header: "Прошел ревью, неполный балл" },
		{ props: { ...reduxProps, hasReviewedSubmissions: true, score: 50 }, header: "Прошел ревью, полный балл" },
		//Instructor cases
		{ props: { ...reduxProps, isSkipped: true, isInstructor: true }, header: "Пропущено (преподаватель)" },
		{ props: { ...reduxProps, score: 50, isInstructor: true }, header: "Полный балл (преподаватель)" },
		{ props: { ...reduxProps, score: 0, isInstructor: true }, header: "0 баллов (преподаватель)" },
	]
};

function GetStore(reduxProps: ScoreHeaderPropsFromRedux) {
	const {
		score, isSkipped, waitingForManualChecking, prohibitFurtherManualChecking, maxScore, hasReviewedSubmissions
	} = reduxProps;
	const state: DeepPartial<RootState> = {
		userProgress: {
			progress: {
				[courseId]: {
					[slideId]: {
						score: score,
						isSkipped: isSkipped,
						waitingForManualChecking: waitingForManualChecking,
						prohibitFurtherManualChecking: prohibitFurtherManualChecking,
					}
				}
			}
		},
		courses: { fullCoursesInfo: { [courseId]: { units: [{ slides: [{ id: slideId, maxScore: maxScore }] }] } } },
		slides: { submissionsByCourses: { [courseId]: { [slideId]: { 1: { manualCheckingPassed: hasReviewedSubmissions } } } } },
	};
	const mockStore = configureStore();
	return mockStore(state);
}

export default {
	title: 'Slide/ScoreHeader',
	argTypes: {
		items: { table: { disable: true } },
	}
} as Meta;
