import React, { Component } from "react";
import PropTypes from 'prop-types';
import { connect } from "react-redux";

import NavigationHeader from './Unit/NavigationHeader';
import NavigationContent from './Unit/NavigationContent';
import NextUnit from "./Unit/NextUnit";

import CourseNavigationHeader from "./Course/CourseNavigationHeader";
import CourseNavigationContent from "./Course/CourseNavigationContent";
import Flashcards from "./Course/Flashcards/Flashcards";

import { courseMenuItemType, menuItemType, groupAsStudentType, progressType } from './types';
import { flashcards } from "src/consts/routes";

import { isMobile } from "src/utils/getDeviceType";
import { toggleNavigation } from "src/actions/navigation";

import styles from './Navigation.less';

class Navigation extends Component {
	constructor(props) {
		super(props);

		this.state = {
			windowWidth: window.innerWidth,
			xDown: null,
			yDown: null,
			touchListenerAdded: false,
		}
	}

	componentDidMount() {
		window.addEventListener('resize', this.handleWindowSizeChange);

		this.tryAddTouchListener();
	}

	tryAddTouchListener = () => {
		if(isMobile() && !this.state.touchListenerAdded) {
			document.addEventListener('touchstart', this.handleTouchStart);
			document.addEventListener('touchmove', this.handleTouchMove);
			this.setState({
				touchListenerAdded: true,
			})
		}
	}

	getTouches = (evt) => {
		return evt.touches ||             // browser API
			evt.originalEvent.touches; // jQuery
	}

	handleTouchStart = (evt) => {
		const { clientX, clientY, } = this.getTouches(evt)[0];

		this.setState({
			xDown: clientX,
			yDown: clientY,
		})
	};

	handleTouchMove = (evt) => {
		const { xDown, yDown, } = this.state;
		const { navigationOpened, toggleNavigation, } = this.props;

		if(!xDown || !yDown) {
			return;
		}

		const { clientX, clientY, target, } = evt.touches[0];

		const xDiff = xDown - clientX;
		const yDiff = yDown - clientY;

		if(Math.abs(xDiff) > Math.abs(yDiff)) {
			if(xDiff > 0 && navigationOpened) {
				/* left swipe */
				toggleNavigation();
			} else if(target === this.root && !navigationOpened) {
				/* right swipe */
				toggleNavigation();
			}
		}

		this.setState({
			xDown: null,
			yDown: null,
		})
	};

	handleWindowSizeChange = () => {
		this.setState({ windowWidth: window.innerWidth });
		this.tryAddTouchListener();
	};

	componentWillUnmount() {
		window.removeEventListener('resize', this.handleWindowSizeChange);
		if(this.state.touchListenerAdded) {
			window.removeEventListener('touchstart', this.handleTouchStart);
			window.removeEventListener('touchmove', this.handleTouchMove);
		}
	}

	componentDidUpdate(prevProps, prevState, snapshot) {
		const { navigationOpened } = this.props;
		const { windowWidth } = this.state;
		const isMobile = windowWidth <= 767;

		if(isMobile && prevProps.navigationOpened !== navigationOpened) {
			document.querySelector('body')
				.classList.toggle(styles.overflow, navigationOpened);
		}
	}

	render() {
		const { unitTitle, toggleNavigation, } = this.props;

		return (
			<aside className={ styles.root } ref={ (ref) => this.root = ref }>
				<div className={ styles.overlay } onClick={ toggleNavigation }/>
				{ unitTitle
					? this.renderUnitNavigation()
					: this.renderCourseNavigation()
				}
			</aside>
		);
	}

	renderUnitNavigation() {
		const { unitTitle, courseTitle, onCourseClick, unitItems, nextUnit, toggleNavigation, groupsAsStudent, unitProgress } = this.props;

		return (
			<div className={ styles.contentWrapper }>
				< NavigationHeader
					createRef={ (ref) => this.unitHeaderRef = ref }
					title={ unitTitle }
					progress={ unitProgress }
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
		const { courseTitle, description, courseItems, containsFlashcards, courseId, slideId, toggleNavigation, groupsAsStudent, courseProgress } = this.props;

		return (
			<div className={ styles.contentWrapper }>
				<CourseNavigationHeader
					title={ courseTitle }
					description={ description }
					groupsAsStudent={ groupsAsStudent }
					courseProgress={ courseProgress }
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
	groupsAsStudent: PropTypes.arrayOf(PropTypes.shape(groupAsStudentType)),

	courseId: PropTypes.string,
	description: PropTypes.string,
	courseProgress: PropTypes.shape(progressType),
	courseItems: PropTypes.arrayOf(PropTypes.shape(courseMenuItemType)),
	slideId: PropTypes.string,
	containsFlashcards: PropTypes.bool,

	unitTitle: PropTypes.string,
	unitProgress: PropTypes.shape(progressType),
	unitItems: PropTypes.arrayOf(PropTypes.shape(menuItemType)),
	nextUnit: PropTypes.shape({
		title: PropTypes.string,
		slug: PropTypes.string,
	}),

	onCourseClick: PropTypes.func,
	toggleNavigation: PropTypes.func,
};

const mapStateToProps = (state) => {
	const { currentCourseId } = state.courses;
	const courseId = currentCourseId
		? currentCourseId.toLowerCase()
		: null;
	const groupsAsStudent = state.account.groupsAsStudent;
	const courseGroupsAsStudent = groupsAsStudent
		? groupsAsStudent.filter(group => group.courseId.toLowerCase() === courseId && !group.isArchived)
		: [];

	return { groupsAsStudent: courseGroupsAsStudent, };
};

const mapDispatchToProps = (dispatch) => ({
	toggleNavigation: () => dispatch(toggleNavigation()),
});

export default connect(mapStateToProps, mapDispatchToProps)(Navigation);
