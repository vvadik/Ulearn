import React from "react";
import AddCommentForm, { Props } from "./AddCommentForm";
import type { Story } from "@storybook/react";
import { mockFunc, returnPromiseAfterDelay } from "src/utils/storyMock";
import { useArgs } from '@storybook/client-api';

const Template: Story<Props> = (args: Props) => {
	const [, updateArgs] = useArgs();

	const onValueChange = (value: string) => {
		updateArgs({ ...args, value });
	};
	return (<AddCommentForm  { ...args } onValueChange={ onValueChange }/>);
};

const comments = [
	{
		text: 'Вот это выглядит довольно непонятно: зачем делать цикл <=, а потом ка каждом (!) шаге проверять не превысило ли i допустимое значение. Постарайтесь обойтись без этого, если что-то непонятно - задавайте вопросы ',
		isFavourite: true,
	},
	{
		text: 'Лучше TryGetValue',
		isFavourite: true,
	},
	{
		text: 'Сложность вашей реализации этого метода больше O(document.Length)',
	},
	{
		text: 'Медленно',
	},
	{
		text: 'Вы не оставите никакого мусора в своём классе после выполнения этой операции?',
	},
].map((c, index) => ({ ...c, renderedText: c.text, useCount: index, id: index }));

const addComment = (comment: string) => {
	const newComment = { text: comment, renderedText: comment, isFavourite: false, useCount: 1, id: comments.length };
	comments.push(newComment);
	return returnPromiseAfterDelay(100, newComment);
};

const addCommentToFavourite = (comment: string) => {
	const newComment = { text: comment, renderedText: comment, isFavourite: true, useCount: 1, id: comments.length };
	comments.push(newComment);
	return returnPromiseAfterDelay(100, newComment);
};

function toggleCommentFavourite(commentId: number) {
	const comment = comments.find(c => c.id === commentId);
	if(!comment) {
		return;
	}
	comment.isFavourite = !comment.isFavourite;

	return returnPromiseAfterDelay(100);
}

const args = {
	comments,
	addCommentToFavourite,
	addComment,
	toggleCommentFavourite,
	onClose: mockFunc,
	coordinates: { left: 0, top: 0, bottom: 0 },
	value: '',
};

export const Default = Template.bind(args);
Default.args = args;


export default {
	title: 'Exercise/InstructorReview/AddCommentForm',
};
