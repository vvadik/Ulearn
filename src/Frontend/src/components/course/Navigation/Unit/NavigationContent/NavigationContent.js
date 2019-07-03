import React, { Component } from "react";
import PropTypes from 'prop-types';
import classnames from 'classnames';
import NavigationItem from '../NavigationItem';
import styles from './NavigationContent.less';
import { menuItemType } from '../../types';
import { itemTypes } from '../../constants';
import getPluralForm from '../../../../../utils/getPluralForm'


class NavigationContent extends Component {
	render () {
		const { items } = this.props;
		return (
			<div className={ styles.root }>
				{ this.renderTitle() }
				{ items.map((item, index) => this.renderItem(item, index)) }
			</div>
		);
	}

	renderTitle () {
		return (
			<h5 className={ classnames(styles.header ) }>Программа модуля</h5>
		);
	}

	renderItem(menuItem, index) {
		const { items } = this.props;

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
				description={ this.createDescription(menuItem) }
				metro={ metroSettings }
			/>
		);


	}

	createDescription(item) {
		if (item.type === itemTypes.quiz && item.questionsCount) {
			const count = item.questionsCount;
			return `${count} ${getPluralForm(count, 'вопрос', 'вопроса', 'вопросов')}`;
		}

		return null;
	}
}

NavigationContent.propTypes ={
	items: PropTypes.arrayOf(PropTypes.shape(menuItemType)),
};

export default NavigationContent
