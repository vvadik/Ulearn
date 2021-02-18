import React from "react";

import NavigationItem from '../NavigationItem';

import getPluralForm from 'src/utils/getPluralForm';

import { MenuItem, QuizMenuItem, } from '../../types';
import { SlideType } from 'src/models/slide';

import styles from './NavigationContent.less';


interface Props {
	items: MenuItem<SlideType>[];
	onClick: () => void;
}

function NavigationContent({ items, onClick, }: Props): React.ReactElement {
	return (
		<div className={ styles.root }>
			<h5 className={ styles.header }>Программа модуля</h5>
			{ items.map(renderItem) }
		</div>
	);

	function renderItem(menuItem: MenuItem<SlideType>, index: number, items: MenuItem<SlideType>[]) {
		const isFirstItem = index === 0;
		const isLastItem = index === items.length - 1;

		const metroSettings = {
			isFirstItem: isFirstItem,
			isLastItem: isLastItem,
			connectToPrev: menuItem.visited && (isFirstItem || items[index - 1].visited) || false,
			connectToNext: menuItem.visited && (isLastItem || items[index + 1].visited) || false,
		};

		return (
			<NavigationItem
				{ ...menuItem }
				key={ menuItem.id }
				description={ createDescription(menuItem) }
				metro={ metroSettings }
				onClick={ onClick }
			/>
		);
	}

	function createDescription(item: MenuItem<SlideType>) {
		if(item.type === SlideType.Quiz && (item as QuizMenuItem).questionsCount) {
			const count = (item as QuizMenuItem).questionsCount;
			return `${ count } ${ getPluralForm(count, 'вопрос', 'вопроса', 'вопросов') }`;
		}

		return null;
	}
}

export default NavigationContent;
