import React, { Component } from "react";
import PropTypes from "prop-types";
import UnitNavigation from "../Navigation/Unit/UnitNavigation";
import CourseNavigation from "../Navigation/Course/CourseNavigation";
import AnyPage from '../../../pages/AnyPage';
import UnitFlashcardsPage from '../../../pages/course/UnitFlashcardsPage';
import CourseFlashcardsPage from '../../../pages/course/CourseFlashcardsPage';
import { flashcards, constructPathToSlide } from '../../../consts/routes';
import { SLIDETYPE } from '../../../consts/general';
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
		};
	}

	componentDidMount() {
		const { loadCourse, loadUserProgress, isAuthenticated, courseId, courseInfo, progress } = this.props;

		if (!courseInfo) {
			loadCourse(courseId);
		}

		if (isAuthenticated && !progress) {
			loadUserProgress(courseId);
		}
	}

	static getDerivedStateFromProps(props, state) {
		if (!props.units) {
			return null;
		}

		if (state.currentCourseId !== props.courseId || state.currentSlideId !== props.slideId) {
			const openUnitId = Course.findUnitIdBySlide(props.slideId, props.courseInfo);
			const openUnit = props.units[openUnitId];

			const slide = Course.findSlideBySlug(props.slideId, props.courseInfo);
			if (slide) {
				props.updateVisitedSlide(props.courseId, slide.id);
			}

			return {
				currentSlideId: props.slideId,
				currentCourseId: props.courseId,
				highlightedUnit: openUnitId || null,
				onCourseNavigation: openUnit ? false : state.onCourseNavigation,
				openUnit: openUnit || state.openUnit,
			}
		}

		return null;
	}

	render() {
		const { courseInfo } = this.props;
		const { onCourseNavigation } = this.state;

		if (!courseInfo) {
			return null;
		}

		const Page = this.findOpenedSlideType();

		return (
			<div className={ styles.root }>
				{ onCourseNavigation ? this.renderCourseNavigation() : this.renderUnitNavigation() }
				<main className={ styles.pageWrapper }>
					<Page match={ this.props.match }/>
				</main>
			</div>
		);
	}

	renderCourseNavigation() {
		const { courseInfo, slideId } = this.props;
		const { highlightedUnit } = this.state;

		return (
			<CourseNavigation
				slideId={ slideId }
				courseId={ courseInfo.id }
				title={ courseInfo.title }
				description={ courseInfo.description }
				items={ courseInfo.units.map(item => ({
					title: item.title,
					id: item.id,
					isActive: highlightedUnit === item.id,
					onClick: this.unitClickHandle,
				})) }
				containsFlashcards={courseInfo.containsFlashcards}
			/>
		);
	}

	renderUnitNavigation() {
		const { openUnit } = this.state;
		const { courseInfo, courseId, slideId, progress } = this.props;

		return (
			<UnitNavigation
				title={ openUnit.title }
				courseName={ courseInfo.title }
				onCourseClick={ this.returnInUnitsMenu }
				items={ openUnit.slides.map(item => ({
					id: item.id,
					title: item.title,
					type: item.type,
					url: constructPathToSlide(courseId, item.slug),
					isActive: item.slug === slideId,
					score: (progress && progress[item.id] && progress[item.id].score) || 0,
					maxScore: item.maxScore,
					questionsCount: item.questionsCount,
					visited: Boolean(progress && progress[item.id]),
				})) }
				nextUnit={ Course.findNextUnit(openUnit, courseInfo) }
			/>
		);
	}

	findOpenedSlideType() {
		const { slideId, courseInfo } = this.props;

		if (slideId === flashcards) {
			return CourseFlashcardsPage;
		}


		if (!courseInfo || !courseInfo.units) {
			return AnyPage;
		}

		let currentSlide;
		const units = courseInfo.units;

		for (const unit of units) {
			for (const slide of unit.slides) {
				if (slideId === slide.slug) {
					currentSlide = slide;
				}
			}
		}

		if (currentSlide && currentSlide.type === SLIDETYPE.flashcards) {
			return UnitFlashcardsPage;
		}

		return AnyPage;
	}

	static findUnitIdBySlide(slideId, courseInfo) {
		if (!courseInfo || !courseInfo.units) {
			return null;
		}

		const units = courseInfo.units;


		for (const unit of units) {
			for (const slide of unit.slides) {
				if (slideId === slide.slug) {
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

Course.propTypes = {
	isAuthenticated: PropTypes.bool,
	courseId: PropTypes.string,
	slideId: PropTypes.string,
	courseInfo: PropTypes.object, // TODO: описать
	progress: PropTypes.object, // TODO: описать
	units: PropTypes.object,
	loadCourse: PropTypes.func,
	loadUserProgress: PropTypes.func,
	updateVisitedSlide: PropTypes.func,
};

export default Course;