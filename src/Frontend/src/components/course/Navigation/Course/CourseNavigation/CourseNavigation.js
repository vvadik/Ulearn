import React, { Component } from "react";
import PropTypes from "prop-types";
import styles from './CourseNavigation.less';
import CourseNavigationHeader from "../CourseNavigationHeader";
import CourseNavigationContent from "../CourseNavigationContent"
import { courseMenuItemType } from "../../types";

class CourseNavigation extends Component {
	render () {
		const { title, description, progress, items } = this.props;

		return (
			<aside className={ styles.root }>
				<CourseNavigationHeader title={ title } description={ description } progress={ progress } />
				{ items && items.length && <CourseNavigationContent items={ items } /> }
			</aside>
		);
	}

}

CourseNavigation.propTypes ={
	title: PropTypes.string,
	description: PropTypes.string,
	progress: PropTypes.number,
	items: PropTypes.arrayOf(PropTypes.shape(courseMenuItemType)),
};

export default CourseNavigation
