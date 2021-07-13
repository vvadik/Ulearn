import React from "react";

import { FLAT_THEME, Select, Tabs, ThemeContext, Toggle } from "ui";
import { Controlled, } from "react-codemirror2";

import { Review } from "../Blocks/Exercise/Review/Review";
import { BlocksWrapper, } from "../Blocks";
import ScoreControls from "./ScoreControls/ScoreControls";
import 'codemirror/addon/selection/mark-selection.js';

import {
	getTextMarkersByReviews,
	createTextMarker,
	getAllReviewsFromSubmission,
	getPreviousManualCheckingInfo,
	getSelectedReviewIdByCursor,
	loadLanguageStyles,
	buildRange,
	PreviousManualCheckingInfo, TextMarkersByReviewId,
} from "../Blocks/Exercise/ExerciseUtils";

import { InstructorReviewTabs } from "./InstructorReviewTabs";
import { Language } from "src/consts/languages";
import { ReviewInfo, SubmissionInfo } from "src/models/exercise";
import CodeMirror, { Editor, EditorConfiguration, MarkerRange, TextMarker, } from "codemirror";
import { clone } from "src/utils/jsonExtensions";
import { DiffInfo, getDataFromReviewToCompareChanges, getDiffInfo, getReviewAnchorTop } from "./utils";
import { InstructorReviewInfo, InstructorReviewInfoWithAnchor, Props, State } from "./InstructorReview.types";

import AddCommentForm from "./AddCommentForm/AddCommentForm";
import AntiplagiarismHeader from "./AntiplagiarismHeader/AntiplagiarismHeader";
import StickyWrapper from "./AntiplagiarismHeader/StickyWrapper";
import {
	AntiplagiarismStatusResponse,
} from "src/models/instructor";
import CourseLoader from "../../CourseLoader";

import texts from "./InstructorReview.texts";

import styles from './InstructorReview.less';


class InstructorReview extends React.Component<Props, State> {
	private shameComment = 'Ой! Наш робот нашёл решения других студентов, подозрительно похожие на ваше. ' +
		'Так может быть, если вы позаимствовали части программы, взяли их из открытых источников либо сами поделились своим кодом. ' +
		'Выполняйте задания самостоятельно.';

	removeWhiteSpaces = (text: string): string => {
		//do not replace spaces in text to avoid scenario with multi line code //
		// .replace(/\s+/g, ' ');
		return text.trim();
	};

	constructor(props: Props) {
		super(props);

		const { studentSubmissions, favouriteReviews, } = props;
		let currentSubmission: SubmissionInfo | undefined = undefined;
		let diffInfo: DiffInfo | undefined = undefined;
		let reviews: InstructorReviewInfo[] | undefined = [];
		let outdatedReviews: InstructorReviewInfo[] | undefined = [];
		const favReviewsByUser = favouriteReviews?.filter(r => r.isFavourite);
		const favReviews = favouriteReviews?.filter(r => !r.isFavourite);
		const favouriteReviewsSet = new Set(favReviews?.map(r => r.text));
		const favouriteByUserSet = new Set(favReviewsByUser?.map(r => r.text));

		if(studentSubmissions) {
			const submissionInfo = this.getSubmissionInfo(studentSubmissions, 0);
			currentSubmission = submissionInfo.submission;
			diffInfo = submissionInfo.diffInfo;

			const allReviews = this.getReviewsFromSubmission(currentSubmission, diffInfo, false,);
			reviews = allReviews.reviews;
			outdatedReviews = allReviews.outdatedReviews;
		}

		this.state = {
			selectedReviewId: -1,
			reviews,
			outdatedReviews,
			markers: {},
			currentTab: InstructorReviewTabs.Review,
			currentSubmission,
			editor: null,
			addCommentValue: '',
			showDiff: false,
			diffInfo: diffInfo,
			favouriteReviewsSet,
			favouriteByUserSet,
		};
	}

	componentDidMount(): void {
		const {
			student,
			studentGroups,
			studentSubmissions,
			getStudentInfo,
			getStudentSubmissions,
			getStudentGroups,
			studentId,
			favouriteReviews,
			getFavouriteReviews,
			slideContext: { courseId, slideId, },
		} = this.props;
		const {
			currentSubmission,
		} = this.state;

		if(!student) {
			getStudentInfo(studentId);
		}
		if(!studentSubmissions) {
			getStudentSubmissions(studentId, courseId, slideId);
		}
		if(!studentGroups) {
			getStudentGroups(courseId, studentId);
		}
		if(!favouriteReviews) {
			getFavouriteReviews(courseId, studentId);
		}

		if(currentSubmission) {
			this.addMarkers();
		}
	}

	componentDidUpdate = (prevProps: Readonly<Props>, prevState: Readonly<State>): void => {
		const { studentSubmissions, } = this.props;
		const { currentSubmission, reviews, diffInfo, showDiff, } = this.state;

		if(!currentSubmission && studentSubmissions && studentSubmissions.length > 0) {
			this.loadSubmission(studentSubmissions, 0);
			return;
		}

		if(currentSubmission && studentSubmissions) {
			if(prevState.showDiff !== showDiff) {
				const newReviews = this.getReviewsFromSubmission(
					currentSubmission,
					diffInfo,
					showDiff,
				);
				this.setState({
					reviews: newReviews.reviews,
					outdatedReviews: newReviews.outdatedReviews,
				}, this.resetMarkers);
				return;
			}

			const submissionIndex = studentSubmissions.findIndex(s => s.id === currentSubmission.id);
			const submission = clone(studentSubmissions[submissionIndex]);
			if(submission) {
				const newReviews = this.getReviewsFromSubmission(
					submission,
					diffInfo,
					showDiff,
				);
				const reviewsCompare = reviews.map(
					r => getDataFromReviewToCompareChanges(r));
				const newReviewsCompare = newReviews.reviews.map(
					r => getDataFromReviewToCompareChanges(r));

				// outdated should not be changed
				//	const newOutdatedReviewsCompare = newReviews.outdatedReviews.map(r => getDataFromReviewToCompareChanges(r));
				//  const outdatedReviewsCompare = outdatedReviews.map(r => getDataFromReviewToCompareChanges(r));
				//  if(JSON.stringify(outdatedReviewsCompare) !== JSON.stringify(newOutdatedReviewsCompare)) {}

				if(JSON.stringify(newReviewsCompare) !== JSON.stringify(reviewsCompare)) {
					this.setState({
						currentSubmission: submission,
						reviews: newReviews.reviews,
						outdatedReviews: newReviews.outdatedReviews,
					}, this.resetMarkers);
				}
			}
		}
	};

	loadSubmission = (studentSubmissions: SubmissionInfo[], index: number,): void => {
		const { showDiff, } = this.state;
		const { submission, diffInfo } = this.getSubmissionInfo(studentSubmissions, index);
		const { reviews, outdatedReviews } = this.getReviewsFromSubmission(submission, diffInfo, showDiff);
		this.setState({
			currentSubmission: submission,
			diffInfo,
			selectedReviewId: -1,
			markers: {},
			addCommentFormCoords: undefined,
			reviews,
			outdatedReviews,
		}, this.resetMarkers);
	};

	getSubmissionInfo = (studentSubmissions: SubmissionInfo[],
		index: number,
	): { submission: SubmissionInfo; diffInfo: DiffInfo | undefined; } => {
		const submission = clone(studentSubmissions[index]);
		const prevSubmissionInfo = getPreviousManualCheckingInfo(studentSubmissions, index);
		const diffInfo = prevSubmissionInfo
			? getDiffInfo(submission, prevSubmissionInfo.submission)
			: undefined;

		return { submission, diffInfo, };
	};

	getReviewsFromSubmission = (
		submission: SubmissionInfo,
		diffInfo: DiffInfo | undefined,
		showDiff: boolean,
	): { reviews: ReviewInfo[]; outdatedReviews: ReviewInfo[]; } => {
		const reviews: ReviewInfo[] = getAllReviewsFromSubmission(submission)
			.map(r => ({
				...r,
				startLine: showDiff && diffInfo ? diffInfo.newCodeNewLineIndex[r.startLine] : r.startLine,
				finishLine: showDiff && diffInfo ? diffInfo.newCodeNewLineIndex[r.finishLine] : r.finishLine,
			}));
		const outdatedReviews: ReviewInfo[] = diffInfo
			? getAllReviewsFromSubmission(diffInfo.prevReviewedSubmission,)
				.map(r => ({
					...r,
					startLine: showDiff && diffInfo ? diffInfo.oldCodeNewLineIndex[r.startLine] : r.startLine,
					finishLine: showDiff && diffInfo ? diffInfo.oldCodeNewLineIndex[r.finishLine] : r.finishLine,
				}))
			: [];

		return { reviews, outdatedReviews, };
	};

	resetMarkers = (): void => {
		const {
			markers,
			editor,
			diffInfo,
			showDiff,
		} = this.state;

		if(!editor) {
			return;
		}

		Object.values(markers)
			?.forEach((markers: TextMarker[]) => markers
				.forEach((m: TextMarker) => m.clear()));

		this.addMarkers();
		if(showDiff && diffInfo) {
			this.addLineClasses(editor, diffInfo);
		}
	};

	addLineClasses = (editor: Editor, diffInfo: DiffInfo,): void => {
		//addding a line class via addLineClass triggers rerender on each method call, it cause perfomance issues
		//instead of it we can add class directly to line wrappers
		const linesWrapper = editor.getWrapperElement().getElementsByClassName('CodeMirror-code')[0];
		for (let i = 0; i < diffInfo.diffByBlocks.length; i++) {
			const { type, } = diffInfo.diffByBlocks[i];
			if(type) {
				const lineWrapper = linesWrapper.children[i];
				if(!lineWrapper) {
					return;
				}
				const lineNumberWrapper = document.createElement('div');

				lineWrapper.prepend(lineNumberWrapper);

				switch (type) {
					case "added": {
						lineWrapper.classList.add(styles.addedLinesCodeMirror);
						lineNumberWrapper.classList.add(styles.addedLinesGutter);
						break;
					}
					case "removed": {
						lineWrapper.classList.add(styles.removedLinesCodeMirror);
						lineNumberWrapper.classList.add(styles.removedLinesGutter);
						break;
					}
				}
			}
		}
	};

	static getDerivedStateFromProps(props: Readonly<Props>, state: Readonly<State>): Partial<State> | null {
		const { favouriteReviews, } = props;
		const { favouriteReviewsSet, favouriteByUserSet, } = state;
		const favReviewsByUser = favouriteReviews?.filter(r => r.isFavourite);
		const favReviews = favouriteReviews?.filter(r => !r.isFavourite);

		if(favReviewsByUser?.length !== favouriteByUserSet?.size || favReviews?.length !== favouriteReviewsSet?.size) {
			return {
				favouriteReviewsSet: new Set(favReviews?.map(r => r.text)),
				favouriteByUserSet: new Set(favReviewsByUser?.map(r => r.text)),
			};
		}

		return null;
	}

	render(): React.ReactNode {
		const {
			student,
			studentGroups,
			studentSubmissions,
			authorSolution,
			formulation,
			favouriteReviews,
		} = this.props;
		const {
			currentTab,
			currentSubmission,
			prevReviewScore,
			currentScore,
		} = this.state;

		if(!student || !studentSubmissions || !studentGroups || !favouriteReviews || !currentSubmission) {
			return <CourseLoader/>;
		}

		return (
			<>
				<BlocksWrapper withoutBottomPaddings>
					<h3 className={ styles.reviewHeader }>
						<span className={ styles.reviewStudentName }>
							{ texts.getStudentInfo(student.visibleName, studentGroups[0].name) }
						</span>
						{ texts.getReviewInfo(studentSubmissions, prevReviewScore, currentScore) }
					</h3>
					<Tabs value={ currentTab } onValueChange={ this.onTabChange }>
						<Tabs.Tab key={ InstructorReviewTabs.Review } id={ InstructorReviewTabs.Review }>
							{ texts.getTabName(InstructorReviewTabs.Review) }
						</Tabs.Tab>
						{
							formulation &&
							<Tabs.Tab key={ InstructorReviewTabs.Formulation } id={ InstructorReviewTabs.Formulation }>
								{ texts.getTabName(InstructorReviewTabs.Formulation) }
							</Tabs.Tab>
						}
						{
							authorSolution &&
							<Tabs.Tab key={ InstructorReviewTabs.AuthorSolution }
									  id={ InstructorReviewTabs.AuthorSolution }>
								{ texts.getTabName(InstructorReviewTabs.AuthorSolution) }
							</Tabs.Tab>
						}
					</Tabs>
				</BlocksWrapper>
				<div className={ styles.separator }/>
				{ this.renderCurrentTab() }
			</>
		);
	}

	onTabChange = (value: string): void => {
		this.setState({ currentTab: value as InstructorReviewTabs });
	};

	renderCurrentTab(): React.ReactNode {
		const { formulation, authorSolution, } = this.props;
		const { currentTab, } = this.state;


		switch (currentTab) {
			case InstructorReviewTabs.Review: {
				return this.renderSubmissions();
			}
			case InstructorReviewTabs.Formulation: {
				return formulation;
			}
			case InstructorReviewTabs.AuthorSolution: {
				return authorSolution;
			}
		}
	}

	renderSubmissions(): React.ReactNode {
		const {
			slideContext,
			onScoreSubmit,
			onProhibitFurtherReviewToggleChange,
			prohibitFurtherManualChecking,
			favouriteReviews,
		} = this.props;
		const {
			prevReviewScore,
			currentScore,
			currentSubmission,
		} = this.state;

		if(!favouriteReviews || !currentSubmission) {
			return null;
		}

		return (
			<BlocksWrapper>
				{ this.renderTopControls() }
				<StickyWrapper
					stickerClass={ styles.wrapperStickerStopper }
					sticker={ (fixed) =>
						<AntiplagiarismHeader
							fixed={ fixed }
							shouldCheck={ true }
							getAntiplagiarismStatus={ this.getAntiplagiarismStatus }
							onZeroScoreButtonPressed={ this.onZeroScoreButtonPressed }
						/>
					}
					content={ this.renderEditor() }
				/>
				<ScoreControls
					curReviewScore={ currentScore ?? currentScore }
					prevReviewScore={ prevReviewScore }
					exerciseTitle={ slideContext.title }
					onSubmit={ onScoreSubmit }
					onToggleChange={ onProhibitFurtherReviewToggleChange }
					toggleChecked={ prohibitFurtherManualChecking }
				/>
			</BlocksWrapper>
		);
	}

	getAntiplagiarismStatus = (): Promise<AntiplagiarismStatusResponse | string> => {
		const { currentSubmission, } = this.state;
		const { getAntiplagiarismStatus, } = this.props;

		return getAntiplagiarismStatus(currentSubmission!.id);
	};

	onZeroScoreButtonPressed = (): void => {
		const {
			onScoreSubmit,
			onProhibitFurtherReviewToggleChange,
			addReview,
		} = this.props;
		const {
			currentSubmission,
		} = this.state;

		if(!currentSubmission) {
			return;
		}

		onScoreSubmit(0);
		onProhibitFurtherReviewToggleChange(false);
		addReview(currentSubmission.id, this.shameComment, 0, 0, 0, 1);
	};

	renderTopControls(): React.ReactElement {
		const { showDiff, diffInfo, } = this.state;

		return (
			<div className={ styles.topControlsWrapper }>
				{ this.renderSubmissionsSelect() }
				{ diffInfo &&
				<Toggle onValueChange={ this.onDiffToggleValueChanged } checked={ showDiff }>
					{ texts.getDiffText(
						diffInfo.addedLinesCount,
						styles.diffAddedLinesTextColor,
						diffInfo.removedLinesCount,
						styles.diffRemovedLinesTextColor)
					}
				</Toggle> }
				<span className={ styles.leaveCommentGuideText }>{ texts.leaveCommentGuideText }</span>
			</div>

		);
	}

	renderEditor = (): React.ReactNode => {
		const {
			user,
			favouriteReviews,
			onToggleReviewFavourite,
			onAddReviewToFavourite,
		} = this.props;
		const {
			currentSubmission,
			selectedReviewId,
			diffInfo,
			showDiff,
			addCommentFormCoords,
			addCommentValue,
		} = this.state;

		if(!favouriteReviews || !currentSubmission) {
			return null;
		}

		return (
			<div className={ styles.positionWrapper }>
				<div className={ styles.wrapper }>
					<Controlled
						//controlled component refreshes more wisely, but it need onChange function, so we mocking it
						onBeforeChange={ this.mock }
						onSelection={ this.onSelectionChange }
						editorDidMount={ this.onEditorMount }
						className={ styles.editor }
						options={ this.getEditorOptions(currentSubmission.language) }
						value={ diffInfo && showDiff
							? diffInfo.code
							: currentSubmission.code
						}
						onCursorActivity={ this.onCursorActivity }
					/>
					<Review
						backgroundColor={ 'gray' }
						user={ user }
						addReviewComment={ this.onAddReviewComment }
						toggleReviewFavourite={ this.onToggleReviewFavouriteByReviewId }
						deleteReviewOrComment={ this.onDeleteReviewOrComment }
						editReviewOrComment={ this.editReviewOrComment }
						selectedReviewId={ selectedReviewId }
						onReviewClick={ this.selectComment }
						reviews={ this.getAllReviewsAsInstructorReviews() }
						isReviewCanBeAdded={ this.isReviewCanBeAdded }
					/>
				</div>
				{ addCommentFormCoords &&
				<AddCommentForm
					value={ addCommentValue }
					valueCanBeAddedToFavourite={ this.isCommentCanBeAddedToFavourite() }
					onValueChange={ this.onCommentFormValueChange }
					addComment={ this.onFormAddComment }
					comments={ favouriteReviews }
					addCommentToFavourite={ onAddReviewToFavourite }
					toggleCommentFavourite={ onToggleReviewFavourite }
					coordinates={ addCommentFormCoords }
					onClose={ this.onFormClose }
				/> }
			</div>
		);
	};

	getAllReviewsAsInstructorReviews = (): InstructorReviewInfoWithAnchor[] => {
		const {
			reviews,
			outdatedReviews,
			diffInfo,
			favouriteByUserSet,
			editor,
			showDiff,
		} = this.state;

		if(!editor) {
			return [];
		}

		const allReviews: InstructorReviewInfoWithAnchor[] = reviews.map(r => ({
			...r,
			instructor: {
				isFavourite: favouriteByUserSet.has(r.comment),
			},
			anchor: getReviewAnchorTop(r, editor,),
		}));

		if(showDiff && diffInfo) {
			return allReviews.concat(outdatedReviews.map(r => ({
				...r,
				instructor: {
					outdated: true,
				},
				anchor: getReviewAnchorTop(r, editor,),
			})));
		}

		return allReviews;
	};

	mock = (): void => {
		return;
	};

	onToggleReviewFavouriteByReviewId = (reviewId: number): void => {
		const {
			onToggleReviewFavourite,
			onAddReviewToFavourite,
			favouriteReviews,
		} = this.props;
		const {
			currentSubmission,
		} = this.state;

		if(currentSubmission && favouriteReviews) {
			const review = currentSubmission.manualCheckingReviews.find(r => r.id === reviewId);
			if(review) {
				const favouriteReview = favouriteReviews.find(r => r.text === review.comment);
				if(favouriteReview) {
					onToggleReviewFavourite(favouriteReview.id);
				} else {
					onAddReviewToFavourite(review.comment);
				}
			}
		}
	};

	isReviewCanBeAdded = (reviewText: string): boolean => {
		return reviewText !== undefined && this.removeWhiteSpaces(reviewText).length > 0;
	};

	isCommentCanBeAddedToFavourite = (): boolean => {
		const { addCommentValue, favouriteByUserSet, favouriteReviewsSet, } = this.state;
		const trimmed = this.removeWhiteSpaces(addCommentValue);

		return trimmed.length > 0 && !favouriteByUserSet?.has(trimmed) && !favouriteReviewsSet?.has(trimmed);
	};

	editReviewOrComment = (text: string, id: number, reviewId?: number,): void => {
		const {
			editReviewOrComment,
		} = this.props;
		const {
			currentSubmission,
		} = this.state;

		if(currentSubmission) {
			const trimmed = this.removeWhiteSpaces(text);

			editReviewOrComment(trimmed, currentSubmission.id, id, reviewId);
		}
	};

	onCommentFormValueChange = (comment: string): void => {
		this.setState({
			addCommentValue: comment,
		});
	};

	onDeleteReviewOrComment = (reviewId: number, commentId?: number): void => {
		const { deleteReviewOrComment, } = this.props;
		const { currentSubmission, } = this.state;

		if(currentSubmission) {
			deleteReviewOrComment(currentSubmission.id, reviewId, commentId);
		}
	};

	getEditorOptions = (language: Language): EditorConfiguration => ({
		lineNumbers: true,
		lineNumberFormatter: this.formatLine,
		lineWrapping: true,
		scrollbarStyle: 'null',
		theme: 'default',
		readOnly: true,
		matchBrackets: true,
		mode: loadLanguageStyles(language),
		styleSelectedText: styles.selectionMarker,
	});

	renderSubmissionsSelect = (): React.ReactNode => {
		const { currentSubmission } = this.state;
		const { studentSubmissions, } = this.props;

		if(!studentSubmissions || !currentSubmission) {
			return null;
		}

		const items = [...studentSubmissions.map(
			(submission, index,) => ([
				submission.id,
				texts.getSubmissionCaption(
					submission,
					index === 0,
					index === 0)
			]))
		];

		return (
			<div className={ styles.submissionsSelect }>
				<ThemeContext.Provider value={ FLAT_THEME }>
					<Select
						width={ '100%' }
						items={ items }
						value={ currentSubmission.id }
						onValueChange={ this.onSubmissionsSelectValueChange }
					/>
				</ThemeContext.Provider>
			</div>
		);
	};

	onDiffToggleValueChanged = (value: boolean): void => {
		const {
			currentSubmission,
			editor,
		} = this.state;

		if(!currentSubmission || !editor) {
			return;
		}

		this.setState({
			showDiff: value,
			selectedReviewId: -1,
			addCommentFormCoords: undefined,
		}, this.resetMarkers);
	};

	onSubmissionsSelectValueChange = (id: unknown): void => {
		const { studentSubmissions, } = this.props;
		const { editor, } = this.state;

		if(!studentSubmissions || !editor) {
			return;
		}
		const currentSubmissionIndex = studentSubmissions.findIndex(s => s.id === id);

		this.loadSubmission(studentSubmissions, currentSubmissionIndex);
	};

	addMarkers = (): void => {
		const {
			reviews,
			outdatedReviews,
			editor,
			diffInfo,
			showDiff,
			selectedReviewId,
		} = this.state;

		if(!editor) {
			return;
		}

		const doc = editor.getDoc();

		let markers = {
			...getTextMarkersByReviews(
				reviews,
				doc,
				styles.defaultMarker,
				showDiff && diffInfo ? diffInfo.deletedLinesSet : undefined,
			),
		};

		if(showDiff) {
			markers = {
				...markers,
				...getTextMarkersByReviews(
					outdatedReviews,
					doc,
					styles.defaultMarker,
				),
			};
		}

		this.setState({
			markers,
		}, () => selectedReviewId > -1 && this.highlightReview(selectedReviewId, editor,));
	};


	formatLine = (lineNumber: number): string => {
		const { diffInfo, showDiff, } = this.state;
		if(showDiff && diffInfo && diffInfo.diffByBlocks.length >= lineNumber) {
			const blockDiff = diffInfo.diffByBlocks[lineNumber - 1];

			return blockDiff.line.toString();
		}

		return lineNumber.toString();
	};

	onMouseUp = (): void => {
		const { editor, addCommentFormCoords, } = this.state;

		if(!editor || addCommentFormCoords) {
			return;
		}

		const doc = editor.getDoc();

		const selections = doc.listSelections();
		const firstSelection = selections[0];
		const startRange = this.getStartAndEndFromRange(firstSelection)[0];

		const lastSelection = selections[selections.length - 1];
		const endRange = this.getStartAndEndFromRange(lastSelection)[1];
		let coords: { left: number, right: number, top: number, bottom: number } | undefined
			= undefined;

		for (const selection of selections) {
			const range = selection;
			const selectedText = doc.getSelection();//ЕСЛИ УБРАТЬ ВЫБОР ТЕКСТА, НЕ ДОБАВИТЬ КОММЕНТ!

			if(selectedText.length > 0) {
				const [startRange, endRange,] = this.getStartAndEndFromRange(range);
				coords = editor.charCoords({ line: endRange.line + 1, ch: 0 }, 'local');

				createTextMarker(
					endRange.line,
					endRange.ch,
					startRange.line,
					startRange.ch,
					styles.selectionToReviewMarker,
					doc);
			}
		}

		this.setState({
			addCommentFormCoords: coords,
			addCommentRanges: { startRange, endRange, },
		});
		document.addEventListener('keydown', this.onEscPressed);
		document.removeEventListener('mouseup', this.onMouseUp);
	};

	onAddReviewComment = (reviewId: number, comment: string): void => {
		const { addReviewComment, } = this.props;
		const { currentSubmission, } = this.state;

		if(currentSubmission) {
			addReviewComment(currentSubmission.id, reviewId, this.removeWhiteSpaces(comment));
		}
	};

	onFormAddComment = (comment: string): void => {
		const {
			onAddReview,
			addReview,
		} = this.props;
		const {
			currentSubmission,
			editor,
			diffInfo,
			showDiff,
			addCommentRanges,
		} = this.state;

		this.setState({
			addCommentFormCoords: undefined,
			addCommentValue: '',
		});

		if(!editor || !currentSubmission || !addCommentRanges) {
			return;
		}
		const { startRange, endRange, } = addCommentRanges;

		comment = this.removeWhiteSpaces(comment);

		if(diffInfo && showDiff) {
			const actualStartLine = diffInfo.diffByBlocks[startRange.line].line - 1;
			const actualEndLine = diffInfo.diffByBlocks[endRange.line].line - 1;

			addReview(currentSubmission.id,
				comment,
				actualStartLine,
				startRange.ch,
				actualEndLine,
				endRange.ch,
			).then(r => this.highlightReview(r.id, editor,));
		} else {
			addReview(currentSubmission.id,
				comment,
				startRange.line,
				startRange.ch,
				endRange.line,
				endRange.ch
			).then(r => this.highlightReview(r.id, editor,));
		}
		onAddReview(comment);

		const doc = editor.getDoc();
		doc
			.getAllMarks()
			.forEach(m => m.className === styles.selectionToReviewMarker && m.clear());
		doc.setSelection({ ch: 0, line: 0, });
	};

	getStartAndEndFromRange = ({
		anchor,
		head,
	}: CodeMirror.Range): [start: CodeMirror.Position, end: CodeMirror.Position] => {
		if(anchor.line < head.line || (anchor.line === head.line && anchor.ch <= head.ch)) {
			return [anchor, head,];
		}

		return [head, anchor,];
	};

	onEscPressed = (event: KeyboardEvent): void => {
		if(event.key === 'Escape') {
			this.onFormClose();
		}
	};

	onFormClose = (): void => {
		const {
			editor,
		} = this.state;

		this.setState({
			addCommentFormCoords: undefined,
		});
		document.removeEventListener('keydown', this.onEscPressed);
		editor
			?.getDoc()
			.getAllMarks()
			.forEach(m => m.className === styles.selectionToReviewMarker && m.clear());
	};

	onSelectionChange = (
		e: Editor,
		data: {
			ranges: CodeMirror.Range[],
			update: (ranges: { anchor: CodeMirror.Position, head: CodeMirror.Position }[]) => void,
		},
	): void => {
		const {
			showDiff,
			diffInfo,
		} = this.state;

		if(!showDiff || !diffInfo || data.ranges.length === 0) {
			return;
		}

		const range = data.ranges[data.ranges.length - 1];
		const [start, end] = this.getStartAndEndFromRange(range);

		const selectedLines = buildRange(end.line - start.line + 1, start.line + 1);
		const finalSelectedLines = selectedLines
			.filter(l => !diffInfo.deletedLinesSet.has(l));

		if(finalSelectedLines.length === selectedLines.length) {
			return;
		}

		const newRanges = finalSelectedLines
			.reduce((pv: {
				anchor: { line: number, ch: number, },
				head: { line: number, ch: number },
			}[], cv, index, selectedLines,) => {
				const line = cv - 1;

				if(index === 0 || line - pv[pv.length - 1].head.line !== 1) {
					pv.push({
						anchor: { line, ch: index === 0 ? start.ch : 0, },
						head: {
							line,
							ch: index === selectedLines.length - 1
								? end.ch
								: 1000,
						},
					});
				} else {
					pv[pv.length - 1].head = {
						line,
						ch: index === selectedLines.length - 1
							? end.ch
							: 1000,
					};
				}

				return pv;
			}, []);
		if(newRanges.length === 0) {
			data.update([{ head: end, anchor: end }]);
		} else {
			data.update(newRanges);
		}
	};

	onEditorMount = (editor: Editor): void => {
		editor.setSize('auto', '100%');

		this.setState({
			editor,
		}, this.resetMarkers);
	};

	onCursorActivity = (): void => {
		const { reviews, editor, selectedReviewId, } = this.state;

		if(!editor) {
			return;
		}
		const doc = editor.getDoc();
		const cursor = doc.getCursor();

		document.addEventListener('mouseup', this.onMouseUp);

		if(doc.getSelection().length > 0) {
			if(selectedReviewId > -1) {
				this.highlightReview(-1, editor);
			}
			return;
		}

		this.highlightReview(getSelectedReviewIdByCursor(reviews, doc, cursor), editor);
	};

	selectComment = (e: React.MouseEvent<Element, MouseEvent> | React.FocusEvent, id: number,): void => {
		const { selectedReviewId, editor, } = this.state;
		e.stopPropagation();

		if(selectedReviewId !== id && editor) {
			this.highlightReview(id, editor);
		}
	};

	highlightReview = (id: number, editor: Editor): void => {
		const {
			markers,
			selectedReviewId,
		} = this.state;
		const doc = editor.getDoc();
		const newMarkers = { ...markers };

		const resetMarkers = (markers: TextMarker[], markerClass: string) =>
			markers.reduce((pv, marker) => {
				const position = marker.find() as MarkerRange;
				if(position) {
					const { from, to, } = position;
					marker.clear();
					pv.push(createTextMarker(to.line, to.ch, from.line, from.ch, markerClass, doc));
				}
				return pv;
			}, [] as TextMarker[]);

		if(newMarkers[selectedReviewId]) {
			newMarkers[selectedReviewId] = resetMarkers(newMarkers[selectedReviewId], styles.defaultMarker,);
		}

		if(newMarkers[id]) {
			newMarkers[id] = resetMarkers(newMarkers[id], styles.selectedMarker,);
		}
		this.setState({
			selectedReviewId: id,
			markers: newMarkers,
		});
	};
}

export default InstructorReview;
