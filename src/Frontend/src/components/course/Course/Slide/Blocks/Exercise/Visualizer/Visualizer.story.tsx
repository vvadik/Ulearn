import React from 'react';

import { Visualizer, VisualizerProps } from './Visualizer';
import type { Story } from "@storybook/react";

const Template : Story<VisualizerProps> = (args: VisualizerProps) =>
	<Visualizer { ...args } />;

export const Print1000 = Template.bind({});
Print1000.args = {
	code: "print(1000)",
	input: "",
}

export const aPlusB = Template.bind({});
aPlusB.args = {
	code: "a = int(input())\nb = int(input()\nprint(a + b)",
	input: "2\n2",
}

export const listAndDict = Template.bind({});
listAndDict.args = {
	code: "a =[1, 2, 3, 4]\nb = {'a': 1, 'b': 'aaaa'}",
	input: "",
}

export const nestedListAndDict = Template.bind({});
nestedListAndDict.args = {
	code: "a =[1, 2, [1, 2, 3]]\nb = {'a': 1, 'b': 'aaaa', 'c': {'a': 1, 'b': {'a': 1}}}",
	input: "",
}

export const defineFunction = Template.bind({});
defineFunction.args = {
	code: "def a(b):\n\tc = 6\n\treturn b + c\n\nk = 6\nprint(a(k + int(input())))",
	input: "10",
}

export default {
	title: "Exercise/Visualizer",
	component: Visualizer,
}
