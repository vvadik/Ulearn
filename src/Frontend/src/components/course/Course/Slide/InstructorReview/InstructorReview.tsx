import React from "react";

import { FLAT_THEME, Select, Tabs, ThemeContext, Toggle } from "ui";
import { Controlled, } from "react-codemirror2";

import { Review } from "../Blocks/Exercise/Review/Review";
import { BlocksWrapper, StaticCode } from "../Blocks";
import ScoreControls from "./ScoreControls/ScoreControls";
import diff_match_patch from 'diff-match-patch';
import 'codemirror/addon/selection/mark-selection.js';

import {
	addTextMarkerToReviews,
	createTextMarker,
	getAllReviewsFromSubmission,
	getPreviousManualCheckingInfo,
	getSelectedReviewIdByCursor,
	loadLanguageStyles, buildRange,
	replaceReviewMarker,
	ReviewInfoWithMarker,
} from "../Blocks/Exercise/ExerciseUtils";

import { InstructorReviewTabs } from "./InstructorReviewTabs";
import { Language } from "src/consts/languages";
import { ReviewInfo, SubmissionInfo } from "src/models/exercise";
import { ShortUserInfo } from "src/models/users";
import { ShortGroupInfo } from "src/models/comments";
import CodeMirror, { Editor, EditorConfiguration, TextMarker, } from "codemirror";

import texts from "./InstructorReview.texts";

import styles from './InstructorReview.less';
import AddCommentForm, { ReviewComment } from "./AddCommentForm/AddCommentForm";
import AntiplagiarismHeader, { AntiplagiarismInfo } from "./AntiplagiarismHeader/AntiplagiarismHeader";
import StickyWrapper from "./AntiplagiarismHeader/StickyWrapper";
import { KeyboardEventCodes } from "@skbkontur/react-ui/lib/events/keyboard/KeyboardEventCodes";

export interface Props {
	authorSolution: {
		code: string;
		language: Language;
	}
	formulation: React.ReactNode;
	studentSubmissions: SubmissionInfo[];
	comments: ReviewComment[];

	student: ShortUserInfo;
	group?: ShortGroupInfo;
	user: ShortUserInfo;

	prevReviewScore?: number;
	currentScore?: number;
	exerciseTitle: string;

	antiplagiarismStatus?: AntiplagiarismInfo;

	onScoreSubmit: (score: number) => void;
	onProhibitFurtherReviewToggleChange: (value: boolean) => void;
	onAddComment: (comment: string) => Promise<ReviewComment>;
	onAddCommentToFavourite: (comment: string) => Promise<ReviewComment>;
	onToggleCommentFavourite: (commentId: number) => void;
	getAntiPlagiarismStatus: () => Promise<AntiplagiarismInfo>;

	addReview: (
		submissionId: number,
		comment: string,
		startLine: number,
		startPosition: number,
		finishLine: number,
		finishPosition: number,
	) => Promise<ReviewInfo>;
	addReviewComment: (submissionId: number, reviewId: number, comment: string) => void;
	deleteReviewComment: (submissionId: number, reviewId: number, commentId?: number) => void;

	prohibitFurtherManualChecking: boolean;
}

interface BlockDiff {
	type?: 'added' | 'removed';
	line: number;
	code: string;
}

interface DiffInfo {
	prevReviewedSubmission: SubmissionInfo;
	diffByBlocks: BlockDiff[];
	addedLinesCount: number;
	removedLinesCount: number;
	deletedLinesSet: Set<number>;
	oldCodeNewLineIndex: number[];
	newCodeNewLineIndex: number[];
	code: string;
}

interface State {
	currentTab: InstructorReviewTabs;

	currentSubmission: SubmissionInfo;

	diffInfo?: DiffInfo;
	selectedReviewId: number;
	reviewsWithTextMarkers: ReviewInfoWithMarker[],

	editor: null | Editor;

	commentValue: string;
	addCommentFormCoords?: { left: number; top: number; bottom: number };

	showDiff: boolean;
	initialCode?: string;
}

class InstructorReview extends React.Component<Props, State> {
	private shameComment = 'Ой! Наш робот нашёл решения других студентов, подозрительно похожие на ваше. ' +
		'Так может быть, если вы позаимствовали части программы, взяли их из открытых источников либо сами поделились своим кодом. ' +
		'Выполняйте задания самостоятельно.';

	constructor(props: Props) {
		super(props);
		const submission = { ...props.studentSubmissions[0] };
		const prevSubmissionInfo = getPreviousManualCheckingInfo(props.studentSubmissions, 0);

		this.state = {
			selectedReviewId: -1,
			reviewsWithTextMarkers: [],
			currentTab: InstructorReviewTabs.Review,
			currentSubmission: submission,
			editor: null,
			commentValue: '',
			showDiff: false,
			diffInfo: prevSubmissionInfo
				? this.getDiffInfo(submission, prevSubmissionInfo.submission)
				: undefined,
		};
	}

	render(): React.ReactNode {
		const { student, group, studentSubmissions, prevReviewScore, currentScore, } = this.props;
		const { currentTab, } = this.state;

		return (
			<>
				<BlocksWrapper withoutBottomPaddings>
					<h3 className={ styles.reviewHeader }>
						<span className={ styles.reviewStudentName }>
							{ texts.getStudentInfo(student.visibleName, group?.name) }
						</span>
						{ texts.getReviewInfo(studentSubmissions, prevReviewScore, currentScore) }
					</h3>
					<Tabs value={ currentTab } onValueChange={ this.onTabChange }>
						<Tabs.Tab key={ InstructorReviewTabs.Review } id={ InstructorReviewTabs.Review }>
							{ texts.getTabName(InstructorReviewTabs.Review) }
						</Tabs.Tab>
						<Tabs.Tab key={ InstructorReviewTabs.Formulation } id={ InstructorReviewTabs.Formulation }>
							{ texts.getTabName(InstructorReviewTabs.Formulation) }
						</Tabs.Tab>
						<Tabs.Tab key={ InstructorReviewTabs.AuthorSolution }
								  id={ InstructorReviewTabs.AuthorSolution }>
							{ texts.getTabName(InstructorReviewTabs.AuthorSolution) }
						</Tabs.Tab>
					</Tabs>
				</BlocksWrapper>
				<div className={ styles.separator }/>
				<BlocksWrapper>
					{ this.renderCurrentTab() }
				</BlocksWrapper>
			</>
		);
	}

	onTabChange = (value: string): void => {
		this.setState({ currentTab: value as InstructorReviewTabs });
	};

	renderCurrentTab(): React.ReactNode {
		const { formulation, } = this.props;
		const { currentTab, } = this.state;


		switch (currentTab) {
			case InstructorReviewTabs.Review: {
				return this.renderSubmissions();
			}
			case InstructorReviewTabs.Formulation: {
				return formulation;
			}
			case InstructorReviewTabs.AuthorSolution: {
				return this.renderAuthorSolution();
			}
		}
	}

	renderAuthorSolution(): React.ReactElement {
		const { authorSolution } = this.props;
		return <StaticCode language={ authorSolution.language } code={ authorSolution.code }/>;
	}

	renderSubmissions(): React.ReactElement {
		const {
			exerciseTitle,
			prevReviewScore,
			onScoreSubmit,
			currentScore,
			onProhibitFurtherReviewToggleChange,
			prohibitFurtherManualChecking,
			getAntiPlagiarismStatus,
		} = this.props;

		return (
			<>
				{ this.renderTopControls() }
				<StickyWrapper
					stickerClass={ styles.wrapperStickerStopper }
					sticker={ (fixed) =>
						<AntiplagiarismHeader
							fixed={ fixed }
							shouldCheck={ true }
							getAntiPlagiarismStatus={ getAntiPlagiarismStatus }
							onZeroScoreButtonPressed={ this.onZeroScoreButtonPressed }
						/>
					}
					content={ this.renderEditor() }
				/>
				<ScoreControls
					curReviewScore={ currentScore ?? currentScore }
					prevReviewScore={ prevReviewScore }
					exerciseTitle={ exerciseTitle }
					onSubmit={ onScoreSubmit }
					onToggleChange={ onProhibitFurtherReviewToggleChange }
					toggleChecked={ prohibitFurtherManualChecking }
				/>
			</>
		);
	}

	onZeroScoreButtonPressed = (): void => {
		const {
			onScoreSubmit,
			onProhibitFurtherReviewToggleChange,
			addReview,
		} = this.props;
		const {
			currentSubmission,
		} = this.state;

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

	renderEditor = (): React.ReactElement => {
		const {
			user,
			comments,
			onAddCommentToFavourite,
			onToggleCommentFavourite,
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
						disableLineNumbers={ diffInfo && showDiff }
						reviewBackgroundColor={ 'gray' }
						editor={ editor }
						userId={ user.id }
						addReviewComment={ this.onAddReviewComment }
						deleteReviewOrComment={ this.onDeleteReviewOrComment }
						selectedReviewId={ selectedReviewId }
						onSelectComment={ this.selectComment }
						reviews={ reviewsWithTextMarkers }
					/>
					}
				</div>
				{ addCommentFormCoords &&
				<AddCommentForm
					value={ commentValue }
					onValueChange={ this.onCommentFormValueChange }
					addComment={ this.onFormAddComment }
					comments={ comments }
					addCommentToFavourite={ onAddCommentToFavourite }
					toggleCommentFavourite={ onToggleCommentFavourite }
					coordinates={ addCommentFormCoords }
					onClose={ this.onFormClose }
				/> }
			</div>
		);
	};

	mock = (): void => {
		return;
	};

	onCommentFormValueChange = (comment: string): void => {
		this.setState({
			commentValue: comment,
		});
	};

	onAddReviewComment = (reviewId: number, comment: string): void => {
		const { addReviewComment, } = this.props;
		const { currentSubmission, } = this.state;

		addReviewComment(currentSubmission.id, reviewId, comment);
	};

	onDeleteReviewOrComment = (reviewId: number, commentId?: number): void => {
		const { deleteReviewComment, } = this.props;
		const { currentSubmission, } = this.state;

		deleteReviewComment(currentSubmission.id, reviewId, commentId);
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

	renderSubmissionsSelect = (): React.ReactElement => {
		const { currentSubmission } = this.state;
		const { studentSubmissions, } = this.props;

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
		}, this.addMarkers);
	};

	onSubmissionsSelectValueChange = (id: unknown): void => {
		const { studentSubmissions, } = this.props;
		const { reviewsWithTextMarkers, } = this.state;

		reviewsWithTextMarkers.forEach(r => r.markers.forEach((m: TextMarker) => m.clear()));

		const currentSubmissionIndex = studentSubmissions.findIndex(s => s.id === id);
		const currentSubmission = studentSubmissions[currentSubmissionIndex];
		const prevSubmission = getPreviousManualCheckingInfo(studentSubmissions, currentSubmissionIndex);
		const diffInfo: DiffInfo | undefined = prevSubmission
			? this.getDiffInfo(currentSubmission, prevSubmission.submission)
			: undefined;

		this.setState({
			currentSubmission,
			diffInfo,
			selectedReviewId: -1,
			reviewsWithTextMarkers: [],
			addCommentFormCoords: undefined,
		}, this.addMarkers);
	};

	addMarkers = (): void => {
		const { editor, currentSubmission, diffInfo, showDiff, } = this.state;

		if(!editor) {
			return;
		}
		const doc = editor.getDoc();
		let newReviews = getAllReviewsFromSubmission(currentSubmission);
		let reviewsWithTextMarkers: ReviewInfoWithMarker[] = [];

		if(showDiff && diffInfo) {
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
			const oldReviews = getAllReviewsFromSubmission(diffInfo.prevReviewedSubmission,)
				.map(r => ({
					...r,
					outdated: true,
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

		if(showDiff && diffInfo) {
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
		this.setState({
			reviewsWithTextMarkers,
		});
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
		let coords: { left: number, right: number, top: number, bottom: number } | undefined
			= undefined;

		for (const selection of selections) {
			const range = selection;
			const selectedText = doc.getSelection();

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
		});
		document.addEventListener('keydown', this.onEscPressed);
		document.removeEventListener('mouseup', this.onMouseUp);
	};

	onFormAddComment = (comment: string): void => {
		const {
			onAddComment,
			addReview,
		} = this.props;
		const {
			currentSubmission,
			editor,
			diffInfo,
			showDiff,
		} = this.state;

		this.setState({
			addCommentFormCoords: undefined,
			commentValue: '',
		});

		if(!editor) {
			return;
		}

		const doc = editor.getDoc();

		const selections = doc.listSelections();
		const firstSelection = selections[0];
		const startRange = this.getStartAndEndFromRange(firstSelection)[0];

		const lastSelection = selections[selections.length - 1];
		const endRange = this.getStartAndEndFromRange(lastSelection)[1];

		if(diffInfo && showDiff) {
			const actualStartLine = diffInfo.diffByBlocks[startRange.line].line - 1;
			const actualEndLine = diffInfo.diffByBlocks[endRange.line].line - 1;

			addReview(currentSubmission.id,
				comment,
				actualStartLine,
				startRange.ch,
				actualEndLine,
				endRange.ch,
			);
		} else {
			addReview(currentSubmission.id,
				comment,
				startRange.line,
				startRange.ch,
				endRange.line,
				endRange.ch
			);
		}
		onAddComment(comment);

		doc
			.getAllMarks()
			.forEach(m => m.className === styles.selectionToReviewMarker && m.clear());
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
		if(event.key === KeyboardEventCodes.Escape) {
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
		}, this.addMarkers);
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

	getDiffInfo = (submission: SubmissionInfo, prevSubmission: SubmissionInfo)
		: DiffInfo => {
		const diffByBlocks = [];
		const oldCodeNewLineIndex: number[] = [];
		const newCodeNewLineIndex = [];
		let addedCount = 0;
		let removedCount = 0;

		const diff = this.getDiff(submission.code, prevSubmission.code);
		let newCodeLineCounter = 1;
		let oldCodeLineCounter = 1;

		for (const [index, [type, code]] of diff.entries()) {
			const splitedLines = code.split('\n');
			const lines = index === diff.length - 1
			|| (index === diff.length - 2 && type === -1)
				? splitedLines
				: splitedLines.slice(0, -1);

			switch (type) {
				case -1: {//removed
					oldCodeNewLineIndex.push(
						...lines.map((
							_, index) => (diffByBlocks.length + index))
					);
					diffByBlocks.push(
						...lines.map(
							(l, index) => ({ code: l, type: 'removed', line: oldCodeLineCounter + index, }))
					);
					oldCodeLineCounter += lines.length;
					removedCount += lines.length;
					break;
				}
				case 0: {//same
					oldCodeNewLineIndex.push(
						...lines.map((
							_, index) => (diffByBlocks.length + index))
					);
					newCodeNewLineIndex.push(
						...lines.map((
							_, index) => (diffByBlocks.length + index))
					);
					diffByBlocks.push(
						...lines.map((
							l, index) => ({ code: l, line: newCodeLineCounter + index, }))
					);
					newCodeLineCounter += lines.length;
					oldCodeLineCounter += lines.length;
					break;
				}
				case 1: {//added
					newCodeNewLineIndex.push(
						...lines.map((
							_, index) => (diffByBlocks.length + index))
					);
					diffByBlocks.push(
						...lines.map(
							(l, index) => ({ code: l, type: 'added', line: newCodeLineCounter + index, }))
					);
					newCodeLineCounter += lines.length;
					addedCount += lines.length;
					break;
				}
			}
		}

		const deletedLines = diffByBlocks
			.filter(b => (b as BlockDiff).type === 'removed')
			.map(b => oldCodeNewLineIndex[b.line - 1] + 1);
		const deletedLinesSet = new Set(deletedLines);

		return {
			addedLinesCount: addedCount,
			removedLinesCount: removedCount,
			prevReviewedSubmission: prevSubmission,
			deletedLinesSet,
			diffByBlocks,
			oldCodeNewLineIndex,
			newCodeNewLineIndex,
			code: diffByBlocks
				.map(d => d.code)
				.join('\n'),
		};
	};

	getDiff = (code: string, previousCode: string): [type: -1 | 0 | 1, code: string][] => {
		//diff-match-patch is not package very well, so we just ignoring types errors
		// eslint-disable-next-line @typescript-eslint/ban-ts-comment
		// @ts-ignore
		const dmp = new diff_match_patch();
		const a = dmp.diff_linesToChars_(previousCode, code);
		const lineText1 = a.chars1;
		const lineText2 = a.chars2;
		const lineArray = a.lineArray;
		const diffs = dmp.diff_main(lineText1, lineText2, false);
		dmp.diff_charsToLines_(diffs, lineArray);
		return diffs;
	};
}

export default InstructorReview;
