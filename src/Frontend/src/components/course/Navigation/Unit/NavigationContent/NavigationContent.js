import React, { Component } from "react";
import PropTypes from 'prop-types';
import classnames from 'classnames';
import NavigationItem from '../NavigationItem';
import styles from './NavigationContent.less';
import { menuItemType } from '../../types';


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
			complete: menuItem.complete,
			isFirstItem: isFirstItem,
			isLastItem: isLastItem,
			connectToPrev: menuItem.complete && (isFirstItem || items[index - 1].complete),
			connectToNext: menuItem.complete && (isLastItem || items[index + 1].complete),
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
				metro={ metroSettings }
			/>
		);


	}
}

NavigationContent.propTypes ={
	items: PropTypes.arrayOf(PropTypes.shape(menuItemType)),
};

export default NavigationContent
