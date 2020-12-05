import React, { Component } from "react";
import PropTypes from 'prop-types';
import NavigationItem from '../NavigationItem';
import styles from './NavigationContent.less';
import { menuItemType } from '../../types';
import { SlideType } from 'src/models/slide';
import getPluralForm from 'src/utils/getPluralForm'


class NavigationContent extends Component {
	render() {
		const { items } = this.props;
		return (
			<div className={ styles.root }>
				{ NavigationContent.renderTitle() }
				{ items.map((item, index) => this.renderItem(item, index)) }
			</div>
		);
	}

	static renderTitle() {
		return (
			<h5 className={ styles.header }>Программа модуля</h5>
		);
	}

	renderItem(menuItem, index) {
		const { items, toggleNavigation } = this.props;

		const isFirstItem = index === 0;
		const isLastItem = index === items.length - 1;

		const metroSettings = {
			isFirstItem: isFirstItem,
			isLastItem: isLastItem,
			connectToPrev: menuItem.visited && (isFirstItem || items[index - 1].visited),
			connectToNext: menuItem.visited && (isLastItem || items[index + 1].visited),
		};

		return (
			<NavigationItem
				key={ menuItem.id }
				text={ menuItem.title }
				url={ menuItem.url }
				type={ menuItem.type }
				score={ menuItem.score }
				maxScore={ menuItem.maxScore }
				isActive={ menuItem.isActive }
				visited={ menuItem.visited }
				description={ NavigationContent.createDescription(menuItem) }
				metro={ metroSettings }
				toggleNavigation={ toggleNavigation }
				hide={ menuItem.hide }
			/>
		);


	}

	static createDescription(item) {
		if(item.type === SlideType.Quiz && item.questionsCount) {
			const count = item.questionsCount;
			return `${ count } ${ getPluralForm(count, 'вопрос', 'вопроса', 'вопросов') }`;
		}

		return null;
	}
}

NavigationContent.propTypes = {
	items: PropTypes.arrayOf(PropTypes.shape(menuItemType)),

	toggleNavigation: PropTypes.func,
};

export default NavigationContent
