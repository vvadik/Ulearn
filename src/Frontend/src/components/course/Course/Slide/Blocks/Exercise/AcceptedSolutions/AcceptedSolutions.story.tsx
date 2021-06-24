import React from 'react';

import { AcceptedSolutionsModal, AcceptedSolutionsProps } from './AcceptedSolutions';
import type { Story } from "@storybook/react";
import { AcceptedSolutionsApi } from "src/api/acceptedSolutions";
import {
	AcceptedSolution,
	AcceptedSolutionsResponse,
	LikedAcceptedSolutionsResponse
} from "src/models/acceptedSolutions";
import { Language } from "src/consts/languages";

const Template: Story<AcceptedSolutionsProps> = (args: AcceptedSolutionsProps) =>
	<AcceptedSolutionsModal { ...args } />;

const getAcceptedSolutionsApi = (promotedSolutions: AcceptedSolution[], randomLikedSolutions: AcceptedSolution[],
	newestSolutions: AcceptedSolution[], likedSolutions: AcceptedSolution[]
): AcceptedSolutionsApi => {
	return {
		getAcceptedSolutions: (courseId: string, slideId: string) => {
			const acceptedSolutionsResponse: AcceptedSolutionsResponse = {
				promotedSolutions: [],
				randomLikedSolutions: [],
				newestSolutions: [],
			};
			return Promise.resolve(acceptedSolutionsResponse);
		},
		getLikedAcceptedSolutions: (courseId: string, slideId: string, offset: number, count: number) => {
			const likedSolutionsResponse: LikedAcceptedSolutionsResponse = { likedSolutions: [] };
			return Promise.resolve(likedSolutionsResponse);
		},
		likeAcceptedSolution: (solutionId: string) => Promise.resolve({} as Response),
		dislikeAcceptedSolution: (solutionId: string) => Promise.resolve({} as Response),
		promoteAcceptedSolution: (courseId: string, solutionId: string) => Promise.resolve({} as Response),
		unpromoteAcceptedSolution: (courseId: string, solutionId: string) => Promise.resolve({} as Response),
	};
};

const acceptedSolution: AcceptedSolution = {
	submissionId: 1,
	code: "var a = 1\nvar a = 1\n",
	language: Language.cSharp,
	likesCount: null,
	likedByMe: null,
};

export const emptyAcceptedSolutions = Template.bind({});
emptyAcceptedSolutions.args = {
	courseId: "",
	slideId: "",
	userId: "",
	isInstructor: true,
	onClose: () => {},
	acceptedSolutionsApi: getAcceptedSolutionsApi([], [], [], []),
}

export default {
	title: "Exercise/AcceptedSolutions",
	component: AcceptedSolutionsModal,
}
