import React, { Component } from "react";
import PropTypes from "prop-types";

import Navigation from "../Navigation";
import AnyPage from '../../../pages/AnyPage';
import { UrlError } from "../../common/Error/NotFoundErrorBoundary";
import UnitFlashcardsPage from '../../../pages/course/UnitFlashcardsPage';
import CourseFlashcardsPage from '../../../pages/course/CourseFlashcardsPage';
import { flashcards, constructPathToSlide } from '../../../consts/routes';
import { changeCurrentCourseAction } from "../../../actions/course";
import { SLIDETYPE } from '../../../consts/general';
import { SCORING_GROUP_IDS } from '../../../consts/scoringGroup';

import queryString from 'query-string';
import classnames from 'classnames';

import styles from "./Course.less"

class Course extends Component {
	constructor(props) {
		super(props);
		const queryProps = queryString.parse(props.location.search);
		const pathname = window.location.pathname.toLowerCase();
		const isLti = pathname.endsWith('/ltislide') || pathname.endsWith('/acceptedalert'); //TODO remove this flag,that hiding nav menu
		const isReview = queryProps.CheckQueueItemId !== undefined;
		const isNavMenuVisible = !isLti && !isReview;

		this.state = {
			onCourseNavigation: true,
			openUnit: null,
			highlightedUnit: null,
			currentSlideId: null,
			currentCourseId: null,
			navigationOpened: this.props.navigationOpened,
			isNavMenuVisible,
			queryProps,
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

	componentDidUpdate(prevProps) {
		const { loadUserProgress, isAuthenticated, courseId, } = this.props;

		if (isAuthenticated !== prevProps.isAuthenticated) {
			loadUserProgress(courseId);
		}
	}

	static getDerivedStateFromProps(props, state) {
		const defaultState = { navigationOpened: props.navigationOpened };

		if (!props.units) {
			return defaultState;
		}

		if (state.currentCourseId !== props.courseId || state.currentSlideId !== props.slideId) {
			const openUnitId = Course.findUnitIdBySlideId(props.slideId, props.courseInfo);
			const openUnit = props.units[openUnitId];
			window.scrollTo(0, 0);

			const slide = Course.findSlideBySlideId(props.slideId, props.courseInfo);
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
		const { navigationOpened, isNavMenuVisible } = this.state;

		if (!courseInfo) {
			return null;
		}

		const Page = this.getOpenedPage();

		const mainClassName = classnames(styles.pageWrapper, { [styles.withoutNavigation]: !isNavMenuVisible }); // TODO remove it

		return (
			<div className={styles.rootWrapper}>
				<div className={ classnames(styles.root, { 'open': navigationOpened }) }>
					{ isNavMenuVisible && this.renderNavigation() }
					<main className={ mainClassName }>
						<Page match={ this.props.match }/>
					</main>
				</div>
			</div>
		);
	}

	renderNavigation() {
		const { courseInfo, units, progress } = this.props;
		const { onCourseNavigation, navigationOpened } = this.state;
		const { byUnits, courseProgress } = this.getCourseStatistics(units, progress, courseInfo.scoring.groups);

		const defaultProps = {
			navigationOpened,
			courseTitle: courseInfo.title,
			courseProgress,
		};

		const additionalProps = onCourseNavigation
			? this.createCourseSettings(byUnits)
			: this.createUnitSettings(byUnits);

		return <Navigation { ...defaultProps } { ...additionalProps }/>;
	}

	getCourseStatistics(units, progress, scoringGroups) {
		const courseStatistics = {
			courseProgress: { current: 0, max: 0 },
			byUnits: {},
		};

		if (!progress || scoringGroups.length === 0)
			return courseStatistics;

		const visitsGroup = scoringGroups.find(gr => gr.id === SCORING_GROUP_IDS.visits);

		for (const unit of Object.values(units)) {
			let unitScore = 0, unitMaxScore = 0;

			for (const { maxScore, id, scoringGroup } of unit.slides) {
				const group = scoringGroups.find(gr => gr.id === scoringGroup);

				if (visitsGroup) {
					unitMaxScore += visitsGroup.weight;
					if (progress[id] && progress[id].visited) {
						unitScore += visitsGroup.weight;
					}
				}

				if (group) {
					unitMaxScore += maxScore * group.weight;
					if (progress[id] && progress[id].score) {
						unitScore += progress[id].score * group.weight;
					}
				}
			}

			courseStatistics.courseProgress.current += unitScore;
			courseStatistics.courseProgress.max += unitMaxScore;
			courseStatistics.byUnits[unit.id] = { current: unitScore, max: unitMaxScore };
		}

		return courseStatistics;
	}

	createCourseSettings(scoresByUnits) {
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
				progress: scoresByUnits.hasOwnProperty(item.id) ? scoresByUnits[item.id] : { current: 0, max: 0 },
			})),
			containsFlashcards: courseInfo.containsFlashcards,
		};
	}

	createUnitSettings(scoresByUnits) {
		const { courseInfo, slideId, courseId, progress } = this.props;
		const { openUnit, } = this.state;

		return {
			unitTitle: openUnit.title,
			unitProgress: scoresByUnits.hasOwnProperty(openUnit.id) ? scoresByUnits[openUnit.id] : { current: 0, max: 0 },
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
			isActive: item.id === slideId,
			score: (progress && progress[item.id] && progress[item.id].score) || 0,
			maxScore: item.maxScore,
			questionsCount: item.questionsCount,
			visited: Boolean(progress && progress[item.id]),
		}));
	}

	getOpenedPage() {
		let { slideId, courseInfo } = this.props;
		const { queryProps } = this.state;

		if (slideId === flashcards) {
			return CourseFlashcardsPage;
		}

		if (!courseInfo || !courseInfo.units) {
			return AnyPage;
		}

		if (queryProps.slideId)
			slideId = queryProps.slideId;
		const currentSlide = Course.findSlideBySlideId(slideId, courseInfo);

		if (currentSlide === null) {
			throw new UrlError();
		}

		if (currentSlide && currentSlide.type === SLIDETYPE.flashcards) {
			return UnitFlashcardsPage;
		}

		return AnyPage;
	}

	static findUnitIdBySlideId(slideId, courseInfo) {
		if (!courseInfo || !courseInfo.units) {
			return null;
		}

		const units = courseInfo.units;

		for (const unit of units) {
			for (const slide of unit.slides) {
				if (slideId === slide.id) {
					return unit.id;
				}
			}
		}

		return null;
	}

	static findSlideBySlideId(slideId, courseInfo) {
		if (!courseInfo || !courseInfo.units) {
			return null;
		}

		const units = courseInfo.units;

		for (const unit of units) {
			for (const slide of unit.slides) {
				if (slideId === slide.id) {
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