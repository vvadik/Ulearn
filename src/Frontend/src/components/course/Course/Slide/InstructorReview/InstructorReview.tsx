import React from "react";

import { FLAT_THEME, Select, Tabs, ThemeContext, Toggle } from "ui";
import { Controlled, } from "react-codemirror2";

import { Review } from "../Blocks/Exercise/Review/Review";
import { BlocksWrapper, } from "../Blocks";
import ScoreControls from "./ScoreControls/ScoreControls";
import 'codemirror/addon/selection/mark-selection.js';

import {
	addTextMarkerToReviews,
	createTextMarker,
	getAllReviewsFromSubmission,
	getPreviousManualCheckingInfo,
	getSelectedReviewIdByCursor,
	loadLanguageStyles,
	buildRange,
	replaceReviewMarker,
	ReviewInfoWithMarker,
} from "../Blocks/Exercise/ExerciseUtils";

import { InstructorReviewTabs } from "./InstructorReviewTabs";
import { Language } from "src/consts/languages";
import { SubmissionInfo } from "src/models/exercise";
import CodeMirror, { Editor, EditorConfiguration, TextMarker, } from "codemirror";
import { clone } from "src/utils/jsonExtensions";
import { DiffInfo, getDataFromReviewToCompareChanges, getDiffInfo } from "./utils";
import { Props, State } from "./InstructorReview.types";

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

		const { favouriteReviews, studentSubmissions } = props;

		let submission: SubmissionInfo | undefined = undefined;
		let prevSubmissionInfo = undefined;
		let favouriteReviewsSet = new Set<string>();
		let favouriteByUserSet = new Set<string>();
		let outdatedReviewsSet = new Set<number>();

		if(studentSubmissions) {
			submission = JSON.parse(JSON.stringify(studentSubmissions[0]));
			prevSubmissionInfo = getPreviousManualCheckingInfo(studentSubmissions, 0);
			favouriteReviewsSet = new Set(favouriteReviews?.filter(r => !r.isFavourite).map(r => r.text));
			favouriteByUserSet = new Set(favouriteReviews?.filter(r => r.isFavourite).map(r => r.text));
			outdatedReviewsSet = new Set(prevSubmissionInfo?.submission?.manualCheckingReviews
				.map(r => r.id)
				.concat(prevSubmissionInfo?.submission?.automaticChecking?.reviews?.map(r => r.id) || []));
		}

		this.state = {
			selectedReviewId: -1,
			reviewsWithTextMarkers: [],
			currentTab: InstructorReviewTabs.Review,
			currentSubmission: submission,
			editor: null,
			commentValue: '',
			showDiff: false,
			diffInfo: prevSubmissionInfo && submission
				? getDiffInfo(submission, prevSubmissionInfo.submission)
				: undefined,
			favouriteReviewsSet,
			favouriteByUserSet,
			outdatedReviewsSet,
		};
	}

	componentDidMount(): void {
		const {
			student,
			groups,
			studentSubmissions,
			getStudentInfo,
			getStudentSubmissions,
			getStudentGroups,
			studentId,
			favouriteReviews,
			getFavouriteReviews,
			slideContext: { courseId, slideId, },
		} = this.props;

		if(!student) {
			getStudentInfo(studentId);
		}
		if(!studentSubmissions) {
			getStudentSubmissions(studentId, courseId, slideId);
		}
		if(!groups) {
			getStudentGroups(courseId, studentId);
		}
		if(!favouriteReviews) {
			getFavouriteReviews(courseId, studentId);
		}
	}

	componentDidUpdate = (prevProps: Props): void => {
		const { studentSubmissions, favouriteReviews, } = this.props;
		const { currentSubmission, } = this.state;

		if(studentSubmissions && studentSubmissions.length > 0) {
			let submissionInPropsIndex = studentSubmissions.findIndex(s => s.id === currentSubmission?.id);
			if(submissionInPropsIndex < 0) {
				submissionInPropsIndex = 0;
			}
			const submissionInProps = studentSubmissions[submissionInPropsIndex];
			const submissionInPropsClone = clone(submissionInProps);
			if(!currentSubmission) {
				const prevSubmissionInfo = getPreviousManualCheckingInfo(studentSubmissions, submissionInPropsIndex);
				this.setState({
					currentSubmission: submissionInPropsClone,
					diffInfo: prevSubmissionInfo && submissionInPropsClone
						? getDiffInfo(submissionInPropsClone, prevSubmissionInfo.submission)
						: undefined,
				}, this.resetMarkers);
			} else {
				const newReviews = submissionInProps.manualCheckingReviews.map(getDataFromReviewToCompareChanges);
				const oldReviews = currentSubmission.manualCheckingReviews.map(getDataFromReviewToCompareChanges);

				if(JSON.stringify(newReviews) !== JSON.stringify(oldReviews)
					|| JSON.stringify(favouriteReviews) !== JSON.stringify(prevProps.favouriteReviews)) {

					this.setState({
						currentSubmission: submissionInPropsClone,
					}, this.resetMarkers);
				}
			}
		}
	};

	static getDerivedStateFromProps(props: Readonly<Props>, state: Readonly<State>): Partial<State> | null {
		const { favouriteReviews, } = props;
		const prevReviewedSubmission = state.diffInfo?.prevReviewedSubmission;

		return {
			favouriteReviewsSet: new Set(favouriteReviews?.filter(r => !r.isFavourite).map(r => r.text)),
			favouriteByUserSet: new Set(favouriteReviews?.filter(r => r.isFavourite).map(r => r.text)),
			outdatedReviewsSet: new Set(prevReviewedSubmission?.manualCheckingReviews
				.map(r => r.id)
				.concat(prevReviewedSubmission?.automaticChecking?.reviews?.map(r => r.id) || [])),
		};
	}

	render(): React.ReactNode {
		const {
			student,
			groups,
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

		if(!student || !studentSubmissions || !groups || !favouriteReviews || !currentSubmission) {
			return <CourseLoader/>;
		}

		return (
			<>
				<BlocksWrapper withoutBottomPaddings>
					<h3 className={ styles.reviewHeader }>
						<span className={ styles.reviewStudentName }>
							{ texts.getStudentInfo(student.visibleName, groups[0].name) }
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
			editor,
			selectedReviewId,
			diffInfo,
			showDiff,
			addCommentFormCoords,
			reviewsWithTextMarkers,
			commentValue,
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
					{ reviewsWithTextMarkers.length > 0 &&
					<Review
						backgroundColor={ 'gray' }
						editor={ editor }
						user={ user }
						addReviewComment={ this.onAddReviewComment }
						toggleReviewFavourite={ this.onToggleReviewFavouriteByReviewId }
						deleteReviewOrComment={ this.onDeleteReviewOrComment }
						editReviewOrComment={ this.editReviewOrComment }
						selectedReviewId={ selectedReviewId }
						onReviewClick={ this.selectComment }
						reviews={ reviewsWithTextMarkers.map(r => ({ ...r, markers: undefined, })) }

						isReviewCanBeAdded={ this.isReviewCanBeAdded }
						isReviewFavourite={ this.isReviewFavourite }
						isReviewOutdated={ this.isReviewOutdated }
					/>
					}
				</div>
				{ addCommentFormCoords &&
				<AddCommentForm
					value={ commentValue }
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
		const { commentValue, favouriteByUserSet, } = this.state;
		const trimmed = this.removeWhiteSpaces(commentValue);

		return trimmed.length > 0 && !favouriteByUserSet.has(trimmed);
	};

	isReviewFavourite = (reviewText: string): boolean => {
		const { favouriteByUserSet, } = this.state;
		const trimmed = this.removeWhiteSpaces(reviewText);

		return favouriteByUserSet.has(trimmed);
	};

	isReviewOutdated = (reviewId: number): boolean => {
		const { outdatedReviewsSet, } = this.state;
		return outdatedReviewsSet.has(reviewId);
	};

	editReviewOrComment = (text: string, id: number, reviewId?: number,): void => {
		const {
			editReviewOrComment,
		} = this.props;
		const {
			currentSubmission,
		} = this.state;

		if(currentSubmission) {
			editReviewOrComment(text, currentSubmission.id, id, reviewId);
		}
	};

	onCommentFormValueChange = (comment: string): void => {
		this.setState({
			commentValue: comment,
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
		this.setState({
			showDiff: value,
			selectedReviewId: -1,
			reviewsWithTextMarkers: [],
			addCommentFormCoords: undefined,
		}, this.resetMarkers);
	};

	onSubmissionsSelectValueChange = (id: unknown): void => {
		const { studentSubmissions, } = this.props;

		if(!studentSubmissions) {
			return;
		}

		const currentSubmissionIndex = studentSubmissions.findIndex(s => s.id === id);
		const currentSubmission = JSON.parse(JSON.stringify(studentSubmissions[currentSubmissionIndex]));
		const prevSubmission = getPreviousManualCheckingInfo(studentSubmissions, currentSubmissionIndex);
		const diffInfo: DiffInfo | undefined = prevSubmission
			? getDiffInfo(currentSubmission, prevSubmission.submission)
			: undefined;

		this.setState({
			currentSubmission,
			diffInfo,
			selectedReviewId: -1,
			reviewsWithTextMarkers: [],
			addCommentFormCoords: undefined,
		}, this.resetMarkers);
	};

	resetMarkers = (): void => {
		const {
			favouriteReviews,
		} = this.props;
		const {
			editor,
			currentSubmission,
			diffInfo,
			showDiff,
			reviewsWithTextMarkers,
			selectedReviewId,
		} = this.state;

		reviewsWithTextMarkers.forEach(r => r.markers.forEach((m: TextMarker) => m.clear()));

		if(!editor || !currentSubmission || !favouriteReviews) {
			return;
		}

		let newReviewsWithTextMarkers = InstructorReview.getMarkers(
			editor,
			currentSubmission,
			showDiff ? diffInfo : undefined,);

		newReviewsWithTextMarkers = replaceReviewMarker(
			newReviewsWithTextMarkers,
			selectedReviewId,
			selectedReviewId,
			editor.getDoc(),
			styles.defaultMarker,
			styles.selectedMarker,
		).reviews;

		this.setState({
			reviewsWithTextMarkers: newReviewsWithTextMarkers,
		});
	};

	static getMarkers = (
		editor: Editor,
		submission: SubmissionInfo,
		diffInfo: DiffInfo | undefined,
	): ReviewInfoWithMarker[] => {
		const doc = editor.getDoc();
		let newReviews = getAllReviewsFromSubmission(submission);
		let reviewsWithTextMarkers: ReviewInfoWithMarker[] = [];

		if(diffInfo) {
			//reviews from current submission
			newReviews = newReviews.map(r => ({
				...r,
				startLine: diffInfo.newCodeNewLineIndex[r.startLine],
				finishLine: diffInfo.newCodeNewLineIndex[r.finishLine],
			}));
			reviewsWithTextMarkers.push(
				...addTextMarkerToReviews(
					newReviews,
					doc,
					styles.defaultMarker,
					diffInfo?.deletedLinesSet,
				));
			//reviews from previously reviewed submission
			const oldReviews = getAllReviewsFromSubmission(diffInfo.prevReviewedSubmission,)
				.map(r => ({
					...r,
					startLine: diffInfo.oldCodeNewLineIndex[r.startLine],
					finishLine: diffInfo.oldCodeNewLineIndex[r.finishLine],
				}));
			reviewsWithTextMarkers.push(
				...addTextMarkerToReviews(
					oldReviews,
					doc,
					styles.defaultMarker,
				));
		} else {
			reviewsWithTextMarkers = addTextMarkerToReviews(
				newReviews,
				doc,
				styles.defaultMarker,
			);
		}

		if(diffInfo) {
			//addding a line class via addLineClass triggers rerender on each method call, it cause perfomance issues
			//instead of it we can add class directly to line wrappers
			const linesWrapper = editor.getWrapperElement().getElementsByClassName('CodeMirror-code')[0];
			for (let i = 0; i < diffInfo.diffByBlocks.length; i++) {
				const { type, } = diffInfo.diffByBlocks[i];
				if(type) {
					const lineWrapper = linesWrapper.children[i];
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
		}

		return reviewsWithTextMarkers;
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
			ranges: { startRange, endRange, },
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
			ranges,
		} = this.state;

		this.setState({
			addCommentFormCoords: undefined,
			commentValue: '',
		});

		if(!editor || !currentSubmission || !ranges) {
			return;
		}
		const { startRange, endRange, } = ranges;

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
		const { reviewsWithTextMarkers, editor, selectedReviewId, } = this.state;

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

		const id = getSelectedReviewIdByCursor(reviewsWithTextMarkers, doc, cursor);
		this.highlightReview(id, editor);
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
			reviewsWithTextMarkers,
			selectedReviewId,
		} = this.state;

		const newReviews = replaceReviewMarker(
			reviewsWithTextMarkers,
			selectedReviewId,
			id,
			editor.getDoc(),
			styles.defaultMarker,
			styles.selectedMarker,
		).reviews;

		this.setState({
			selectedReviewId: id,
			reviewsWithTextMarkers: newReviews,
		});
	};
}

export default InstructorReview;
