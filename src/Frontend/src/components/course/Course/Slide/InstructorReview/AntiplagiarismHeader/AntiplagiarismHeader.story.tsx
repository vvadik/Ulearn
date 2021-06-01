import React from "react";
import AntiplagiarismHeader, { Props } from "./AntiplagiarismHeader";

import { Story } from "@storybook/react";
import { returnPromiseAfterDelay } from "src/utils/storyMock";
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
			(returnPromiseAfterDelay(0)
				.then(() => ({ suspicionCount: 2, suspicionLevel: 'warning' }))),
		shouldCheck: false,
	},
	{
		title: 'checking -> long running',
		getAntiPlagiarismStatus: () =>
			(returnPromiseAfterDelay(50000000)
				.then(() => ({ suspicionCount: 0, suspicionLevel: 'accepted' }))),
		shouldCheck: true,
	},
	{
		title: 'checking -> running for 2 secs',
		getAntiPlagiarismStatus: () =>
			(returnPromiseAfterDelay(2000)
				.then(() => ({ suspicionCount: 0, suspicionLevel: 'accepted' }))),
		shouldCheck: true,
	},
	{
		title: 'checking -> accepted',
		getAntiPlagiarismStatus: () =>
			(returnPromiseAfterDelay(0)
				.then(() => ({ suspicionCount: 0, suspicionLevel: 'accepted' }))),
		shouldCheck: true,
	},
	{
		title: 'checking -> suspicions with 5',
		getAntiPlagiarismStatus: () =>
			(returnPromiseAfterDelay(0)
				.then(() => ({ suspicionCount: 5, suspicionLevel: 'warning' }))),
		shouldCheck: true,
	},
	{
		title: 'checking -> strong suspicions with 15',
		getAntiPlagiarismStatus: () =>
			(returnPromiseAfterDelay(0)
				.then(() => ({ suspicionCount: 15, suspicionLevel: 'strongWarning' }))),
		shouldCheck: true,
	},
];

export default {
	title: 'Exercise/InstructorReview/AntiplagiarismHeader',
};
