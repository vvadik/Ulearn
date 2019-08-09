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

class Navigation extends Component {
	componentDidMount() {
		this.bodyElement = document.getElementsByTagName('body')[0];

		this.bodyElement
			.classList.add(styles.overflow);
	}

	componentWillUnmount() {
		this.bodyElement
			.classList.remove(styles.overflow);
	}

	componentDidUpdate(prevProps, prevState, snapshot) {
		const { navigationOpened } = this.props;

		this.bodyElement
			.classList.toggle(styles.overflow, navigationOpened);
	}

	render() {
		const { unitTitle, toggleNavigation, } = this.props;

		return (
			<aside className={ styles.root }>
				<div className={ styles.overlay } onClick={ toggleNavigation }/>
				{ unitTitle
					? this.renderUnitNavigation()
					: this.renderCourseNavigation()
				}
			</aside>
		);
	}

	renderUnitNavigation() {
		const { unitTitle, courseTitle, onCourseClick, unitItems, nextUnit, toggleNavigation } = this.props;

		return (
			<div className={ styles.contentWrapper }>
				< NavigationHeader createRef={ (ref) => this.unitHeaderRef = ref } title={ unitTitle } courseName={ courseTitle }
								   onCourseClick={ onCourseClick }/>
				< NavigationContent items={ unitItems }/>
				{ nextUnit && <NextUnit unit={ nextUnit } toggleNavigation={ () => {
					this.unitHeaderRef.scrollIntoView();
					toggleNavigation();
				} }
				/> }
			</div>
		)
	}

	renderCourseNavigation() {
		const { courseTitle, description, courseItems, containsFlashcards, courseId, slideId, toggleNavigation } = this.props;

		return (
			<div className={ styles.contentWrapper }>
				<CourseNavigationHeader title={ courseTitle } description={ description }/>
				{ courseItems && courseItems.length && <CourseNavigationContent items={ courseItems }/> }
				{ containsFlashcards &&
				<Flashcards toggleNavigation={ toggleNavigation } courseId={ courseId }
							isActive={ slideId === flashcards }/> }
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
	nextUnit: PropTypes.shape({
		title: PropTypes.string,
		slug: PropTypes.string,
	}),

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
