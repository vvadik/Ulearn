import React, { Component } from "react";
import PropTypes from "prop-types";

import Navigation from "../Navigation";
import AnyPage from '../../../pages/AnyPage';
import UnitFlashcardsPage from '../../../pages/course/UnitFlashcardsPage';
import CourseFlashcardsPage from '../../../pages/course/CourseFlashcardsPage';
import { flashcards, constructPathToSlide } from '../../../consts/routes';
import { changeCurrentCourseAction } from "../../../actions/course";
import { SLIDETYPE } from '../../../consts/general';

import classnames from 'classnames';

import styles from "./Course.less"

class Course extends Component {
	constructor(props) {
		super(props);

		this.state = {
			onCourseNavigation: true,
			openUnit: null,
			highlightedUnit: null,
			currentSlideId: null,
			currentCourseId: null,
			navigationOpened: this.props.navigationOpened,
		};
	}

	componentDidMount() {
		const { loadCourse, loadUserProgress, isAuthenticated, courseId, courseInfo, progress } = this.props;

		changeCurrentCourseAction(courseId);

		if (!courseInfo) {
			loadCourse(courseId);
		}

		if (isAuthenticated && !progress) {
			loadUserProgress(courseId);
		}
	}

	static getDerivedStateFromProps(props, state) {
		const defaultState = { navigationOpened: props.navigationOpened };

		if (!props.units) {
			return defaultState;
		}

		if (state.currentCourseId !== props.courseId || state.currentSlideId !== props.slideId) {
			const openUnitId = Course.findUnitIdBySlug(props.slideId, props.courseInfo);
			const openUnit = props.units[openUnitId];
			window.scrollTo(0, 0);

			const slide = Course.findSlideBySlug(props.slideId, props.courseInfo);
			if (slide && props.progress) {
				props.updateVisitedSlide(props.courseId, slide.id);
			}

			return {
				currentSlideId: props.slideId,
				currentCourseId: props.courseId,
				highlightedUnit: openUnitId || null,
				onCourseNavigation: openUnit ? false : state.onCourseNavigation,
				openUnit: openUnit || state.openUnit,
				...defaultState,
			}
		}

		return defaultState;
	}

	render() {
		const { courseInfo } = this.props;
		const { navigationOpened } = this.state;

		if (!courseInfo) {
			return null;
		}

		const Page = this.getOpenedPage();

		return (
			<div className={ classnames(styles.root, { 'open': navigationOpened }) }>
				{ this.renderNavigation() }
				<main className={ styles.pageWrapper }>
					<Page match={ this.props.match }/>
				</main>
			</div>
		);
	}

	renderNavigation() {
		const { courseInfo } = this.props;
		const { onCourseNavigation, navigationOpened } = this.state;

		const defaultProps = {
			navigationOpened,
			courseTitle: courseInfo.title,
		};

		const additionalProps = onCourseNavigation
			? this.createCourseSettings()
			: this.createUnitSettings();

		return <Navigation { ...defaultProps } { ...additionalProps }/>;
	}

	createCourseSettings() {
		const { courseInfo, slideId, } = this.props;
		const { highlightedUnit, } = this.state;

		return {
			slideId: slideId,
			courseId: courseInfo.id,
			description: courseInfo.description,
			courseItems: courseInfo.units.map(item => ({
				title: item.title,
				id: item.id,
				isActive: highlightedUnit === item.id,
				onClick: this.unitClickHandle,
			})),
			containsFlashcards: courseInfo.containsFlashcards,
		};
	}

	createUnitSettings() {
		const { courseInfo, slideId, courseId, progress } = this.props;
		const { openUnit, } = this.state;

		return {
			unitTitle: openUnit.title,
			onCourseClick: this.returnInUnitsMenu,
			unitItems: Course.mapUnitItems(openUnit.slides, progress, courseId, slideId,),
			nextUnit: Course.findNextUnit(openUnit, courseInfo),
		};
	}

	static mapUnitItems(unitSlides, progress, courseId, slideId) {
		return unitSlides.map(item => ({
			id: item.id,
			title: item.title,
			type: item.type,
			url: constructPathToSlide(courseId, item.slug),
			isActive: item.slug === slideId,
			score: (progress && progress[item.id] && progress[item.id].score) || 0,
			maxScore: item.maxScore,
			questionsCount: item.questionsCount,
			visited: Boolean(progress && progress[item.id]),
		}));
	}

	getOpenedPage() {
		const { slideId, courseInfo } = this.props;

		if (slideId === flashcards) {
			return CourseFlashcardsPage;
		}

		if (!courseInfo || !courseInfo.units) {
			return AnyPage;
		}

		const currentSlide = Course.findSlideBySlug(slideId, courseInfo);

		if (currentSlide && currentSlide.type === SLIDETYPE.flashcards) {
			return UnitFlashcardsPage;
		}

		return AnyPage;
	}

	static findUnitIdBySlug(slideSlug, courseInfo) {
		if (!courseInfo || !courseInfo.units) {
			return null;
		}

		const units = courseInfo.units;

		for (const unit of units) {
			for (const slide of unit.slides) {
				if (slideSlug === slide.slug) {
					return unit.id;
				}
			}
		}

		return null;
	}

	static findSlideBySlug(slideSlug, courseInfo) {
		if (!courseInfo || !courseInfo.units) {
			return null;
		}

		const units = courseInfo.units;

		for (const unit of units) {
			for (const slide of unit.slides) {
				if (slideSlug === slide.slug) {
					return slide;
				}
			}
		}

		return null;
	}

	static findNextUnit(activeUnit, courseInfo) {
		const units = courseInfo.units;
		const activeUnitId = activeUnit.id;

		const indexOfActiveUnit = units.findIndex(item => item.id === activeUnitId);


		if (indexOfActiveUnit < 0 || indexOfActiveUnit === units.length - 1) {
			return null;
		}


		return units[indexOfActiveUnit + 1];
	}

	unitClickHandle = (id) => {
		const { units } = this.props;

		this.setState({
			openUnit: units[id],
			onCourseNavigation: false,
		});
	};

	returnInUnitsMenu = () => {
		this.setState({
			openUnit: null,
			onCourseNavigation: true,
		});
	};
}

Course
	.propTypes = {
	isAuthenticated: PropTypes.bool,
	courseId: PropTypes.string,
	slideId: PropTypes.string,
	courseInfo: PropTypes.object, // TODO: описать
	progress: PropTypes.object, // TODO: описать
	units: PropTypes.object,
	loadCourse: PropTypes.func,
	loadUserProgress: PropTypes.func,
	updateVisitedSlide: PropTypes.func,
	navigationOpened: PropTypes.bool,
};

export default Course;