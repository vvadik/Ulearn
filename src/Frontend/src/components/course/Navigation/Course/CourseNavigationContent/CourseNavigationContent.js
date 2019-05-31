import React, { Component } from "react";
import PropTypes from 'prop-types';
import CourseNavigationItem from '../CourseNavigationItem';
import styles from './CourseNavigationContent.less';
import { menuItemType } from '../../types';


class CourseNavigationContent extends Component {
	render () {
		const { items } = this.props;
		return (
			<div className={ styles.root }>
				<h5 className={ styles.header }>Программа курса</h5>
				{ items.map((item) => this.renderItem(item)) }
			</div>
		);
	}

	renderItem(menuItem) {
		return (
			<CourseNavigationItem
				key={ menuItem.url }
				text={ menuItem.title }
				url={ menuItem.url }
				progress={ menuItem.progress } />
		);
	}
}

CourseNavigationContent.propTypes ={
	items: PropTypes.arrayOf(menuItemType)
};

export default CourseNavigationContent
