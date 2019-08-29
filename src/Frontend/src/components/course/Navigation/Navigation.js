import React, { Component } from "react";
import PropTypes from 'prop-types';
import { connect } from "react-redux";

import NavigationHeader from './Unit/NavigationHeader';
import NavigationContent from './Unit/NavigationContent';
import NextUnit from "./Unit/NextUnit";

import CourseNavigationHeader from "./Course/CourseNavigationHeader";
import CourseNavigationContent from "./Course/CourseNavigationContent";
import Flashcards from "./Course/Flashcards/Flashcards";

import { courseMenuItemType, menuItemType, groupAsStudentType } from './types';
import { flashcards } from "../../../consts/routes";

import { toggleNavigation } from "../../../actions/navigation";

import styles from './Navigation.less';

class Navigation extends Component {
	constructor(props) {
		super(props);

		this.state = {
			windowWidth: window.innerWidth,
		}
	}

	componentDidMount() {
		window.addEventListener('resize', this.handleWindowSizeChange);
	}

	handleWindowSizeChange = () => {
		this.setState({ windowWidth: window.innerWidth });
	};

	componentWillUnmount() {
		window.removeEventListener('resize', this.handleWindowSizeChange);
	}

	componentDidUpdate(prevProps, prevState, snapshot) {
		const { navigationOpened } = this.props;
		const { windowWidth } = this.state;
		const isMobile = windowWidth <= 767;

		if (isMobile && prevProps.navigationOpened !== navigationOpened) {
			document.querySelector('body')
				.classList.toggle(styles.overflow, navigationOpened);
		}
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
		const { unitTitle, courseTitle, onCourseClick, unitItems, nextUnit, toggleNavigation, groupsAsStudent, } = this.props;

		return (
			<div className={ styles.contentWrapper }>
				< NavigationHeader
					createRef={ (ref) => this.unitHeaderRef = ref }
					title={ unitTitle }
					courseName={ courseTitle }
					onCourseClick={ onCourseClick }
					groupsAsStudent={ groupsAsStudent }
				/>
				< NavigationContent items={ unitItems } toggleNavigation={ toggleNavigation }/>
				{ nextUnit && <NextUnit unit={ nextUnit } toggleNavigation={ this.handleToggleNavigation }
				/> }
			</div>
		)
	}

	handleToggleNavigation = () => {
		const { toggleNavigation, } = this.props;
		this.unitHeaderRef.scrollIntoView();
		toggleNavigation();
	};

	renderCourseNavigation() {
		const { courseTitle, description, courseItems, containsFlashcards, courseId, slideId, toggleNavigation, groupsAsStudent } = this.props;

		return (
			<div className={ styles.contentWrapper }>
				<CourseNavigationHeader
					title={ courseTitle }
					description={ description }
					groupsAsStudent={ groupsAsStudent }
				/>
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
	groupAsStudent: PropTypes.arrayOf(PropTypes.shape(groupAsStudentType)),

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
	const courseId = state.courses.currentCourseId.toLowerCase();
	const groupsAsStudent = state.account.groupAsStudent;
	const courseGroupsAsStudent = groupsAsStudent
		? groupsAsStudent.filter(group => group.courseId.toLowerCase() === courseId && !group.isArchived)
		: [];

	return { groupsAsStudent: courseGroupsAsStudent, };
};

const mapDispatchToProps = (dispatch) => ({
	toggleNavigation: () => dispatch(toggleNavigation()),
});

export default connect(mapStateToProps, mapDispatchToProps)(Navigation);
