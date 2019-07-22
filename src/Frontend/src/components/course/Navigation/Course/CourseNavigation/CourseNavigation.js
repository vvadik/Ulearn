import React, {Component} from "react";
import PropTypes from "prop-types";
import styles from './CourseNavigation.less';
import CourseNavigationHeader from "../CourseNavigationHeader";
import CourseNavigationContent from "../CourseNavigationContent"
import {courseMenuItemType} from "../../types";
import Flashcards from "../Flashcards";
import {flashcards} from "../../../../../consts/routes";

class CourseNavigation extends Component {
	render() {
		const {title, description, items, courseId, slideId} = this.props;

		return (
			<aside className={styles.root}>
				<div className={styles.contentWrapper}>
					<CourseNavigationHeader title={title} description={description}/>
					{items && items.length && <CourseNavigationContent items={items}/>}
					<Flashcards courseId={courseId} isActive={slideId === flashcards}/>
				</div>
			</aside>
		);
	}

}

CourseNavigation.propTypes = {
	title: PropTypes.string,
	description: PropTypes.string,
	items: PropTypes.arrayOf(PropTypes.shape(courseMenuItemType)),
	courseId: PropTypes.string,
	slideId: PropTypes.string,
};

export default CourseNavigation
