import React, { Component } from "react";
import { HubConnection } from "@microsoft/signalr";

import api from "src/api";

import Navigation from "../Navigation";
import AnyPage from 'src/pages/AnyPage';
import UnitFlashcardsPage from 'src/pages/course/UnitFlashcardsPage.js';
import CourseFlashcardsPage from 'src/pages/course/CourseFlashcardsPage.js';
import PreviewUnitPageFromAllCourse from "src/components/flashcards/UnitPage/PreviewUnitPageFromAllCourse";
import SlideHeader from "./Slide/SlideHeader/SlideHeader";
import { BlocksWrapper } from "src/components/course/Course/Slide/Blocks";
import CommentsView from "src/components/comments/CommentsView/CommentsView.js";
import Slide from './Slide/Slide';

import { UrlError } from "src/components/common/Error/NotFoundErrorBoundary";
import Error404 from "src/components/common/Error/Error404";
import { Link, RouteComponentProps } from "react-router-dom";
import { Helmet } from "react-helmet";
import { Edit, } from "icons";
import CourseLoader from "./CourseLoader/CourseLoader.js";

import { constructPathToSlide, flashcards, flashcardsPreview, signalrWS, } from 'src/consts/routes';
import { ShortSlideInfo, SlideType, } from 'src/models/slide';
import { ScoringGroupsIds } from 'src/consts/scoringGroup';
import { CourseInfo, ScoringGroup, UnitInfo } from "src/models/course";
import { SlideUserProgress, } from "src/models/userProgress";
import { AccountState } from "src/redux/account";
import { CourseAccessType, CourseRoleType } from "src/consts/accessType";

import classnames from 'classnames';

import styles from "./Course.less";

const slideNavigationButtonTitles = {
	next: "Далее",
	previous: "Назад",
	nextModule: "Следующий модуль",
	previousModule: "Предыдущий модуль",
};

interface State {
	Page?: React.ReactNode;
	title?: string;
	onCourseNavigation: boolean;
	openUnit?: UnitInfo;
	highlightedUnit: string | null;
	currentSlideId?: string;
	currentSlideInfo: CourseSlideInfo | null;
	currentCourseId: string;
	meta?: Meta;
}

interface Meta {
	title: string;
	description: string;
	keywords: string;
	imageUrl: string;
}

interface Props extends RouteComponentProps {
	user: AccountState;
	courseId: string;
	slideId?: string;
	courseInfo: CourseInfo;
	progress: { [p: string]: SlideUserProgress };
	isSlideReady: boolean;
	units: UnitsInfo | null;
	enterToCourse: (courseId: string) => void;
	loadCourse: (courseId: string) => void;
	loadCourseErrors: (courseId: string) => void;
	loadUserProgress: (courseId: string, userId: string) => void;
	updateVisitedSlide: (courseId: string, slideId: string) => void;
	isStudentMode: boolean;
	navigationOpened: boolean;
	courseLoadingErrorStatus: string | null;
	loadedCourseIds: Record<string, unknown>;
	isHijacked: boolean;
	pageInfo: PageInfo;
}

interface PageInfo {
	isLti: boolean;
	isReview: boolean;
	isAcceptedSolutions: boolean;
	isNavigationVisible: boolean;
	isAcceptedAlert: boolean;
}

interface CourseSlideInfo {
	slideType: SlideType;
	previous?: ShortSlideInfo;
	current?: ShortSlideInfo & { firstInModule?: boolean; lastInModule?: boolean; };
	next?: ShortSlideInfo;
}

interface UnitsInfo {
	[p: string]: UnitInfo;
}

interface CourseStatistics {
	courseProgress: ScoreStatistic;
	byUnits: { [unitId: string]: ScoreStatistic };
}

interface ScoreStatistic {
	current: number;
	max: number;
}

const defaultMeta = {
	title: 'Ulearn',
	description: 'Интерактивные учебные онлайн-курсы по программированию',
	keywords: '',
	imageUrl: '',
};

class Course extends Component<Props, State> {
	private signalRConnection: HubConnection | null = null;

	constructor(props: Props) {
		super(props);

		this.state = {
			currentCourseId: "",
			currentSlideId: "",
			currentSlideInfo: null,
			highlightedUnit: null,
			onCourseNavigation: true,
		};
	}

	componentDidMount(): void {
		const { loadCourse, loadUserProgress, loadCourseErrors, courseId, courseInfo, progress, user, } = this.props;
		const { title } = this.state;
		const { isAuthenticated } = user;

		this.startSignalRConnection();

		if(!courseInfo) {
			loadCourse(courseId);
		} else {
			this.updateWindowMeta(title, courseInfo.title);
			if(courseInfo.isTempCourse) {
				loadCourseErrors(courseId);
			}
		}

		if(isAuthenticated && !progress && user.id) {
			loadUserProgress(courseId, user.id);
		}

		/* TODO: (rozentor) for now it copied from downloadedHtmlContetn, which run documentReadyFunctions scripts. In future, we will have no scripts in back, so it can be removed totally ( in other words, remove it when DownloadedHtmlContent will be removed)  */
		(window.documentReadyFunctions || []).forEach(f => f());
	}

	startSignalRConnection = (): void => {
		const connection = api.createSignalRConnection(signalrWS);
		connection.on("courseChanged", this.onCourseChangedEvent);
		connection.start();

		this.signalRConnection = connection;
	};

	componentWillUnmount(): void {
		const { signalRConnection } = this;

		if(signalRConnection) {
			signalRConnection.stop();
		}
	}

	onCourseChangedEvent = (eventData: string): void => {
		const { loadCourse, loadedCourseIds, } = this.props;
		const { courseId } = JSON.parse(eventData);

		if(loadedCourseIds[courseId]) {
			loadCourse(courseId);
		}
	};

	componentDidUpdate(prevProps: Props, prevState: State): void {
		const {
			loadUserProgress,
			courseId,
			loadCourse,
			user,
			courseInfo,
			loadCourseErrors,
			progress,
			isHijacked,
			updateVisitedSlide,
		} = this.props;
		const { title, currentSlideInfo, } = this.state;
		const { isAuthenticated } = user;

		if(isAuthenticated !== prevProps.user.isAuthenticated && user.id) {
			loadCourse(courseId);
			loadUserProgress(courseId, user.id);
		}

		if(title !== prevState.title) {
			this.updateWindowMeta(title, courseInfo.title);
			if(courseInfo.isTempCourse) {
				loadCourseErrors(courseId);
			}
		}

		if(!prevProps.progress && progress && !isHijacked && currentSlideInfo && currentSlideInfo.current && currentSlideInfo.current.id) {
			updateVisitedSlide(courseId, currentSlideInfo.current.id);
		}
	}

	static getDerivedStateFromProps(props: Props, state: State): State | null {
		const { courseId, slideId, units, enterToCourse, courseInfo, progress, isHijacked, updateVisitedSlide } = props;

		if(!units) {
			return null;
		}

		if(state.currentCourseId !== courseId || state.currentSlideId !== slideId) {
			enterToCourse(courseId);
			const openUnitId = Course.findUnitIdBySlideId(slideId, courseInfo);
			const openUnit = openUnitId ? units[openUnitId] : undefined;
			window.scrollTo(0, 0);

			const slideInfo = Course.getSlideInfoById(props.slideId, props.courseInfo);
			const Page = Course.getOpenedPage(props.slideId, props.courseInfo, slideInfo?.slideType, props.pageInfo);
			const title = Course.getTitle(slideInfo?.current?.title);
			if(slideInfo && slideInfo.current && slideInfo.current.id && progress && !isHijacked) {
				updateVisitedSlide(courseId, slideInfo.current.id);
			}

			return {
				meta: state.meta || defaultMeta,
				Page,
				title,
				currentSlideId: slideId,
				currentSlideInfo: slideInfo,
				currentCourseId: courseId,
				highlightedUnit: openUnitId || null,
				onCourseNavigation: openUnit ? false : state.onCourseNavigation,
				openUnit: openUnit || state.openUnit,
			};
		}

		return null;
	}

	static getOpenedPage = (
		slideId: string | undefined,
		courseInfo: CourseInfo | undefined,
		slideType: SlideType | undefined,
		pageInfo: PageInfo
	): React.ReactNode => {
		if(slideId === flashcardsPreview) {
			return PreviewUnitPageFromAllCourse;
		}

		if(slideId === flashcards) {
			return CourseFlashcardsPage;
		}

		if(!courseInfo || !courseInfo.units) {
			return AnyPage;
		}

		if(!slideType) {
			throw new UrlError();
		}

		if(slideType === SlideType.Flashcards) {
			return UnitFlashcardsPage;
		}

		if(slideType &&
			(slideType === SlideType.Lesson
				|| (slideType === SlideType.Exercise && !pageInfo.isReview && !pageInfo.isAcceptedAlert))) {
			return Slide;
		}

		return AnyPage;
	};

	static getTitle = (currentSlideTitle: string | undefined): string => {
		return currentSlideTitle ? currentSlideTitle : "Вопросы для самопроверки";
	};

	updateWindowMeta = (slideTitle: string | undefined, courseTitle: string): void => {
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
	};

	render(): React.ReactElement {
		const {
			courseInfo,
			courseLoadingErrorStatus,
			pageInfo: { isNavigationVisible, },
			navigationOpened,
		} = this.props;
		const { meta, } = this.state;

		if(courseLoadingErrorStatus) {
			return <Error404/>;
		}

		if(!courseInfo) {
			return <CourseLoader/>;
		}

		return (
			<div
				className={ classnames(styles.root, { 'open': navigationOpened },
					{ [styles.withoutMinHeight]: !isNavigationVisible }) }>
				{ meta && this.renderMeta(meta) }
				{ isNavigationVisible && this.renderNavigation() }
				{ courseInfo.tempCourseError
					? <div className={ classnames(styles.errors) }>{ courseInfo.tempCourseError }</div>
					: this.renderSlide()
				}
			</div>
		);
	}

	renderMeta(meta: Meta): React.ReactElement {
		return (
			<Helmet defer={ false }>
				<title>{ meta.title }</title>
			</Helmet>
		);
	}

	renderSlide(): React.ReactElement {
		const { pageInfo: { isNavigationVisible, isReview, isLti, }, user, courseId, isStudentMode, } = this.props;
		const { currentSlideInfo, currentSlideId, currentCourseId, Page, title, openUnit, } = this.state;

		const wrapperClassName = classnames(
			styles.rootWrapper,
			{ [styles.withoutNavigation]: !isNavigationVisible }, // TODO remove isNavMenuVisible flag
			{ [styles.forStudents]: isNavigationVisible && isStudentMode },
		);

		const slideInfo = currentSlideInfo
			? currentSlideInfo.current
			: null;

		const { isSystemAdministrator, accessesByCourse, roleByCourse } = user;
		const courseAccesses = accessesByCourse[courseId] ? accessesByCourse[courseId] : [];
		const courseRole = roleByCourse[courseId] ? roleByCourse[courseId] : CourseRoleType.student;
		const userRoles = { isSystemAdministrator, courseRole, courseAccesses, };
		return (
			<main className={ wrapperClassName }>
				{ (isNavigationVisible || isReview) && title &&
				<h1 className={ styles.title }>
					{ title }
					{ slideInfo && slideInfo.gitEditLink && this.renderGitEditLink(slideInfo) }
				</h1> }
				<div className={ styles.slide }>
					{ isNavigationVisible && !isStudentMode && <SlideHeader
						courseId={ courseId }
						slideId={ currentSlideId || '' }
						isHiddenSlide={ slideInfo && slideInfo.hide || false }
						slideType={ slideInfo && slideInfo.type }
						userRoles={ userRoles }
						openUnitId={ openUnit?.id }
					/> }
					{
						Page === Slide
							?
							slideInfo && <Slide
								slideId={ currentSlideId }
								courseId={ currentCourseId }
								showHiddenBlocks={ !isStudentMode }
								slideInfo={ slideInfo }
								isLti={ isLti }
							/>
							: <BlocksWrapper>
								{/* eslint-disable-next-line @typescript-eslint/ban-ts-comment */ }
								{/* @ts-ignore*/ }
								<Page/>
							</BlocksWrapper>
					}
				</div>
				{ currentSlideInfo && isNavigationVisible && this.renderNavigationButtons(currentSlideInfo) }
				{ currentSlideInfo && currentSlideInfo.current && slideInfo && slideInfo.id && isNavigationVisible && this.renderComments(
					currentSlideInfo.current, userRoles) }
				{ isNavigationVisible && this.renderFooter() }
			</main>
		);
	}

	renderGitEditLink = (slideInfo: ShortSlideInfo): React.ReactElement => {
		return (
			<a className={ styles.gitEditLink } rel='noopener noreferrer' target='_blank'
			   href={ slideInfo.gitEditLink }>
				<Edit/>
			</a>
		);
	};

	renderNavigationButtons(slideInfo: CourseSlideInfo): React.ReactElement {
		const { courseId, } = this.props;
		const { next, current, } = slideInfo;
		const prevSlideHref = this.getPreviousSlideUrl(slideInfo);
		const nextSlideHref = next ? constructPathToSlide(courseId, next.slug) : null;

		const previousButtonText = current?.firstInModule ? slideNavigationButtonTitles.previousModule : slideNavigationButtonTitles.previous;
		const nextButtonText = current?.lastInModule ? slideNavigationButtonTitles.nextModule : slideNavigationButtonTitles.next;

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

	getPreviousSlideUrl = (slideInfo: CourseSlideInfo): string | null => {
		const { courseId, pageInfo: { isAcceptedSolutions, }, } = this.props;
		const { previous, current, } = slideInfo;

		if(isAcceptedSolutions && current) {
			return constructPathToSlide(courseId, current.slug);
		}

		return previous
			? constructPathToSlide(courseId, previous.slug)
			: null;
	};

	constructPathWithAutoplay = (baseHref: string): string => {
		return baseHref + "?autoplay=true";
	};

	handleNavigationButtonClick = (href: string): void => {
		const { history } = this.props;

		history.push(href);
	};

	renderComments(currentSlide: ShortSlideInfo,
		userRoles: { isSystemAdministrator: boolean, courseRole: CourseRoleType, courseAccesses: CourseAccessType[] },
	): React.ReactElement {
		const { user, courseId, isSlideReady, } = this.props;

		return (
			<BlocksWrapper className={ styles.commentsWrapper }>
				<CommentsView
					user={ user }
					slideType={ currentSlide.type }
					slideId={ currentSlide.id }
					userRoles={ userRoles }
					courseId={ courseId }
					isSlideReady={ isSlideReady }
				/>
			</BlocksWrapper>
		);
	}

	renderFooter(): React.ReactElement {
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

	renderNavigation(): React.ReactElement {
		const { courseInfo, units, progress, navigationOpened, } = this.props;
		const { onCourseNavigation, } = this.state;
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

	getCourseStatistics(
		units: UnitsInfo | null,
		progress: { [p: string]: SlideUserProgress },
		scoringGroups: ScoringGroup[]
	): CourseStatistics {
		const courseStatistics: CourseStatistics = {
			courseProgress: { current: 0, max: 0 },
			byUnits: {},
		};

		if(!progress || scoringGroups.length === 0) {
			return courseStatistics;
		}

		const visitsGroup = scoringGroups.find(gr => gr.id === ScoringGroupsIds.visits);

		if(units) {
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
		}

		return courseStatistics;
	}

	createCourseSettings(scoresByUnits: { [p: string]: ScoreStatistic }): Record<string, unknown> {
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
				progress: Object.prototype.hasOwnProperty
					.call(scoresByUnits, item.id) ? scoresByUnits[item.id] : { current: 0, max: 0 },
				isNotPublished: item.isNotPublished,
				publicationDate: item.publicationDate,
			})),
			containsFlashcards: courseInfo.containsFlashcards,
		};
	}

	createUnitSettings(scoresByUnits: { [p: string]: ScoreStatistic }): Record<string, unknown> {
		const { courseInfo, slideId, courseId, progress } = this.props;
		const { openUnit, } = this.state;
		if(!openUnit) {
			return {};
		}

		return {
			unitTitle: openUnit?.title,
			unitProgress: Object.prototype.hasOwnProperty
				.call(scoresByUnits, openUnit.id) ? scoresByUnits[openUnit?.id] : {
				current: 0,
				max: 0
			},
			onCourseClick: this.returnInUnitsMenu,
			unitItems: Course.mapUnitItems(openUnit.slides, progress, courseId, slideId,),
			nextUnit: Course.findNextUnit(openUnit, courseInfo),
		};
	}

	static mapUnitItems(
		unitSlides: ShortSlideInfo[],
		progress: { [p: string]: SlideUserProgress },
		courseId: string,
		slideId?: string
	): unknown[] {
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
			hide: item.hide,
		}));
	}

	static findUnitIdBySlideId(slideId?: string, courseInfo?: CourseInfo): string | null {
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

	static getSlideInfoById(
		slideId?: string,
		courseInfo?: CourseInfo
	): CourseSlideInfo | null {
		if(!courseInfo || !courseInfo.units) {
			return null;
		}
		const units = courseInfo.units;
		let prevSlide, nextSlide;

		for (let i = 0; i < units.length; i++) {
			const { slides } = units[i];
			for (let j = 0; j < slides.length; j++) {
				const slide = slides[j] as ShortSlideInfo & { firstInModule: boolean, lastInModule: boolean };

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

					return { slideType: slide.type, previous: prevSlide, current: slide, next: nextSlide };
				}
			}
		}
		const slideType = slideId === flashcardsPreview
			? SlideType.PreviewFlashcards
			: SlideType.CourseFlashcards;

		return { slideType };
	}

	static findNextUnit(activeUnit: UnitInfo, courseInfo: CourseInfo): UnitInfo | null {
		const units = courseInfo.units;
		const activeUnitId = activeUnit.id;

		const indexOfActiveUnit = units.findIndex(item => item.id === activeUnitId);

		if(indexOfActiveUnit < 0 || indexOfActiveUnit === units.length - 1) {
			return null;
		}

		return units[indexOfActiveUnit + 1];
	}

	unitClickHandle = (id: string): void => {
		const { units } = this.props;

		if(units) {
			this.setState({
				openUnit: units[id],
				onCourseNavigation: false,
			});
		}
	};

	returnInUnitsMenu = (): void => {
		this.setState({
			openUnit: undefined,
			onCourseNavigation: true,
		});
	};
}

export default Course;
