import GoogleSheet, {Props} from './GoogleSheet';
import { Meta, Story } from "@storybook/react";
import React from "react";

const ListTemplate: Story<{ items: {props: Props, header: string,}[] }>
	= ({ items }) => {
	return <>
		{ items.map((item) =>
			<>
				<p>{ item.header }</p>
				<GoogleSheet { ...item.props } />
			</>
		) }
	</>;
};

export const Default = ListTemplate.bind({});

Default.args = {
	items:[
		{
			props:{columnName:'title'},
			header:'Default',
		}
	]
};


export default {
	title: 'GoogleSheet',
} as Meta;
