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

import { isMobile, isTablet } from "src/utils/getDeviceType";
import { toggleNavigation } from "src/actions/navigation";

import styles from './Navigation.less';

const mobileNavigationMenuWidth = 250;//250 is @mobileNavigationMenuWidth, its mobile nav menu width

class Navigation extends Component {
	constructor(props) {
		super(props);

		this.state = {
			overlayStyle: null,
			sideMenuStyle: null,
			xDown: null,
			yDown: null,
			touchListenerAdded: false,
		}
	}

	componentDidMount() {
		document.addEventListener('resize', this.handleWindowSizeChange);
		this.tryAddTouchListener();
	}

	isMobileNavigationMenu = () => {
		return isMobile() || isTablet();
	}

	tryAddTouchListener = () => {
		if(this.isMobileNavigationMenu() && !this.state.touchListenerAdded) {
			document.addEventListener('touchstart', this.handleTouchStart);
			this.setState({
				touchListenerAdded: true,
			})
		}
	}

	getTouches = (evt) => {
		return evt.touches || evt.changedTouches;
	}

	handleTouchStart = (evt) => {
		document.addEventListener('touchmove', this.handleTouchMove);
		document.addEventListener('touchend', this.handleTouchEnd);

		const { clientX, clientY, } = this.getTouches(evt)[0];
		const { navigationOpened } = this.props;
		const touchOnLeftSideOfScreen = clientX < window.innerWidth / 3;

		if((!navigationOpened && touchOnLeftSideOfScreen) || navigationOpened) {
			this.setState({
				xDown: clientX,
				yDown: clientY,
			})
		}
	};

	handleTouchEnd = (evt) => {
		const { clientX, } = evt.changedTouches[0];
		const { toggleNavigation, navigationOpened, } = this.props;
		const { xDown, yDown, overlayStyle, } = this.state;

		if(!xDown || !yDown) {
			return;
		}

		const moveDistance = Math.abs(xDown - clientX);
		const isDistanceEnough = moveDistance > mobileNavigationMenuWidth / 4; //if we showed/hided more then 1/4 of menu then toggle navigation
		if(isDistanceEnough) {
			const leftSwap = !navigationOpened && clientX > xDown;
			const rightSwap = navigationOpened && clientX < xDown;
			if(leftSwap || rightSwap) {
				toggleNavigation();
			}
		}

		if(overlayStyle) {
			this.playHidingOverlayAnimation();
		}

		this.setState({
			xDown: null,
			yDown: null,
			sideMenuStyle: null,
		})

		document.removeEventListener('touchmove', this.handleTouchMove);
		document.removeEventListener('touchend', this.handleTouchEnd);
	};

	handleTouchMove = (evt) => {
		const { xDown, yDown, } = this.state;
		const { navigationOpened } = this.props;

		if(!xDown || !yDown) {
			return;
		}

		const { clientX, clientY, } = evt.touches[0];

		const xDiff = xDown - clientX;
		const yDiff = yDown - clientY;

		if(Math.abs(xDiff) > Math.abs(yDiff)) {
			let diff, ratio;
			if(navigationOpened) {
				diff = -xDiff;
				ratio = 1 - Math.abs(Math.min(diff, 1) / mobileNavigationMenuWidth);
			} else {
				diff = -xDiff - mobileNavigationMenuWidth;
				ratio = 1 - Math.abs(Math.min(diff, 0) / mobileNavigationMenuWidth);
			}

			this.setState({
				overlayStyle: {
					visibility: 'visible',
					opacity: ratio,
					transition: 'unset',
				},
				sideMenuStyle: {
					transform: `translateX(${ Math.min(0, diff) }px)`,
					transition: 'unset',
				}
			})
		} else {
			this.setState({
				xDown: null,
				yDown: null,
				overlayStyle: null,
				sideMenuStyle: null,
			})
		}
	};

	handleWindowSizeChange = () => {
		this.tryAddTouchListener();
	};

	componentWillUnmount() {
		document.removeEventListener('resize', this.handleWindowSizeChange);
		if(this.state.touchListenerAdded) {
			document.removeEventListener('touchstart', this.handleTouchStart);
			document.removeEventListener('touchmove', this.handleTouchMove);
			document.removeEventListener('touchend', this.handleTouchEnd);
		}
	}

	componentDidUpdate(prevProps, prevState, snapshot) {
		const { navigationOpened } = this.props;

		if((isMobile() || isTablet()) && prevProps.navigationOpened !== navigationOpened) {
			document.querySelector('body')
				.classList.toggle(styles.overflow, navigationOpened);
			if(!navigationOpened) {
				this.playHidingOverlayAnimation();
			}
		}
	}

	playHidingOverlayAnimation = () => {
		this.setState({
			overlayStyle: {
				visibility: 'visible',
			},
		});
		setTimeout(() => {
			this.setState({
				overlayStyle: null,
			})
		}, 300);
	}

	hideNavigationMenu = () => {
		const { navigationOpened, toggleNavigation, } = this.props;

		if(navigationOpened) {
			toggleNavigation();
		}
	}

	render() {
		const { unitTitle, } = this.props;
		const { overlayStyle } = this.state;

		return (
			<aside>
				<div className={ styles.overlay } style={ overlayStyle } onClick={ this.hideNavigationMenu }/>
				{ unitTitle
					? this.renderUnitNavigation()
					: this.renderCourseNavigation()
				}
			</aside>
		);
	}

	renderUnitNavigation() {
		const { unitTitle, courseTitle, onCourseClick, unitItems, nextUnit, groupsAsStudent, unitProgress } = this.props;
		const { sideMenuStyle } = this.state;

		return (
			<div className={ styles.contentWrapper } style={ sideMenuStyle }>
				< NavigationHeader
					createRef={ (ref) => this.unitHeaderRef = ref }
					title={ unitTitle }
					progress={ unitProgress }
					courseName={ courseTitle }
					onCourseClick={ onCourseClick }
					groupsAsStudent={ groupsAsStudent }
				/>
				< NavigationContent items={ unitItems } toggleNavigation={ this.hideNavigationMenu }/>
				{ nextUnit && <NextUnit unit={ nextUnit } toggleNavigation={ this.handleToggleNavigation }
				/> }
			</div>
		)
	}

	handleToggleNavigation = () => {
		this.unitHeaderRef.scrollIntoView();
		this.hideNavigationMenu();
	};

	renderCourseNavigation() {
		const { courseTitle, description, courseItems, containsFlashcards, courseId, slideId, toggleNavigation, groupsAsStudent, courseProgress } = this.props;
		const { sideMenuStyle } = this.state;

		return (
			<div className={ styles.contentWrapper } style={ sideMenuStyle }>
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
