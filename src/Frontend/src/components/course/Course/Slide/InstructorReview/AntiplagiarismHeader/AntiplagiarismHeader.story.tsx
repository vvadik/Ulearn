import React from "react";
import AntiplagiarismHeader, { AntiplagiarismInfo, Props } from "./AntiplagiarismHeader";

import { Story } from "@storybook/react";
import { mockFunc, returnPromiseAfterDelay } from "src/utils/storyMock";
import { Gapped } from "ui";

interface PropsWithDecorator extends Props {
	title: string;
}

const Template: Story<PropsWithDecorator[]> = (args) => (
	<Gapped vertical>
		{ Object.values(args).map(arg => (
			<div key={ arg.title }>
				<h1>{ arg.title }</h1>
				<AntiplagiarismHeader { ...arg } />
			</div>)) }
	</Gapped>);


export const List = Template.bind({});
List.args = [
	{
		title: 'notChecking',
		getAntiPlagiarismStatus: () =>
			(returnPromiseAfterDelay<AntiplagiarismInfo>(0, { suspicionCount: 2, suspicionLevel: 'warning' })),
		shouldCheck: false,
	},
	{
		title: 'checking -> long running',
		getAntiPlagiarismStatus: () =>
			(returnPromiseAfterDelay<AntiplagiarismInfo>(50000000, { suspicionCount: 0, suspicionLevel: 'accepted' })),
		shouldCheck: true,
	},
	{
		title: 'checking -> running for 2 secs',
		getAntiPlagiarismStatus: () =>
			(returnPromiseAfterDelay<AntiplagiarismInfo>(2000, { suspicionCount: 0, suspicionLevel: 'accepted' })),
		shouldCheck: true,
	},
	{
		title: 'checking -> accepted',
		getAntiPlagiarismStatus: () =>
			(returnPromiseAfterDelay<AntiplagiarismInfo>(0, { suspicionCount: 0, suspicionLevel: 'accepted' })),
		shouldCheck: true,
	},
	{
		title: 'checking -> suspicions with 5',
		getAntiPlagiarismStatus: () =>
			(returnPromiseAfterDelay<AntiplagiarismInfo>(0, { suspicionCount: 5, suspicionLevel: 'warning' })),
		shouldCheck: true,
	},
	{
		title: 'checking -> strong suspicions with 15',
		getAntiPlagiarismStatus: () =>
			(returnPromiseAfterDelay<AntiplagiarismInfo>(0, { suspicionCount: 15, suspicionLevel: 'strongWarning' })),
		shouldCheck: true,
	},
].map(a => ({
	...a, fixed: false,
	onZeroScoreButtonPressed: mockFunc,
}));

export default {
	title: 'Exercise/InstructorReview/AntiplagiarismHeader',
};
