import React, { Component } from "react";
import PropTypes from "prop-types";

import api from "src/api";

import Navigation from "../Navigation";
import AnyPage from 'src/pages/AnyPage';
import UnitFlashcardsPage from 'src/pages/course/UnitFlashcardsPage';
import CourseFlashcardsPage from 'src/pages/course/CourseFlashcardsPage';
import { BlocksWrapper } from "src/components/course/Course/Slide/Blocks";
import CommentsView from "src/components/comments/CommentsView/CommentsView";
import Slide from './Slide/Slide';

import { UrlError } from "src/components/common/Error/NotFoundErrorBoundary";
import Error404 from "src/components/common/Error/Error404";
import { Link } from "react-router-dom";
import { Helmet } from "react-helmet";
import { Edit, } from "icons";
import { Toggle } from "ui";

import { flashcards, constructPathToSlide, signalrWS, } from 'src/consts/routes';
import { ROLES, SLIDETYPE, } from 'src/consts/general';
import { SCORING_GROUP_IDS } from 'src/consts/scoringGroup';

import classnames from 'classnames';

import styles from "./Course.less"

const slideNavigationButtonTitles = {
	next: "Далее",
	previous: "Назад",
	nextModule: "Следующий модуль",
	previousModule: "Предыдущий модуль",
}

class Course extends Component {
	constructor(props) {
		super(props);

		this.signalRConnection = null;

		this.state = {
			onCourseNavigation: true,
			openUnit: null,
			highlightedUnit: null,
			currentSlideId: null,
			currentSlideInfo: null,
			currentCourseId: null,
			navigationOpened: this.props.navigationOpened,
			showForStudents: false,
			meta: {
				title: 'Ulearn',
				description: 'Интерактивные учебные онлайн-курсы по программированию',
				keywords: '',
				imageUrl: '',
			},
		};
	}

	componentDidMount() {
		const { loadCourse, loadUserProgress, loadCourseErrors, courseId, courseInfo, progress, user, } = this.props;
		const { title } = this.state;
		const { isAuthenticated } = user;

		this.startSignalRConnection();

		if(!courseInfo) {
			loadCourse(courseId);
		} else {
			this.updateWindowMeta(title, courseInfo.title);
			if(courseInfo.isTempCourse)
				loadCourseErrors(courseId);
		}

		if(isAuthenticated && !progress) {
			loadUserProgress(courseId, user.id);
		}

		if(isAuthenticated) {
			window.reloadUserProgress = () => loadUserProgress(courseId, user.id); //adding hack to let legacy page scripts to reload progress,TODO(rozentor) remove it after implementing react task slides
		}

		/* TODO: (rozentor) for now it copied from downloadedHtmlContetn, which run documentReadyFunctions scripts. In future, we will have no scripts in back, so it can be removed totally ( in other words, remove it when DownloadedHtmlContent will be removed)  */
		(window.documentReadyFunctions || []).forEach(f => f());
	}

	startSignalRConnection = () => {
		const connection = api.createSignalRConnection(signalrWS);
		connection.on("courseChanged", this.onCourseChangedEvent);
		connection.start();

		this.signalRConnection = connection;
	}

	componentWillUnmount() {
		const { signalRConnection } = this;

		if(signalRConnection) {
			signalRConnection.stop();
		}
	}

	onCourseChangedEvent = (eventData) => {
		const { loadCourse, loadedCourseIds, } = this.props;
		const { courseId } = JSON.parse(eventData);

		if(loadedCourseIds[courseId]) {
			loadCourse(courseId);
		}
	}

	componentDidUpdate(prevProps, prevState) {
		const { loadUserProgress, courseId, loadCourse, user, courseInfo, loadCourseErrors, progress, isHijacked, updateVisitedSlide, } = this.props;
		const { title, currentSlideInfo, showForStudents, currentSlideId, } = this.state;
		const { isAuthenticated } = user;

		if(isAuthenticated !== prevProps.user.isAuthenticated) {
			loadCourse(courseId);
			loadUserProgress(courseId, user.id);
			window.reloadUserProgress = () => loadUserProgress(courseId, user.id); //adding hack to let legacy page scripts to reload progress,TODO(rozentor) remove it after implementing react task slides
		}

		if(title !== prevState.title) {
			this.updateWindowMeta(title, courseInfo.title);
			if(courseInfo.isTempCourse)
				loadCourseErrors(courseId);
		}

		if(!prevProps.progress && progress && !isHijacked && currentSlideInfo) {
			updateVisitedSlide(courseId, currentSlideInfo.current.id);
		}

		if((currentSlideId !== prevState.currentSlideId || showForStudents !== prevState.showForStudents) && currentSlideInfo.current.type === SLIDETYPE.exercise) {
			if(showForStudents) {
				this.props.history.push('?version=-1'); //prevent showing task solution
			} else if(this.props.history.location.search === '?version=-1') {
				this.props.history.replace();
			}
		}
	}

	static getDerivedStateFromProps(props, state) {
		const defaultState = { navigationOpened: props.navigationOpened };
		const { courseId, slideId, units, enterToCourse, courseInfo, progress, isHijacked, updateVisitedSlide } = props;

		if(!units) {
			return defaultState;
		}

		if(state.currentCourseId !== courseId || state.currentSlideId !== slideId) {
			enterToCourse(courseId);
			const openUnitId = Course.findUnitIdBySlideId(slideId, courseInfo);
			const openUnit = units[openUnitId];
			window.scrollTo(0, 0);

			const slideInfo = Course.getSlideInfoById(props.slideId, props.courseInfo);
			const Page = Course.getOpenedPage(props.slideId, props.courseInfo, slideInfo);
			const title = Course.getTitle(slideInfo, Page);
			if(slideInfo && progress && !isHijacked) {
				updateVisitedSlide(courseId, slideInfo.current.id);
			}

			return {
				Page,
				title,
				currentSlideId: slideId,
				currentSlideInfo: slideInfo,
				currentCourseId: courseId,
				highlightedUnit: openUnitId || null,
				onCourseNavigation: openUnit ? false : state.onCourseNavigation,
				openUnit: openUnit || state.openUnit,
				...defaultState,
			}
		}

		return defaultState;
	}

	static getOpenedPage = (slideId, courseInfo, currentSlideInfo) => {
		if(slideId === flashcards) {
			return CourseFlashcardsPage;
		}

		if(!courseInfo || !courseInfo.units) {
			return AnyPage;
		}

		if(currentSlideInfo === null) {
			throw new UrlError();
		}

		if(currentSlideInfo && currentSlideInfo.current.type === SLIDETYPE.flashcards) {
			return UnitFlashcardsPage;
		}

		if(currentSlideInfo && currentSlideInfo.current.type === SLIDETYPE.lesson) {
			return Slide;
		}

		return AnyPage;
	}

	static getTitle = (slideInfo) => {
		return slideInfo ? slideInfo.current.title : "Вопросы для самопроверки";
	}

	updateWindowMeta = (slideTitle, courseTitle) => {
		if(slideTitle) {
			this.setState({
				meta: {
					title: `${ courseTitle }: ${ slideTitle } на ulearn.me`,
					description: 'Интерактивные учебные онлайн-курсы по программированию',
					keywords: '',
					imageUrl: '',
				}
			});
		}
	}

	render() {
		const { courseInfo, courseLoadingErrorStatus, pageInfo: { isNavigationVisible, }, } = this.props;
		const { navigationOpened, meta, } = this.state;

		if(courseLoadingErrorStatus) {
			return <Error404/>;
		}

		if(!courseInfo) {
			return null;
		}

		return (
			<div
				className={ classnames(styles.root, { 'open': navigationOpened }, { [styles.withoutMinHeight]: !isNavigationVisible }) }>
				{ this.renderMeta(meta) }
				{ isNavigationVisible && this.renderNavigation() }
				{ courseInfo.tempCourseError ? <div
					className={ classnames(styles.errors) }>{ courseInfo.tempCourseError }</div> : this.renderSlide() }
			</div>
		);
	}

	renderMeta(meta) {
		return (
			<Helmet defer={ false }>
				<title>{ meta.title }</title>
			</Helmet>
		)
	}

	renderSlide() {
		const { pageInfo: { isNavigationVisible, isReview, }, progress, user, courseId, } = this.props;
		const { currentSlideInfo, currentSlideId, currentCourseId, Page, title, showForStudents, } = this.state;

		const wrapperClassName = classnames(
			styles.rootWrapper,
			{ [styles.withoutNavigation]: !isNavigationVisible }, // TODO remove isNavMenuVisible flag
			{ [styles.forStudents]: showForStudents },
		);

		const slideInfo = currentSlideInfo
			? currentSlideInfo.current
			: null;

		const score = slideInfo
			? {
				score: (progress && progress[slideInfo.id] && progress[slideInfo.id].score) || 0,
				maxScore: slideInfo.maxScore,
			}
			: null;

		const { isSystemAdministrator, accessesByCourse, roleByCourse } = user;
		const courseAccesses = accessesByCourse[courseId] ? accessesByCourse[courseId] : [];
		const courseRole = roleByCourse[courseId] ? roleByCourse[courseId] : '';
		const userRoles = { isSystemAdministrator, courseRole, courseAccesses, };

		return (
			<main className={ wrapperClassName }>
				{ this.isInstructor(userRoles) && this.renderShowForStudentToggle() }
				{ (isNavigationVisible || isReview) && title &&
				<h1 className={ styles.title }>
					{ title }
					{ slideInfo && slideInfo.gitEditLink && this.renderGitEditLink(slideInfo) }
				</h1> }
				<div className={ styles.slide }>
					{
						Page === Slide
							? <Slide
								slideId={ currentSlideId }
								courseId={ currentCourseId }
								showHiddenBlocks={ !showForStudents }
							/>
							: <BlocksWrapper score={ isNavigationVisible ? score : null }>
								<Page match={ this.props.match }/>
							</BlocksWrapper>
					}

				</div>
				{ currentSlideInfo && isNavigationVisible && this.renderNavigationButtons(currentSlideInfo) }
				{ currentSlideInfo && isNavigationVisible && this.renderComments(currentSlideInfo.current, userRoles) }
				{ isNavigationVisible && this.renderFooter() }
			</main>
		);
	}

	renderGitEditLink = (slideInfo) => {
		return (
			<a className={ styles.gitEditLink } rel='noopener noreferrer' target='_blank'
			   href={ slideInfo.gitEditLink }>
				<Edit/>
			</a>
		);
	}

	isCourseAdmin(userRoles) {
		return userRoles.isSystemAdministrator ||
			userRoles.courseRole === ROLES.courseAdmin;
	}

	isInstructor(userRoles) {
		return this.isCourseAdmin(userRoles) ||
			userRoles.courseRole === ROLES.instructor;
	}

	renderShowForStudentToggle = () => {
		return (
			<div className={ styles.toggleContainer }>
				<span className={ styles.toggleText }> Режим студента </span>
				<Toggle
					checked={ this.state.showForStudents }
					onValueChange={ this.showForStudentToggleChanged }
				/>
			</div>
		)
	}

	showForStudentToggleChanged = (value) => {
		this.setState({
			showForStudents: value,
		})
	}

	renderNavigationButtons(slideInfo) {
		const { courseId, } = this.props;
		const { next, current, } = slideInfo;
		const prevSlideHref = this.getPreviousSlideUrl(slideInfo);
		const nextSlideHref = next ? constructPathToSlide(courseId, next.slug) : null;

		const previousButtonText = current.firstInModule ? slideNavigationButtonTitles.previousModule : slideNavigationButtonTitles.previous;
		const nextButtonText = current.lastInModule ? slideNavigationButtonTitles.nextModule : slideNavigationButtonTitles.next;

		return (
			<div className={ styles.navigationButtonsWrapper }>
				{
					prevSlideHref
						? <Link className={ classnames(styles.slideButton, styles.previousSlideButton) }
								to={ this.constructPathWithAutoplay(prevSlideHref) }>
							{ previousButtonText }
						</Link>
						: <div className={ classnames(styles.slideButton, styles.disabledSlideButton) }>
							{ previousButtonText }
						</div>
				}
				{
					nextSlideHref
						?
						<Link className={ classnames(styles.slideButton, styles.nextSlideButton) }
							  to={ this.constructPathWithAutoplay(nextSlideHref) }>
							{ nextButtonText }
						</Link>
						: <div className={ classnames(styles.slideButton, styles.disabledSlideButton) }>
							{ nextButtonText }
						</div>
				}
			</div>
		);
	}

	getPreviousSlideUrl = (slideInfo) => {
		const { courseId, pageInfo: { isAcceptedSolutions, }, } = this.props;
		const { previous, current, } = slideInfo;

		if(isAcceptedSolutions) {
			return constructPathToSlide(courseId, current.slug);
		}

		return previous
			? constructPathToSlide(courseId, previous.slug)
			: null;
	}

	constructPathWithAutoplay = (baseHref) => {
		return baseHref + "?autoplay=true";
	}

	handleNavigationButtonClick = (href) => {
		this.props.history.push(href);
	};

	renderComments(currentSlide, userRoles,) {
		const { user, courseId, isSlideReady, } = this.props;

		return (
			<BlocksWrapper className={ styles.commentsWrapper }>
				<CommentsView user={ user }
							  slideType={ currentSlide.type }
							  slideId={ currentSlide.id }
							  userRoles={ userRoles }
							  courseId={ courseId }
							  isSlideReady={ isSlideReady }
				/>
			</BlocksWrapper>
		)
	}

	renderFooter() {
		return (
			<footer className={ styles.footer }>
				<p><Link to="/Home/Terms">Условия использования платформы</Link></p>
				<p>
					Вопросы и пожеланиями пишите на <a href="mailto:support@ulearn.me">support@ulearn.me</a>
				</p>
				<p>
					Сделано в СКБ Контур
				</p>
			</footer>
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

		if(!progress || scoringGroups.length === 0)
			return courseStatistics;

		const visitsGroup = scoringGroups.find(gr => gr.id === SCORING_GROUP_IDS.visits);

		for (const unit of Object.values(units)) {
			let unitScore = 0, unitMaxScore = 0;

			for (const { maxScore, id, scoringGroup } of unit.slides) {
				const group = scoringGroups.find(gr => gr.id === scoringGroup);

				if(visitsGroup) {
					unitMaxScore += visitsGroup.weight;
					if(progress[id] && progress[id].visited) {
						unitScore += visitsGroup.weight;
					}
				}

				if(group) {
					unitMaxScore += maxScore * group.weight;
					if(progress[id] && progress[id].score) {
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
				isNotPublished: item.isNotPublished,
				publicationDate: item.publicationDate,
			})),
			containsFlashcards: courseInfo.containsFlashcards,
		};
	}

	createUnitSettings(scoresByUnits) {
		const { courseInfo, slideId, courseId, progress } = this.props;
		const { openUnit, } = this.state;

		return {
			unitTitle: openUnit.title,
			unitProgress: scoresByUnits.hasOwnProperty(openUnit.id) ? scoresByUnits[openUnit.id] : {
				current: 0,
				max: 0
			},
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

	static findUnitIdBySlideId(slideId, courseInfo) {
		if(!courseInfo || !courseInfo.units) {
			return null;
		}

		const units = courseInfo.units;

		for (const unit of units) {
			for (const slide of unit.slides) {
				if(slideId === slide.id) {
					return unit.id;
				}
			}
		}

		return null;
	}

	static getSlideInfoById(slideId, courseInfo) {
		if(!courseInfo || !courseInfo.units) {
			return null;
		}

		const units = courseInfo.units;
		let prevSlide, nextSlide;

		for (let i = 0; i < units.length; i++) {
			const { slides } = units[i];
			for (let j = 0; j < slides.length; j++) {
				const slide = slides[j];

				if(slide.id === slideId) {
					if(j > 0) {
						prevSlide = slides[j - 1];
					} else if(i > 0) {
						const prevSlides = units[i - 1].slides;
						slide.firstInModule = true;
						prevSlide = prevSlides[prevSlides.length - 1];
					}

					if(j < slides.length - 1) {
						nextSlide = slides[j + 1];
					} else if(i < units.length - 1) {
						const nextSlides = units[i + 1].slides;
						slide.lastInModule = true;
						nextSlide = nextSlides[0];
					}

					return { previous: prevSlide, current: slide, next: nextSlide };
				}
			}
		}

		return null;
	}

	static findNextUnit(activeUnit, courseInfo) {
		const units = courseInfo.units;
		const activeUnitId = activeUnit.id;

		const indexOfActiveUnit = units.findIndex(item => item.id === activeUnitId);


		if(indexOfActiveUnit < 0 || indexOfActiveUnit === units.length - 1) {
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
	user: PropTypes.object,
	courseId: PropTypes.string,
	slideId: PropTypes.string,
	courseInfo: PropTypes.object, // TODO: описать
	progress: PropTypes.object, // TODO: описать
	units: PropTypes.object,
	enterToCourse: PropTypes.func,
	loadCourse: PropTypes.func,
	loadCourseErrors: PropTypes.func,
	loadUserProgress: PropTypes.func,
	updateVisitedSlide: PropTypes.func,
	navigationOpened: PropTypes.bool,
	courseLoadingErrorStatus: PropTypes.number,
	loadedCourseIds: PropTypes.object,
	pageInfo: PropTypes.shape({
		isLti: PropTypes.bool,
		isReview: PropTypes.bool,
		isAcceptedSolutions: PropTypes.bool,
		isNavigationVisible: PropTypes.bool,
	})
};

export default Course;