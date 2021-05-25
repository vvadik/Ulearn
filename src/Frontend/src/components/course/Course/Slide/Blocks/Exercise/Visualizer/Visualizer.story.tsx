import React from 'react';

import { Visualizer, VisualizerProps } from './Visualizer';
import type { Story } from "@storybook/react";



const props: VisualizerProps = {
	code: "print(1000)",
	input: "2\n2",
}

const Template : Story<VisualizerProps> = (args: VisualizerProps) =>
	<Visualizer { ...args } />;

export const FirstStory = Template.bind({});
FirstStory.args = {
	...props,
}

export default {
	title: "Exercise/Visualizer",
	component: Visualizer,
}
