import React, { Component } from "react";
import PropTypes from 'prop-types';
import NavigationHeader from '../NavigationHeader';
import NavigationContent from '../NavigationContent';
import { menuItemType } from '../../types';
import styles from './Navigation.less';


class Navigation extends Component {
	render () {
		const { items } = this.props;
		return (
			<aside className={ styles.root }>
				<NavigationHeader {...this.props} />
				<NavigationContent items={ items } />
			</aside>
		);
	}
}

Navigation.propTypes ={
	title: PropTypes.string.isRequired,
	items: PropTypes.arrayOf(menuItemType),

	courseName: PropTypes.string,
	courseUrl: PropTypes.string,
};

export default Navigation
