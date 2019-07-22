import React, { Component } from "react";
import PropTypes from 'prop-types';
import NavigationHeader from '../NavigationHeader';
import NavigationContent from '../NavigationContent';
import { menuItemType } from '../../types';
import styles from './Navigation.less';
import NextUnit from "../NextUnit";


class Navigation extends Component {
	render () {
		const { items, nextUnit } = this.props;
		return (
			<aside className={ styles.root }>
				<div className={ styles.contentWrapper }>
					<NavigationHeader {...this.props} />
					<NavigationContent items={ items } />
					{ nextUnit && <NextUnit unit={ nextUnit } />}
				</div>
			</aside>
		);
	}
}

Navigation.propTypes ={
	title: PropTypes.string.isRequired,
	items: PropTypes.arrayOf(PropTypes.shape(menuItemType)),
	nextUnit: PropTypes.object, // TODO: описать нормально

	courseName: PropTypes.string,
	onCourseClick: PropTypes.func,
};

export default Navigation
