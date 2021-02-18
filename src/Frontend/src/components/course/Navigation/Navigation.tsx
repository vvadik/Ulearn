import React, { Component, CSSProperties } from "react";
import { connect } from "react-redux";
import { Dispatch } from "redux";

import NavigationHeader from './Unit/NavigationHeader';
import NavigationContent from './Unit/NavigationContent';
import NextUnit from "./Unit/NextUnit";

import CourseNavigationHeader from "./Course/CourseNavigationHeader";
import CourseNavigationContent from "./Course/CourseNavigationContent";
import Flashcards from "./Course/Flashcards/Flashcards";

import { flashcards } from "src/consts/routes";

import { isMobile, isTablet } from "src/utils/getDeviceType";
import { toggleNavigationAction } from "src/actions/navigation";

import { CourseMenuItem, MenuItem, Progress } from "./types";

import { GroupAsStudentInfo } from "src/models/groups";
import { RootState } from "src/redux/reducers";
import { UnitInfo } from "src/models/course";
import { SlideType } from "src/models/slide";

import styles from './Navigation.less';

const mobileNavigationMenuWidth = 250;//250 is @mobileNavigationMenuWidth, its mobile nav menu width

export type Props = PropsFromRedux & DefaultNavigationProps & (CourseNavigationProps | UnitNavigationProps);

interface DefaultNavigationProps {
	courseTitle: string;
	navigationOpened: boolean;
}

interface PropsFromRedux {
	groupsAsStudent: GroupAsStudentInfo[];
	toggleNavigation: () => void;
}

export interface CourseNavigationProps {
	courseId: string;
	slideId?: string;
	description: string;

	containsFlashcards: boolean;

	courseProgress: Progress;
	courseItems: CourseMenuItem[];
}

export interface UnitNavigationProps {
	unitTitle: string;

	unitProgress: Progress;

	unitItems: MenuItem<SlideType>[];
	nextUnit: UnitInfo | null;

	onCourseClick: () => void;
}

interface State {
	overlayStyle?: CSSProperties;
	sideMenuStyle?: CSSProperties;
	xDown: null | number;
	yDown: null | number;
	touchListenerAdded: boolean,
}

class Navigation extends Component<Props, State> {
	touchDistanceTolerance = 10;
	touchStartMaxX = 24;//is simmilar to left padding on slide
	unitHeaderRef: React.RefObject<HTMLElement> = React.createRef();
	animationDuration = 300;

	constructor(props: Props) {
		super(props);

		this.state = {
			xDown: null,
			yDown: null,
			touchListenerAdded: false,
		};
	}

	componentDidMount() {
		document.addEventListener('resize', this.handleWindowSizeChange);
		this.tryAddTouchListener();
	}

	isMobileNavigationMenu = () => {
		return isMobile() || isTablet();
	};

	tryAddTouchListener = () => {
		if(this.isMobileNavigationMenu() && !this.state.touchListenerAdded) {
			document.addEventListener('touchstart', this.handleTouchStart);
			this.setState({
				touchListenerAdded: true,
			});
		}
	};

	getTouches = (evt: TouchEvent) => {
		return evt.touches || evt.changedTouches;
	};

	handleTouchStart = (evt: TouchEvent) => {
		document.addEventListener('touchmove', this.handleTouchMove);
		document.addEventListener('touchend', this.handleTouchEnd);

		const { clientX, clientY, } = this.getTouches(evt)[0];
		const { navigationOpened } = this.props;

		if(!navigationOpened && clientX <= this.touchStartMaxX || navigationOpened) {
			this.setState({
				xDown: clientX,
				yDown: clientY,
			});
		}
	};

	handleTouchEnd = (evt: TouchEvent) => {
		const { clientX, } = evt.changedTouches[0];
		const { toggleNavigation, navigationOpened, } = this.props;
		const { xDown, yDown, overlayStyle, } = this.state;

		if(!xDown || !yDown) {
			return;
		}

		const moveDistance = Math.abs(xDown - clientX);
		const isDistanceEnough = moveDistance !== 0;
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
			sideMenuStyle: undefined,
		});

		document.removeEventListener('touchmove', this.handleTouchMove);
		document.removeEventListener('touchend', this.handleTouchEnd);
	};

	handleTouchMove = (evt: TouchEvent) => {
		const { xDown, yDown, } = this.state;
		const { navigationOpened } = this.props;

		if(!xDown || !yDown) {
			return;
		}

		const { clientX, clientY, } = evt.touches[0];

		const xDiff = xDown - clientX;
		const yDiff = yDown - clientY;

		if(Math.abs(xDiff) > this.touchDistanceTolerance && Math.abs(xDiff) > Math.abs(yDiff)) {
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
			});
		} else {
			this.setState({
				xDown: null,
				yDown: null,
				overlayStyle: undefined,
				sideMenuStyle: undefined,
			});
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

	componentDidUpdate(prevProps: Props) {
		const { navigationOpened } = this.props;

		if((isMobile() || isTablet()) && prevProps.navigationOpened !== navigationOpened) {
			document.querySelector('body')?.classList.toggle(styles.overflow, navigationOpened);
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
				overlayStyle: undefined,
			});
		}, this.animationDuration);
	};

	hideNavigationMenu = () => {
		const { navigationOpened, toggleNavigation, } = this.props;

		if(navigationOpened) {
			toggleNavigation();
		}
	};

	render() {
		const { overlayStyle } = this.state;

		const courseProps = this.props as CourseNavigationProps;

		return (
			<aside>
				<div className={ styles.overlay } style={ overlayStyle } onClick={ this.hideNavigationMenu }/>
				{ courseProps.courseItems
					? this.renderCourseNavigation(this.props as CourseNavigationProps)
					: this.renderUnitNavigation(this.props as UnitNavigationProps)
				}
			</aside>
		);
	}

	renderUnitNavigation({
		unitTitle,
		onCourseClick,
		unitItems,
		nextUnit,
		unitProgress
	}: UnitNavigationProps) {
		const {
			courseTitle,
			groupsAsStudent,
			toggleNavigation,
		} = this.props;
		const { sideMenuStyle } = this.state;

		return (
			<div className={ styles.contentWrapper } style={ sideMenuStyle }>
				<NavigationHeader
					title={ unitTitle }
					courseName={ courseTitle }
					progress={ unitProgress }
					groupsAsStudent={ groupsAsStudent }
					createRef={ this.unitHeaderRef }
					onClick={ onCourseClick }
				/>
				<NavigationContent items={ unitItems } onClick={ toggleNavigation }/>
				{ nextUnit && <NextUnit unit={ nextUnit } onClick={ this.handleToggleNavigation }/> }
			</div>
		);
	}

	handleToggleNavigation = () => {
		this.unitHeaderRef.current?.scrollIntoView();
		this.hideNavigationMenu();
	};

	renderCourseNavigation({
		description,
		courseItems,
		containsFlashcards,
		courseId,
		slideId,
		courseProgress
	}: CourseNavigationProps) {
		const {
			courseTitle,
			toggleNavigation,
			groupsAsStudent,
		} = this.props;
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
		);
	}
}

const mapStateToProps = (state: RootState) => {
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

const mapDispatchToProps = (dispatch: Dispatch) => ({
	toggleNavigation: () => dispatch(toggleNavigationAction()),
});

const connected = connect(mapStateToProps, mapDispatchToProps)(Navigation);
export default connected;
