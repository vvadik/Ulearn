import React from 'react';

import { AcceptedSolutionsModal, AcceptedSolutionsProps } from './AcceptedSolutions';
import type { Story } from "@storybook/react";

const Template : Story<AcceptedSolutionsProps> = (args: AcceptedSolutionsProps) =>
	<AcceptedSolutionsModal { ...args } />;
