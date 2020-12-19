import React from 'react';

import { Controlled, } from "react-codemirror2";
import { Checkbox, FLAT_THEME, Select, Tooltip, Toast, } from "ui";
import { Review } from "./Review/Review";
import { Lightbulb, } from "icons";
import { CongratsModal } from "./CongratsModal/CongratsModal.tsx";
import { ExerciseOutput, HasOutput } from "./ExerciseOutput/ExerciseOutput.tsx";
import { ExerciseFormHeader } from "./ExerciseFormHeader/ExerciseFormHeader.tsx";
import { ThemeContext } from "ui";

import PropTypes from 'prop-types';
import classNames from 'classnames';
import { connect } from "react-redux";

import { exerciseSolutions, loadFromCache, saveToCache } from "src/utils/localStorageManager";

import { sendCode, addReviewComment, deleteReviewComment, } from "src/actions/course";
import { userProgressVisitAcceptedSolutions } from "src/actions/userProgress";

import { constructPathToAcceptedSolutions, } from "src/consts/routes";
import {
	AutomaticExerciseCheckingResult as CheckingResult,
	AutomaticExerciseCheckingProcessStatus as ProcessStatus,
	SolutionRunStatus,
} from "src/models/exercise.ts";
import { userType } from "src/components/comments/commonPropTypes";

import CodeMirror from 'codemirror/lib/codemirror';
import 'codemirror/addon/edit/matchbrackets';
import 'codemirror/addon/hint/show-hint';
import 'codemirror/addon/hint/show-hint.css';
import 'codemirror/addon/hint/javascript-hint';
import 'codemirror/addon/hint/anyword-hint';
import 'codemirror/theme/darcula.css';
import './CodeMirrorAutocompleteExtension';

import styles from './Exercise.less';

import texts from './Exercise.texts';
import { GetSubmissionColor } from "./ExerciseUtils.ts";
import {
	HasSuccessSubmission,
	IsFirstRightAnswer,
	SubmissionIsLast,
	SubmissionIsLastSuccess
} from "./ExerciseUtils";
import Controls from "src/components/course/Course/Slide/Blocks/Exercise/Controls/Controls";

const editThemeName = 'darcula';
const defaultThemeName = 'default';
const newTry = { id: -1 };

class Exercise extends React.Component {
	constructor(props) {
		super(props);
		const { exerciseInitialCode, submissions, languages, } = props;

		this.state = {
			value: exerciseInitialCode,
			valueChanged: false,

			isEditable: submissions.length === 0,

			language: languages[0],

			showedHintsCount: 0,
			congratsModalData: null,

			submissionLoading: false,
			visibleCheckingResponse: null, // Не null только если только что сделанная посылка не содержит submission
			currentSubmission: null,
			currentReviews: [],
			selectedReviewId: -1,
			showOutput: false,

			editor: null,
			exerciseCodeDoc: null,

			selfChecks: texts.checkups.self.checks.map((ch, i) => ({
				text: ch,
				checked: false,
				onClick: () => this.onSelfCheckBoxClick(i)
			})),
		}
	}

	componentDidMount() {
		const { forceInitialCode, } = this.props;
		this.overrideCodeMirrorAutocomplete();

		if(forceInitialCode) {
			this.resetCode();
		} else {
			this.loadSlideSubmission();
		}

		window.addEventListener("beforeunload", this.saveCodeDraftToCache);
	}

	loadSlideSubmission = () => {
		const { slideId, submissions, } = this.props;

		if(submissions.length > 0) {
			this.loadSubmissionToState(submissions[0]);
		} else {
			this.loadCodeFromCache(slideId);
		}
	}

	componentDidUpdate(prevProps, prevState, snapshot) {
		const { lastCheckingResponse, courseId, slideId, submissions, forceInitialCode, submissionError, } = this.props;
		const { currentSubmission, submissionLoading, showOutput, selectedReviewId, } = this.state;

		if(submissionError && submissionError !== prevProps.submissionError) {
			Toast.push("При добавлении или удалении комментария произошла ошибка");
		}

		if(forceInitialCode !== prevProps.forceInitialCode) {
			if(forceInitialCode) {
				this.resetCode();
			} else {
				this.loadSlideSubmission();
			}
			return;
		}

		if(courseId !== prevProps.courseId || slideId !== prevProps.slideId) {
			this.loadSlideSubmission();
			this.setState({ showedHintsCount: 0 });
			return;
		}

		const hasNewLastCheckingResponse = lastCheckingResponse
			&& lastCheckingResponse !== prevProps.lastCheckingResponse; // Сравнение по ссылкам
		if(hasNewLastCheckingResponse) {
			const { submission, solutionRunStatus } = lastCheckingResponse;

			if(submission) {
				this.loadSubmissionToState(submissions.find(s => s.id === submission.id));
			} else {
				this.setState({
					visibleCheckingResponse: lastCheckingResponse,
				});
			}
			if(submissionLoading) {
				this.setState({
					submissionLoading: false,
				});
			}
			if(!showOutput) {
				this.setState({
					showOutput: true,
				});
			}

			if(solutionRunStatus === SolutionRunStatus.Success) {
				const { automaticChecking } = submission;

				if((!automaticChecking || automaticChecking.result === CheckingResult.RightAnswer)
					&& IsFirstRightAnswer(submissions, submission)) {
					this.openModal({
						score: lastCheckingResponse.score,
						waitingForManualChecking: lastCheckingResponse.waitingForManualChecking,
					});
				}
			}
		} else if(currentSubmission) {
			const submission = submissions.find(s => s.id === currentSubmission.id);

			if(currentSubmission !== submission) { // Сравнение по ссылке. Отличаться должны только в случае изменения комментериев
				this.setCurrentSubmission(submission,
					() => this.highlightReview(selectedReviewId)); //Сохраняем выделение выбранного ревью
			}
		}
	}

	overrideCodeMirrorAutocomplete = () => {
		CodeMirror.commands.autocomplete = (cm) => {
			const { language, } = this.state;
			const hint = CodeMirror.hint[language.toLowerCase()];
			if(hint) {
				cm.showHint({ hint: hint });
			}
		};
	}

	saveCodeDraftToCache = () => {
		const { slideId, forceInitialCode, } = this.props;
		const { value, } = this.state;

		if(!forceInitialCode) {
			this.saveCodeToCache(slideId, value);
		}
	}

	componentWillUnmount() {
		this.saveCodeDraftToCache();
		window.removeEventListener("beforeunload", this.saveCodeDraftToCache);
	}

	render() {
		const { className, } = this.props;

		const opts = this.codeMirrorOptions;

		return (
			<div className={ classNames(styles.wrapper, className) }>
				{ this.renderControlledCodeMirror(opts) }
			</div>
		);
	}

	get codeMirrorOptions() {
		const { isAuthenticated, } = this.props;
		const { isEditable, language } = this.state;

		return {
			mode: Exercise.loadLanguageStyles(language),
			lineNumbers: true,
			scrollbarStyle: 'null',
			lineWrapping: true,
			theme: isEditable ? editThemeName : defaultThemeName,
			readOnly: !isEditable || !isAuthenticated,
			matchBrackets: true,
			tabSize: 4,
			indentUnit: 4,
			indentWithTabs: true,
			extraKeys: {
				ctrlSpace: "autocomplete",
				".": function (cm) {
					setTimeout(function () {
						cm.execCommand("autocomplete");
					}, 100);
					cm.replaceSelection('.');
				}
			},
		};
	}

	renderControlledCodeMirror = (opts) => {
		const {
			expectedOutput, submissions, author,
			slideProgress, maxScore, languages,
			courseId, slideId, hideSolutions, hints,
			attemptsStatistics,
		} = this.props;
		const {
			value, showedHintsCount, currentSubmission,
			isEditable, exerciseCodeDoc, congratsModalData,
			currentReviews, showOutput, selectedReviewId, visibleCheckingResponse,
			submissionLoading, valueChanged,
		} = this.state;

		const isReview = !isEditable && currentReviews.length > 0;
		const automaticChecking = currentSubmission?.automaticChecking ?? visibleCheckingResponse?.automaticChecking;
		const selectedSubmissionIsLast = SubmissionIsLast(submissions, currentSubmission);
		const selectedSubmissionIsLastSuccess = SubmissionIsLastSuccess(submissions, currentSubmission);
		const isMaxScore = slideProgress.score === maxScore;
		const submissionColor = GetSubmissionColor(visibleCheckingResponse?.solutionRunStatus, automaticChecking?.result,
			HasSuccessSubmission(submissions), selectedSubmissionIsLast, selectedSubmissionIsLastSuccess,
			slideProgress.prohibitFurtherManualChecking, slideProgress.isSkipped, isMaxScore);

		const wrapperClassName = classNames(
			styles.exercise,
			{ [styles.reviewWrapper]: isReview },
		);
		const editorClassName = classNames(
			styles.editor,
			{ [styles.editorWithoutBorder]: isEditable },
			{ [styles.editorInReview]: isReview },
		);

		const hasOutput = currentSubmission
			&& HasOutput(visibleCheckingResponse?.message, currentSubmission.automaticChecking,
				expectedOutput);

		return (
			<React.Fragment>
				{ submissions.length !== 0 && this.renderSubmissionsSelect() }
				{ languages.length > 1 && (submissions.length > 0 || isEditable) && this.renderLanguageSelect() }
				{ !isEditable && this.renderHeader(submissionColor, selectedSubmissionIsLast, selectedSubmissionIsLastSuccess) }
				<div className={ wrapperClassName }>
					<Controlled
						onBeforeChange={ this.onBeforeChange }
						editorDidMount={ this.onEditorMount }
						onCursorActivity={ this.onCursorActivity }
						className={ editorClassName }
						options={ opts }
						value={ value }
					/>
					{ !isEditable && this.renderEditButton(isReview) }
					{ exerciseCodeDoc && isReview &&
					<Review
						userId={ author.id }
						addReviewComment={ this.addReviewComment }
						deleteReviewComment={ this.deleteReviewComment }
						selectedReviewId={ selectedReviewId }
						onSelectComment={ this.selectComment }
						reviews={ this.getReviewsWithoutDeleted(currentReviews) }
						getReviewAnchorTop={ this.getReviewAnchorTop }
					/>
					}
				</div>
				{/* TODO not included in current release !isEditable && currentSubmission && this.renderOverview(currentSubmission)*/ }
				<Controls
					hasOutput={ hasOutput }
					hideSolutions={ hideSolutions }
					countOfHints={ hints.length }
					isEditable={ isEditable }
					showOutput={ showOutput }
					submissionLoading={ submissionLoading }
					valueChanged={ valueChanged }
					attemptsStatistics={ attemptsStatistics }
					isShowAcceptedSolutionsAvailable={ submissions.length > 0 || slideProgress.isSkipped }
					acceptedSolutionsUrl={ constructPathToAcceptedSolutions(courseId, slideId) }
					showedHintsCount={ showedHintsCount }

					onResetButtonClicked={ this.resetCodeAndCache }
					onShowOutputButtonClicked={ this.toggleOutput }
					onVisitAcceptedSolutions={ this.onVisitAcceptedSolutions }
					onsSendExerciseButtonClicked={ this.sendExercise }
					showHint={ this.showHint }
				/>
				{ showOutput && HasOutput(visibleCheckingResponse?.message, automaticChecking, expectedOutput) &&
				<ExerciseOutput
					solutionRunStatus={ visibleCheckingResponse?.solutionRunStatus ?? SolutionRunStatus.Success }
					message={ visibleCheckingResponse?.message }
					expectedOutput={ expectedOutput }
					automaticChecking={ automaticChecking }
					submissionColor={ submissionColor }
				/>
				}
				{ showedHintsCount > 0 && this.renderHints() }
				{ congratsModalData && this.renderCongratsModal(congratsModalData) }
			</React.Fragment>
		)
	}

	getReviewsWithoutDeleted = (reviews) => {
		return reviews.map(r => ({ ...r, comments: r.comments.filter(c => !c.isDeleted && !c.isLoading) }));
	}

	getReviewAnchorTop = (review) => {
		const { exerciseCodeDoc, } = this.state;

		return exerciseCodeDoc.cm.charCoords({
			line: review.startLine,
			ch: review.startPosition,
		}, 'local').top;
	}

	renderSubmissionsSelect = () => {
		const { currentSubmission } = this.state;
		const { submissions, } = this.props;
		const { waitingForManualChecking } = this.props.slideProgress;

		const submissionsWithNewTry = [newTry, ...submissions,];
		const items = submissionsWithNewTry.map((submission) => {
			const isLastSuccess = SubmissionIsLastSuccess(submissions, submission);
			const caption = submission === newTry
				? texts.submissions.newTry
				: texts.submissions.getSubmissionCaption(submission, isLastSuccess, waitingForManualChecking)
			return [submission.id, caption];
		});

		return (
			<div className={ styles.select }>
				<ThemeContext.Provider value={ FLAT_THEME }>
					<Select
						width={ '100%' }
						items={ items }
						value={ currentSubmission?.id || newTry.id }
						onValueChange={ (id) => this.loadSubmissionToState(submissionsWithNewTry.find(s => s.id === id)) }
					/>
				</ThemeContext.Provider>
			</div>
		);
	}

	renderLanguageSelect = () => {
		const { language } = this.state;
		const { languages, languageNames } = this.props;

		const items = languages.map((l) => {
			return [l, texts.getLanguageCaption(l, languageNames)];
		});

		return (
			<div className={ styles.select }>
				<ThemeContext.Provider value={ FLAT_THEME }>
					<Select
						width={ '100%' }
						items={ items }
						value={ language }
						onValueChange={ (l) => this.setState({ language: l }) }
					/>
				</ThemeContext.Provider>
			</div>
		);
	}

	renderHeader = (submissionColor, selectedSubmissionIsLast, selectedSubmissionIsLastSuccess) => {
		const { currentSubmission, visibleCheckingResponse } = this.state;
		const { waitingForManualChecking, prohibitFurtherManualChecking, score } = this.props.slideProgress;
		if(!currentSubmission && !visibleCheckingResponse)
			return null;
		return (
			<ExerciseFormHeader
				solutionRunStatus={ visibleCheckingResponse ? visibleCheckingResponse.solutionRunStatus : null }
				selectedSubmission={ currentSubmission }
				waitingForManualChecking={ waitingForManualChecking }
				prohibitFurtherManualChecking={ prohibitFurtherManualChecking }
				selectedSubmissionIsLast={ selectedSubmissionIsLast }
				selectedSubmissionIsLastSuccess={ selectedSubmissionIsLastSuccess }
				score={ score }
				submissionColor={ submissionColor }
			/>
		);
	}

	loadSubmissionToState = (submission,) => {
		const { valueChanged, } = this.state;

		if(submission === newTry) {
			this.loadNewTry();
			return;
		}
		if(valueChanged) {
			this.saveCodeDraftToCache();
		}
		this.clearAllTextMarkers();

		// Firstly we updating code in code mirror
		// when code is rendered we attaching reviewMarkers and loading reviews
		// after all is done we refreshing editor to refresh layout and sizes depends on reviews sizes
		this.setState({
				value: submission.code,
				language: submission.language,
				isEditable: false,
				valueChanged: false,
				showOutput: false,
				visibleCheckingResponse: null,
				currentReviews: [],
			}, () =>
				this.setCurrentSubmission(submission)
		);
	}

	setCurrentSubmission = (submission, callback) => {
		this.clearAllTextMarkers();
		this.setState({
			currentSubmission: submission,
			currentReviews: this.getReviewsWithTextMarkers(submission),
		}, () => {
			this.state.editor.refresh();
			if(callback) {
				callback();
			}
		})
	}

	openModal = (data) => {
		this.setState({
			congratsModalData: data,
		})
	}

	getReviewsWithTextMarkers = (submission) => {
		const reviews = this.getAllReviewsFromSubmission(submission);

		const reviewsWithTextMarkers = [];

		for (const review of reviews) {
			const { finishLine, finishPosition, startLine, startPosition } = review;
			const textMarker = this.highlightLine(finishLine, finishPosition, startLine, startPosition, styles.reviewCode);

			reviewsWithTextMarkers.push({
				marker: textMarker,
				...review
			});
		}

		return reviewsWithTextMarkers;
	}

	getAllReviewsFromSubmission = (submission) => {
		if(!submission) {
			return [];
		}

		const manual = submission.manualCheckingReviews || [];
		const auto = submission.automaticChecking && submission.automaticChecking.reviews ? submission.automaticChecking.reviews : [];
		return manual.concat(auto);
	}

	renderOverview = (submission) => {
		const { selfChecks } = this.state;
		const checkups = [
			{
				title: texts.checkups.self.title,
				content:
					<React.Fragment>
						<span className={ styles.overviewSelfCheckComment }>
							{ texts.checkups.self.text }
						</span>
						<ul className={ styles.overviewSelfCheckList }>
							{ this.renderSelfCheckBoxes(selfChecks) }
						</ul>
					</React.Fragment>
			},
		];

		if(submission.automaticChecking.reviews !== 0) {
			checkups.unshift(
				{
					title: texts.checkups.bot.title,
					content:
						<span className={ styles.overviewComment }>
						{ texts.checkups.bot.countBotComments(submission.automaticChecking.reviews) }
							<a onClick={ this.showFirstBotComment }>{ texts.showReview }</a>
					</span>
				});
		}

		if(submission.manualCheckingReviews.length !== 0) {
			const reviewsCount = submission.reviews.length;

			checkups.unshift({
				title: texts.checkups.teacher.title,
				content:
					<span className={ styles.overviewComment }>
						{ texts.checkups.teacher.countTeacherReviews(reviewsCount) }
						<a onClick={ this.showFirstComment }>{ texts.showReview }</a>
					</span>
			});
		}

		return (
			<ul className={ styles.overview }>
				{ checkups.map(({ title, content }) =>
					<li key={ title } className={ styles.overviewLine } title={ title }>
						<h3>{ title }</h3>
						{ content }
					</li>
				) }
			</ul>
		);
	}

	renderSelfCheckBoxes = (selfChecks) => {
		return (
			selfChecks.map(({ text, checked, onClick, }, i) =>
				<li key={ i }>
					<Checkbox checked={ checked } onClick={ onClick }/> <span
					className={ styles.selfCheckText }>{ text }</span>
				</li>
			)
		);
	}

	onSelfCheckBoxClick = (i) => {
		const { selfChecks } = this.state;
		const newSelfChecks = [...selfChecks];

		newSelfChecks[i].checked = !newSelfChecks[i].checked;

		this.setState({
			selfChecks: newSelfChecks,
		});
	}

	renderHints = () => {
		const { showedHintsCount } = this.state;
		const { hints } = this.props;

		return (
			<ul className={ styles.hintsWrapper }>
				{ hints.slice(0, showedHintsCount)
					.map((h, i) =>
						<li key={ i }>
							<span className={ styles.hintBulb }><Lightbulb/></span>
							{ h }
						</li>
					) }
			</ul>
		)
	}

	renderCongratsModal = ({ score, waitingForManualChecking, }) => {
		const { hideSolutions, } = this.props;

		return (
			<CongratsModal
				showAcceptedSolutions={ !waitingForManualChecking && !hideSolutions }
				score={ score }
				waitingForManualChecking={ waitingForManualChecking }
				onClose={ this.closeCongratsModal }
			/>
		);
	}

	showFirstComment = () => {
		//TODO
	}

	showFirstBotComment = () => {
		//TODO
	}

	selectComment = (e, id,) => {
		const { isEditable, selectedReviewId, } = this.state;
		e.stopPropagation();

		if(!isEditable && selectedReviewId !== id) {
			this.highlightReview(id);
		}
	}

	highlightReview = (id) => {
		const { currentReviews, selectedReviewId, editor, } = this.state;
		const newCurrentReviews = [...currentReviews];

		if(selectedReviewId >= 0) {
			const selectedReview = newCurrentReviews.find(r => r.id === selectedReviewId);
			const { from, to, } = selectedReview.marker.find();
			selectedReview.marker.clear();
			selectedReview.marker = this.highlightLine(to.line, to.ch, from.line, from.ch, styles.reviewCode);
		}

		let line = 0;
		if(id >= 0) {
			const review = newCurrentReviews.find(r => r.id === id);
			const { from, to, } = review.marker.find();
			review.marker.clear();
			review.marker = this.highlightLine(to.line, to.ch, from.line, from.ch, styles.selectedReviewCode);

			line = from.line;
		}

		this.setState({
			currentReviews: newCurrentReviews,
			selectedReviewId: id,
		}, () => {
			if(id >= 0) {
				editor.scrollIntoView({ line, }, 200);
			}
		});
	}

	highlightLine = (finishLine, finishPosition, startLine, startPosition, className) => {
		const { exerciseCodeDoc } = this.state;

		return exerciseCodeDoc.markText({
			line: startLine,
			ch: startPosition
		}, {
			line: finishLine,
			ch: finishPosition
		}, {
			className,
		});
	}

	renderEditButton = (isReview) => {
		return (
			<div className={ classNames(styles.editButton, { [styles.editButtonWithReviews]: isReview }) }
				 onClick={ this.enableEditing }>
				{ texts.controls.edit.text }
			</div>
		)
	}

	renderShowStatisticsHint = () => {
		const {
			attemptedUsersCount,
			usersWithRightAnswerCount,
			lastSuccessAttemptDate,
		} = this.props.attemptsStatistics;
		const statisticsClassName = classNames(styles.exerciseControls, styles.statistics);

		return (
			<span className={ statisticsClassName }>
					<Tooltip pos={ "bottom right" } trigger={ "hover&focus" } render={
						() =>
							<span>
								{ texts.controls.statistics.buildStatistics(attemptedUsersCount, usersWithRightAnswerCount, lastSuccessAttemptDate) }
							</span>
					}>
						{ texts.controls.statistics.buildShortText(usersWithRightAnswerCount) }
					</Tooltip>
				</span>
		);
	}

	enableEditing = (e) => {
		e.stopPropagation();

		this.clearAllTextMarkers();
		this.setState({
			isEditable: true,
			valueChanged: true,
			currentSubmission: null,
			visibleCheckingResponse: null,
			currentReviews: [],
			showOutput: false
		})
	}

	showHint = () => {
		const { showedHintsCount, } = this.state;
		const { hints, } = this.props;

		this.setState({
			showedHintsCount: Math.min(showedHintsCount + 1, hints.length),
		})
	}

	resetCodeAndCache = () => {
		const { slideId, exerciseInitialCode, } = this.props;

		this.resetCode();
		this.saveCodeToCache(slideId, exerciseInitialCode);
	}

	resetCode = () => {
		const { exerciseInitialCode } = this.props;

		this.clearAllTextMarkers();
		this.setState({
			value: exerciseInitialCode,
			valueChanged: false,
			isEditable: true,
			currentSubmission: null,
			visibleCheckingResponse: null,
			currentReviews: [],
			showOutput: false
		});
	}

	clearAllTextMarkers = () => {
		const { currentReviews, } = this.state;

		currentReviews.forEach(({ marker }) => marker.clear());

		this.setState({
			selectedReviewId: -1,
		});
	}

	loadNewTry = () => {
		const { slideId } = this.props;
		this.resetCode();
		this.loadCodeFromCache(slideId);
	}

	toggleOutput = () => {
		const { showOutput, } = this.state;

		this.setState({
			showOutput: !showOutput
		})
	}

	onVisitAcceptedSolutions = () => {
		const { courseId, slideId, visitAcceptedSolutions, } = this.props;

		visitAcceptedSolutions(courseId, slideId);
	}

	closeCongratsModal = () => {
		this.setState({
			congratsModalData: null,
		})
	}

	sendExercise = () => {
		const { value, language } = this.state;
		const { courseId, slideId, } = this.props;

		this.setState({
			submissionLoading: true,
		});

		this.props.sendCode(courseId, slideId, value, language);
	}

	addReviewComment = (reviewId, text) => {
		const { addReviewComment, courseId, slideId, } = this.props;
		const { currentSubmission, } = this.state;


		addReviewComment(courseId, slideId, currentSubmission.id, reviewId, text);
	}

	deleteReviewComment = (reviewId, commentId,) => {
		const { deleteReviewComment, courseId, slideId, } = this.props;
		const { currentSubmission, } = this.state;

		deleteReviewComment(courseId, slideId, currentSubmission.id, reviewId, commentId,);
	}

	isSubmitResultsContainsError = ({ automaticChecking }) => {
		return automaticChecking.result === CheckingResult.CompilationError
			|| automaticChecking.result === CheckingResult.WrongAnswer;
	}

	onBeforeChange = (editor, data, value) => {
		this.setState({
			value,
			valueChanged: true,
		});
	}

	onEditorMount = (editor) => {
		editor.setSize('auto', '100%');
		this.setState({
			exerciseCodeDoc: editor.getDoc(),
			editor,
		})
	}

	onCursorActivity = () => {
		const { currentReviews, exerciseCodeDoc, isEditable, } = this.state;
		const cursor = exerciseCodeDoc.getCursor();

		if(!isEditable && currentReviews.length > 0) {
			const reviewId = Exercise.getSelectedReviewIdByCursor(currentReviews, exerciseCodeDoc, cursor);
			this.highlightReview(reviewId);
		}
	}

	static getSelectedReviewIdByCursor = (reviews, exerciseCodeDoc, cursor) => {
		const { line, ch } = cursor;
		const reviewsUnderCursor = reviews.filter(r =>
			r.startLine <= line && r.finishLine >= line
			&& !(r.startLine === line && ch < r.startPosition)
			&& !(r.finishLine === line && r.finishPosition < ch)
		);

		if(reviewsUnderCursor.length === 0) {
			return -1;
		}

		reviewsUnderCursor.sort((a, b) => {
			const aLength = Exercise.getReviewSelectionLength(a, exerciseCodeDoc);
			const bLength = Exercise.getReviewSelectionLength(b, exerciseCodeDoc);
			if(aLength !== bLength)
				return aLength - bLength;
			return a.startLine !== b.startLine
				? a.startLine - b.startLine
				: a.startPosition !== b.startPosition
					? a.startPosition - b.startPosition
					: a.timestamp - b.timestamp
		});

		return reviewsUnderCursor[0].id;
	}

	static
	getReviewSelectionLength = (review, exerciseCodeDoc) =>
		exerciseCodeDoc.indexFromPos({ line: review.finishLine, ch: review.finishPosition })
		- exerciseCodeDoc.indexFromPos({ line: review.startLine, ch: review.startPosition });

	static
	loadLanguageStyles = (language) => {
		switch (language.toLowerCase()) {
			case 'csharp':
				require('codemirror/mode/clike/clike');
				return `text/x-csharp`;
			case 'java':
				require('codemirror/mode/clike/clike');
				return `text/x-java`;

			case 'javascript':
				require('codemirror/mode/javascript/javascript');
				return `text/javascript`;
			case 'typescript':
				require('codemirror/mode/javascript/javascript');
				return `text/typescript`;
			case 'jsx':
				require('codemirror/mode/jsx/jsx');
				return `text/jsx`;

			case 'python2':
				require('codemirror/mode/python/python');
				return `text/x-python`;
			case 'python3':
				require('codemirror/mode/python/python');
				return `text/x-python`;

			case 'css':
				require('codemirror/mode/css/css');
				return `text/css`;

			case 'haskell':
				require('codemirror/mode/haskell/haskell');
				return `text/x-haskell`;

			case 'c':
				require('codemirror/mode/clike/clike');
				return `text/x-c`;
			case 'cpp':
				require('codemirror/mode/clike/clike');
				return `text/x-c++src`;

			default:
				require('codemirror/mode/xml/xml');
				return 'text/html';
		}
	}

	saveCodeToCache = (slideId, value) => {
		saveToCache(exerciseSolutions, slideId, value);
	}

	loadCodeFromCache = (slideId) => {
		const code = loadFromCache(exerciseSolutions, slideId);
		if(code !== undefined) {
			this.resetCode();
			this.setState({
				value: code,
			})
		}
	}
}

const mapStateToProps = (state, { courseId, slideId, }) => {
	const { slides, account, userProgress } = state;
	const { submissionsByCourses, submissionError, } = slides;
	let { lastCheckingResponse, } = slides;
	let slideProgress = userProgress?.progress[courseId]?.[slideId] || {};

	if(!(lastCheckingResponse && lastCheckingResponse.courseId === courseId && lastCheckingResponse.slideId === slideId))
		lastCheckingResponse = null;

	const submissions = Object.values(submissionsByCourses[courseId][slideId])
		.filter((s, i, arr) =>
			(i === arr.length - 1)
			|| (!s.automaticChecking || s.automaticChecking.result === CheckingResult.RightAnswer));

	//newer is first
	submissions.sort((s1, s2) => (new Date(s2.timestamp) - new Date(s1.timestamp)));

	return {
		isAuthenticated: account.isAuthenticated,
		submissions,
		submissionError,
		lastCheckingResponse,
		author: account,
		slideProgress
	};
};

const mapDispatchToProps = (dispatch) => ({
	sendCode: (courseId, slideId, code, language) => dispatch(sendCode(courseId, slideId, code, language)),
	addReviewComment: (courseId, slideId, submissionId, reviewId, comment) => dispatch(addReviewComment(courseId, slideId, submissionId, reviewId, comment)),
	deleteReviewComment: (courseId, slideId, submissionId, reviewId, comment) => dispatch(deleteReviewComment(courseId, slideId, submissionId, reviewId, comment)),
	visitAcceptedSolutions: (courseId, slideId,) => dispatch(userProgressVisitAcceptedSolutions(courseId, slideId,)),
});

const exerciseBlockProps = {
	languages: PropTypes.array.isRequired,
	languageNames: PropTypes.object,
	hints: PropTypes.array,
	exerciseInitialCode: PropTypes.string,
	hideSolutions: PropTypes.bool,
	expectedOutput: PropTypes.string,
	submissions: PropTypes.array,
	attemptsStatistics: PropTypes.object,
}
const dispatchFunctionsProps = {
	sendCode: PropTypes.func,
	addCommentToReview: PropTypes.func,
	deleteReviewComment: PropTypes.func,
	visitAcceptedSolutions: PropTypes.func,
}
const fromSlideProps = {
	courseId: PropTypes.string,
	slideId: PropTypes.string,
	maxScore: PropTypes.number,
	forceInitialCode: PropTypes.bool,
}
const fromMapStateToProps = {
	isAuthenticated: PropTypes.bool,
	lastCheckingResponse: PropTypes.object,
	author: userType,
	slideProgress: PropTypes.object,
}
Exercise.propTypes = {
	...exerciseBlockProps,
	...dispatchFunctionsProps,
	...fromSlideProps,
	...fromMapStateToProps,
	className: PropTypes.string,
}

export default connect(mapStateToProps, mapDispatchToProps)(Exercise);
