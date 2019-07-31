import React, { Component } from "react";
import PropTypes from 'prop-types';
import { connect } from "react-redux";

import NavigationHeader from './Unit/NavigationHeader';
import NavigationContent from './Unit/NavigationContent';
import { courseMenuItemType, menuItemType } from './types';
import NextUnit from "./Unit/NextUnit";

import CourseNavigationHeader from "./Course/CourseNavigationHeader";
import CourseNavigationContent from "./Course/CourseNavigationContent";
import Flashcards from "./Course/Flashcards/Flashcards";
import { flashcards } from "../../../consts/routes";

import { toggleNavigation } from "../../../actions/navigation";

import styles from './Navigation.less';

const overflow = {
	hidden: 'hidden',
	auto: 'auto',
};

class Navigation extends Component {
	componentDidMount() {
		this.bodyElement = document.getElementsByTagName('body')[0];

		this.bodyElement
			.style.overflow = overflow.hidden;
	}

	componentWillUnmount() {
		this.bodyElement
			.style.overflow = overflow.auto;
	}

	componentDidUpdate(prevProps, prevState, snapshot) {
		const { navigationOpened } = this.props;

		this.bodyElement
			.style.overflow = navigationOpened
			? overflow.hidden
			: overflow.auto;
	}

	render() {
		const {
			courseItems, courseTitle, description, courseId, slideId, containsFlashcards,
			unitItems, unitTitle, nextUnit, onCourseClick,
		} = this.props;

		return (
			<aside className={ styles.root }>
				<div className={ styles.overlay } onClick={ this.props.toggleNavigation }/>
				{ unitTitle

					? Navigation.renderUnitNavigation(
						unitTitle,
						courseTitle,
						onCourseClick,
						unitItems,
						nextUnit)

					: Navigation.renderCourseNavigation(
						courseTitle,
						description,
						courseItems,
						containsFlashcards,
						courseId,
						slideId)
				}
			</aside>
		);
	}

	static renderUnitNavigation(title, courseName, onCourseClick, items, nextUnit) {
		return (
			<div className={ styles.contentWrapper }>
				< NavigationHeader title={ title } courseName={ courseName } onCourseClick={ onCourseClick }/>
				< NavigationContent items={ items }/>
				{ nextUnit && <NextUnit unit={ nextUnit }/> }
			</div>
		)
	}

	static renderCourseNavigation(title, description, items, containsFlashcards, courseId, slideId) {
		return (
			<div className={ styles.contentWrapper }>
				<CourseNavigationHeader title={ title } description={ description }/>
				{ items && items.length && <CourseNavigationContent items={ items }/> }
				{ containsFlashcards &&
				<Flashcards courseId={ courseId } isActive={ slideId === flashcards }/> }
			</div>
		)
	}
}

Navigation.propTypes = {
	navigationOpened: PropTypes.bool,
	courseTitle: PropTypes.string,

	courseId: PropTypes.string,
	description: PropTypes.string,
	courseItems: PropTypes.arrayOf(PropTypes.shape(courseMenuItemType)),
	slideId: PropTypes.string,
	containsFlashcards: PropTypes.bool,

	unitTitle: PropTypes.string,
	unitItems: PropTypes.arrayOf(PropTypes.shape(menuItemType)),
	nextUnit: PropTypes.object, // TODO: описать нормально

	onCourseClick: PropTypes.func,

	toggleNavigation: PropTypes.func,
};

const mapStateToProps = (state) => {
	return {};
};

const mapDispatchToProps = (dispatch) => ({
	toggleNavigation: () => dispatch(toggleNavigation()),
});

export default connect(mapStateToProps, mapDispatchToProps)(Navigation);
