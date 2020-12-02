import React from 'react';

import { Controlled, } from "react-codemirror2";
import { Button, Checkbox, FLAT_THEME, Modal, Select, Tooltip, Toast, } from "ui";
import Review from "./Review/Review";
import { darkTheme } from 'ui/internal/ThemePlayground/darkTheme';
import DownloadedHtmlContent from "src/components/common/DownloadedHtmlContent";
import { Lightbulb, Refresh, EyeOpened, DocumentLite, } from "icons";
import CongratsModal from "./CongratsModal/CongratsModal.tsx";
import { ExerciseOutput, HasOutput } from "./ExerciseOutput/ExerciseOutput.tsx";
import { ExerciseFormHeader } from "./ExerciseFormHeader/ExerciseFormHeader.tsx";
import { ThemeContext } from "@skbkontur/react-ui/index";

import PropTypes from 'prop-types';
import classNames from 'classnames';
import { connect } from "react-redux";

import { exerciseSolutions, loadFromCache, saveToCache } from "src/utils/localStorageManager";

import { sendCode, addReviewComment, deleteReviewComment, } from "src/actions/course";

import { constructPathToAcceptedSolutions, } from "src/consts/routes";
import {
	AutomaticExerciseCheckingResult as CheckingResult,
	AutomaticExerciseCheckingProcessStatus as ProcessStatus,
	SolutionRunStatus,
} from "src/models/exercise.ts";
import { isMobile, isTablet, } from "src/utils/getDeviceType";
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

const isControlsTextSuits = () => !isMobile() && !isTablet();
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
			showAcceptedSolutions: false,
			showAcceptedSolutionsWarning: false,
			congratsModalData: null,

			resizeTimeout: null,
			showControlsText: isControlsTextSuits(),

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
		window.addEventListener("resize", this.onWindowResize);
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

				if(automaticChecking?.processStatus === ProcessStatus.Done
					&& automaticChecking.result === CheckingResult.RightAnswer
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

	onWindowResize = () => {
		const { resizeTimeout } = this.state;

		const throttleTimeout = 66;

		//resize event can be called rapidly, to prevent performance issue, we throttling event handler
		if(!resizeTimeout) {
			this.setState({
				resizeTimeout: setTimeout(() => {
					this.setState({
						resizeTimeout: null,
						showControlsText: isControlsTextSuits(),
					})
				}, throttleTimeout),
			});
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

		if(!forceInitialCode) {
			this.saveCodeToCache(slideId);
		}
	}

	componentWillUnmount() {
		this.saveCodeDraftToCache();
		window.removeEventListener("beforeunload", this.saveCodeDraftToCache);
		window.removeEventListener("resize", this.onWindowResize);
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
		const { expectedOutput, submissions, author, slideProgress, maxScore, languages } = this.props;
		const {
			value, showedHintsCount, showAcceptedSolutions, currentSubmission,
			isEditable, exerciseCodeDoc, congratsModalData,
			currentReviews, showOutput, selectedReviewId, visibleCheckingResponse
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
						reviews={ currentReviews }
						getReviewAnchorTop={ this.getReviewAnchorTop }
					/>
					}
				</div>
				{/* TODO not included in current release !isEditable && currentSubmission && this.renderOverview(currentSubmission)*/ }
				{ this.renderControls() }
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
				{ showAcceptedSolutions && this.renderAcceptedSolutions() }
				{ congratsModalData && this.renderCongratsModal(congratsModalData) }
			</React.Fragment>
		)
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
		const { languages } = this.props;

		const items = languages.map((l) => {
			return [l, texts.getLanguageCaption(l)];
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

	renderControls = () => {
		const { hints, hideSolutions, slideProgress, submissions, expectedOutput } = this.props;
		const { isEditable, currentSubmission, showedHintsCount, visibleCheckingResponse } = this.state;

		return (
			<div className={ styles.exerciseControlsContainer }>
				{ this.renderSubmitSolutionButton() }
				<ThemeContext.Provider value={ darkTheme }>
					{ hints.length > 0 && this.renderShowHintButton() }
					{ isEditable && this.renderResetButton() }
					{ !isEditable && currentSubmission && HasOutput(visibleCheckingResponse?.message, currentSubmission.automaticChecking, expectedOutput)
					&& this.renderShowOutputButton()
					}
					{ this.renderShowStatisticsHint() }
				</ThemeContext.Provider>
				{ !hideSolutions
				&& (hints.length === showedHintsCount || submissions.length > 0 || slideProgress.isSkipped)
				&& this.renderShowAcceptedSolutionsButton()
				}
			</div>
		)
	}

	renderSubmitSolutionButton = () => {
		const { valueChanged, submissionLoading, } = this.state;

		return (
			<span className={ styles.exerciseControls }>
				<Tooltip pos={ "bottom center" } trigger={ "hover&focus" }
						 render={ () => valueChanged ? null : <span>{ texts.controls.submitCode.hint }</span> }>
							<Button
								loading={ submissionLoading }
								use={ "primary" }
								disabled={ !valueChanged }
								onClick={ this.sendExercise }>
								{ texts.controls.submitCode.text }
							</Button>
				</Tooltip>
			</span>
		);
	}

	renderShowHintButton = () => {
		const { showedHintsCount, showControlsText, } = this.state;
		const { hints, } = this.props;
		const noHintsLeft = showedHintsCount === hints.length;
		const hintClassName = classNames(styles.exerciseControls, { [styles.noHintsLeft]: noHintsLeft });

		return (
			<span className={ hintClassName } onClick={ this.showHint }>
				<Tooltip pos={ "bottom center" } trigger={ "hover&focus" }
						 render={ () => noHintsLeft ? <span>{ texts.controls.hints.hint }</span> : null }>
					<span className={ styles.exerciseControlsIcon }>
						<Lightbulb/>
					</span>
					{ showControlsText && texts.controls.hints.text }
				</Tooltip>
			</span>
		);
	}

	renderResetButton = () => {
		const { showControlsText, } = this.state;

		return (
			<span className={ styles.exerciseControls } onClick={ this.resetCode }>
				<span className={ styles.exerciseControlsIcon }>
					<Refresh/>
				</span>
				{ showControlsText && texts.controls.reset.text }
			</span>
		);
	}

	renderShowOutputButton = () => {
		const { showOutput, showControlsText, } = this.state;

		return (
			<span className={ styles.exerciseControls } onClick={ this.toggleOutput }>
				<span className={ styles.exerciseControlsIcon }>
					<DocumentLite/>
				</span>
				{ showControlsText && (showOutput ? texts.controls.output.hide : texts.controls.output.show) }
			</span>
		)
	}

	renderShowAcceptedSolutionsButton = () => {
		const { showAcceptedSolutionsWarning, showControlsText, } = this.state;

		return (
			<span className={ styles.exerciseControls } onClick={ this.showAcceptedSolutionsWarning }>
					<Tooltip
						onCloseClick={ this.hideAcceptedSolutionsWarning }
						pos={ "bottom left" }
						trigger={ showAcceptedSolutionsWarning ? "opened" : "closed" }
						render={
							() =>
								<span>
									{ texts.controls.acceptedSolutions.buildWarning() }
									<Button use={ "danger" } onClick={ this.showAcceptedSolutions }>
										{ texts.controls.acceptedSolutions.continue }
									</Button>
								</span>
						}>
						<span className={ styles.exerciseControlsIcon }>
							<EyeOpened/>
						</span>
						{ showControlsText && texts.controls.acceptedSolutions.text }
					</Tooltip>
				</span>
		);
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

	renderAcceptedSolutions = () => {
		const { slideId, courseId, } = this.props;

		return (
			<Modal onClose={ this.closeAcceptedSolutions }>
				<Modal.Header>{ texts.acceptedSolutions.title }</Modal.Header>
				<Modal.Body>
					{ texts.acceptedSolutions.content }
					<DownloadedHtmlContent url={ constructPathToAcceptedSolutions(courseId, slideId) }/>
				</Modal.Body>
			</Modal>
		)
	}

	renderCongratsModal = ({ score, waitingForManualChecking, }) => {
		const { hideSolutions, } = this.props;
		const { showAcceptedSolutions, } = this.state;

		return (
			<CongratsModal
				showAcceptedSolutions={ !waitingForManualChecking && !hideSolutions && showAcceptedSolutions }
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
		const { attemptedUsersCount, usersWithRightAnswerCount, lastSuccessAttemptDate, } = this.props.attemptsStatistics;
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

	resetCode = () => {
		const { exerciseInitialCode } = this.props;

		this.clearAllTextMarkers();
		this.setState({
			value: exerciseInitialCode,
			valueChanged: true,
			isEditable: true,
			currentSubmission: null,
			visibleCheckingResponse: null,
			currentReviews: [],
			showOutput: false
		})
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

	showAcceptedSolutionsWarning = () => {
		const { slideProgress, submissions, } = this.props;

		if(submissions.length > 0 || slideProgress.isSkipped) {
			this.showAcceptedSolutions();
		} else {
			this.setState({
				showAcceptedSolutionsWarning: true,
			});
		}
	}

	hideAcceptedSolutionsWarning = () => {
		this.setState({
			showAcceptedSolutionsWarning: false,
		})
	}

	showAcceptedSolutions = (e) => {
		this.setState({
			showAcceptedSolutions: true,
		})

		if(e) {
			e.stopPropagation();
		}

		this.hideAcceptedSolutionsWarning();
	}

	closeAcceptedSolutions = () => {
		this.setState({
			showAcceptedSolutions: false,
		})
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

			default:
				require('codemirror/mode/xml/xml');
				return 'text/html';
		}
	}

	saveCodeToCache = (slideId) => {
		const { value, } = this.state;

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
});

const exerciseBlockProps = {
	languages: PropTypes.array.isRequired,
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
