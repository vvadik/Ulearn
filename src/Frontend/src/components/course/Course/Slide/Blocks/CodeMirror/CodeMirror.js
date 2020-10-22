import React from 'react';

import { UnControlled, Controlled, } from "react-codemirror2";
import { Button, Checkbox, Dropdown, MenuItem, Modal, Tooltip, } from "ui";
import DownloadedHtmlContent from "src/components/common/DownloadedHtmlContent";
import { Lightbulb, Refresh, EyeOpened, DocumentLite, Error, } from "icons";
import Avatar from "src/components/common/Avatar/Avatar";

import PropTypes from 'prop-types';
import classNames from 'classnames';
import { constructPathToAcceptedSolutions, } from "src/consts/routes";
import { getDateDDMMYY } from "src/utils/getMoment";
import getPluralForm from "src/utils/getPluralForm";
import { isMobile } from "src/utils/getDeviceType";
import moment from "moment";
import api from "src/api";

import 'codemirror/lib/codemirror';
import 'codemirror/addon/edit/matchbrackets';
import 'codemirror/theme/darcula.css';

import styles from './CodeMirror.less';


class CodeMirror extends React.Component {
	constructor(props) {
		super(props);
		const { exerciseInitialCode, submissions, code, } = props;
		const isExercise = !!submissions;

		this.state = {
			value: exerciseInitialCode || code,
			valueChanged: false,
			showedHintsCount: 0,
			showAcceptedSolutions: false,
			showAcceptedSolutionsWarning: false,
			isExercise,
			currentSubmission: null,
			isEditable: isExercise && submissions.length === 0,
			showedOutput: null,
			showedOutputError: false,
			reviewTextMarkers: [],
			selectedReviewIndex: -1,
			exerciseCodeDoc: null,
		};
	}

	componentDidMount() {
		const { slideId, submissions, } = this.props;
		const { isExercise, } = this.state;

		if(!isExercise) {
			return;
		}

		if(submissions.length > 0) {
			this.loadSubmissionToState(submissions[submissions.length - 1]);
		} else {
			this.refreshPreviousDraft(slideId);
		}

		window.addEventListener("beforeunload", this.saveCodeDraftToCache);
	}

	saveCodeDraftToCache = () => {
		const { slideId, } = this.props;

		this.saveExerciseCodeDraft(slideId);
	}

	componentWillUnmount() {
		this.saveCodeDraftToCache();
		window.removeEventListener("beforeunload", this.saveCodeDraftToCache);
	}

	render() {
		const { language, className, isAuthenticated, } = this.props;
		const { isExercise, isEditable, value, } = this.state;

		const opts = {
			mode: this.loadLanguageStyles(language),
			lineNumbers: true,
			scrollbarStyle: 'null',
			lineWrapping: true,
			theme: isEditable ? 'darcula' : 'default',
			readOnly: !isEditable || !isAuthenticated,
			matchBrackets: true,
			tabSize: 4,
			indentUnit: 4,
			indentWithTabs: true,
			/*			extraKeys: {
							ctrlSpace: "autocomplete",
							".": function (cm) {
								setTimeout(function () {
									cm.execCommand("autocomplete");
								}, 100);
								return CodeMirror.Pass;
							}
						},*/
		};

		return (
			<div className={ classNames(styles.wrapper, className) } onClick={ this.resetSelectedReviewTextMarker }>
				{ isExercise && isAuthenticated
					? this.renderControlledCodeMirror(opts)
					: <UnControlled
						editorDidMount={ this.onEditorMount }
						className={ styles.editor }
						options={ opts }
						value={ value }
					/>
				}
			</div>
		);
	}

	resetSelectedReviewTextMarker = () => {
		const { selectedReviewIndex, reviewTextMarkers, isEditable, } = this.state;

		if(!isEditable && selectedReviewIndex >= 0) {
			const newReviewTextMarkers = [...reviewTextMarkers];
			const reviewTextMarker = newReviewTextMarkers[selectedReviewIndex];

			const { from, to, } = reviewTextMarker.marker.find();
			reviewTextMarker.marker.clear();
			reviewTextMarker.marker = this.highlightLine(to.line, to.ch, from.line, from.ch, styles.reviewCode);

			this.setState({
				reviewTextMarkers: newReviewTextMarkers,
				selectedReviewIndex: -1,
			});
		}
	}

	renderControlledCodeMirror = (opts) => {
		const { value, showedHintsCount, showAcceptedSolutions, currentSubmission, isEditable, showedOutput, exerciseCodeDoc, } = this.state;
		const { submissions, } = this.props;

		const isReview = !isEditable && currentSubmission?.reviews.length > 0;

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
				{ submissions.length !== 0 && this.renderSubmissionsDropdown() }
				{ !isEditable && currentSubmission && this.renderSuccessHeader() }
				<div className={ wrapperClassName }>
					<Controlled
						onBeforeChange={ this.onBeforeChange }
						editorDidMount={ this.onEditorMount }
						onCursorActivity={ this.onCursorActivity }
						className={ editorClassName }
						options={ opts }
						value={ value }
					/>
					{ exerciseCodeDoc && isReview && this.renderReview() }
				</div>
				{ !isEditable && this.renderEditButton() }
				{ !isEditable && this.renderOverview() }
				{ this.renderControls() }
				{ showedOutput !== null && this.renderOutput() }
				{ showedHintsCount > 0 && this.renderHints() }
				{ showAcceptedSolutions && this.renderAcceptedSolutions() }
			</React.Fragment>
		)
	}

	renderSubmissionsDropdown = () => {
		const { submissions, } = this.props;
		const { currentSubmission, } = this.state;

		const submissionsWithoutCurrent = submissions.filter(({ id }) => !currentSubmission || id !== currentSubmission.id);

		if(currentSubmission) {
			submissionsWithoutCurrent.push({ isNew: true });
		}

		return (
			<div className={ styles.submissionsDropdown }>
				<Dropdown
					caption={ currentSubmission ? this.getSubmissionDate(currentSubmission.timestamp) : 'Новая попытка' }>
					{ submissionsWithoutCurrent.map(({ id, timestamp, isNew, }) =>
						isNew
							? <MenuItem
								title={ -1 }
								key={ -1 }
								onClick={ this.loadNewTry }>
								Новая попытка
							</MenuItem>
							: <MenuItem
								title={ id }
								key={ id }
								onClick={ this.loadSubmission }>
								{ this.getSubmissionDate(timestamp) }
							</MenuItem>) }
				</Dropdown>
			</div>
		)
	}

	renderSuccessHeader = () => {
		const { currentSubmission, } = this.state;

		if(currentSubmission?.reviews.length > 0) {
			return (
				<div className={ styles.reviewHeader }>
					Решение отправлено на ревью – 5 баллов. После ревью преподаватель поставит итоговый балл
				</div>
			);
		}
		return (
			<div className={ styles.header }>
				Все тесты пройдены, задача сдана
			</div>
		)
	}

	getSubmissionDate = (timestamp) => {
		return moment(timestamp).format('DD MMMM YYYY в HH:mm');
	}

	loadSubmission = (e) => {
		const { submissions } = this.props;
		const id = Number.parseInt(e.target.title);

		const submission = submissions.find(s => s.id === id);

		this.loadSubmissionToState(submission);
	}

	loadSubmissionToState = (submission) => {
		this.saveCodeDraftToCache();
		this.clearAllTextMarkers();

		this.setState({
			value: submission.code,
			isEditable: false,
			showedOutput: null,
		}, () => this.setState({
			currentSubmission: submission,
			reviewTextMarkers: this.getTextMarkers(submission.reviews),
		}));
	}

	getTextMarkers = (reviews) => {
		const textMarkers = [];

		for (const [i, { finishLine, finishPosition, startLine, startPosition }] of reviews.entries()) {
			const textMarker = this.highlightLine(finishLine, finishPosition, startLine, startPosition, styles.reviewCode);

			textMarkers.push({
				marker: textMarker,
				index: i,
			});
		}

		return textMarkers;
	}

	renderOverview = () => {
		const botCommentsLength = 1;
		const { currentSubmission } = this.state;

		const checkups = [
			{
				title: 'Самопроверка',
				content:
					<React.Fragment>
						<span className={ styles.overviewSelfCheckComment }>
							Посмотрите, всё ли вы учли и отметьте сделанное
						</span>
						<ul className={ styles.overviewSelfCheckList }>
							{ this.renderSelfCheckBoxes() }
						</ul>
					</React.Fragment>
			},
		];

		if(botCommentsLength !== 0) {
			checkups.unshift(
				{
					title: 'Ulearn Bot',
					content:
						<span className={ styles.overviewComment }>
						{ `Бот нашёл ${ botCommentsLength } ${ getPluralForm(botCommentsLength, 'ошибку', 'ошибки', 'ошибок') }. ` }
							<a onClick={ this.showFirstBotComment }>Посмотреть</a>
					</span>
				});
		}

		if(currentSubmission && currentSubmission.reviews && currentSubmission.reviews.length !== 0) {
			const reviewsCount = currentSubmission.reviews.length;

			checkups.unshift({
				title: 'Код-ревью',
				content:
					<span className={ styles.overviewComment }>
						{ `Преподаватель оставил ${ reviewsCount } ${ getPluralForm(reviewsCount, 'комментарий', 'комментария', 'комментариев') }. ` }
						<a onClick={ this.showFirstComment }>Посмотреть</a>
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

	renderSelfCheckBoxes = () => {
		const selfCheckups = [
			'Проверьте оформление',
			'Проверьте, у всех полей и методов правильно выбраны модификаторы доступа.',
			'Метод точно работает корректно?',
		];

		return (
			selfCheckups.map((ch, i) =>
				<li key={ i }>
					<Checkbox/> <span className={ styles.selfCheckText }>{ ch }</span>
				</li>
			)
		);
	}

	renderControls = () => {
		const { hints, } = this.props;
		const { isEditable, currentSubmission, } = this.state;

		return (
			<div className={ styles.exerciseControlsContainer }>
				{ this.renderSubmitSolutionButton() }
				{ hints.length > 0 && this.renderShowHintButton() }
				{ isEditable && this.renderResetButton() }
				{ !isEditable && currentSubmission && currentSubmission.output && this.renderShowOutputButton() }
				{ this.renderShowAcceptedSolutionsButton() }
				{ this.renderShowStatisticsHint() }
			</div>
		)
	}

	renderOutput = () => {
		const { showedOutputError, } = this.state;
		const wrapperClasses = showedOutputError ? styles.wrongOutput : styles.output;
		const headerClasses = showedOutputError ? styles.wrongOutputHeader : styles.outputHeader;

		return (
			<div className={ wrapperClasses }>
				<span className={ headerClasses }>
					{ showedOutputError
						? <React.Fragment><Error/>Неверный результат</React.Fragment>
						: 'Вывод программы' }
				</span>
				{ this.renderOutputLines() }
			</div>
		);
	}

	renderOutputLines = () => {
		const { showedOutput, showedOutputError, } = this.state;
		const lines = Array.isArray(showedOutput)
			? showedOutput
			: [showedOutput];

		if(showedOutputError) {
			return (
				<table className={ styles.outputTable }>
					<thead>
					<tr>
						<th/>
						<th>Вывод вашей программы</th>
						<th>Ожидаемый вывод</th>
					</tr>
					</thead>
					<tbody>
					<tr className={ styles.lineWithError }>
						<td>1</td>
						<td>{ showedOutput.actualOutput }</td>
						<td>{ showedOutput.expectedOutput }</td>
					</tr>
					</tbody>
				</table>
			);
		}

		return lines.map((l, i) =>
			<p key={ i } className={ styles.oneLineOutput }>
				{ l }
			</p>);
	}

	renderSubmitSolutionButton = () => {
		const { valueChanged, } = this.state;

		return (
			<span className={ styles.exerciseControls }>
				<Tooltip pos={ "bottom center" } trigger={ "hover&focus" }
						 render={ () => valueChanged ? null : <span>Начните писать код</span> }>
							<Button
								use={ "primary" }
								disabled={ !valueChanged }
								onClick={ this.sendExercise }>
								Отправить
							</Button>
				</Tooltip>
			</span>
		);
	}

	renderShowHintButton = () => {
		const { showedHintsCount, } = this.state;
		const { hints, } = this.props;
		const noHintsLeft = showedHintsCount === hints.length;
		const hintClassName = classNames(styles.exerciseControls, { [styles.noHintsLeft]: noHintsLeft });

		return (
			<span className={ hintClassName } onClick={ this.showHint }>
				<Tooltip pos={ "bottom center" } trigger={ "hover&focus" }
						 render={ () => noHintsLeft ? <span>Подсказки закончились</span> : null }>
							<Lightbulb/>{ !isMobile() && `Взять подсказку` }
				</Tooltip>
			</span>
		);
	}

	renderResetButton = () => {
		return (
			<span className={ styles.exerciseControls } onClick={ this.resetCode }>
					<Refresh/>{ !isMobile() && `Начать сначала` }
			</span>
		);
	}

	renderShowOutputButton = () => {
		const { showedOutput } = this.state;

		return (
			<span className={ styles.exerciseControls } onClick={ this.toggleOutput }>
					<DocumentLite/> { showedOutput ? 'Скрыть вывод' : 'Показать вывод' }
			</span>
		)
	}

	renderShowAcceptedSolutionsButton = () => {
		const { showAcceptedSolutionsWarning } = this.state;

		return (
			<span className={ styles.exerciseControls } onClick={ this.showAcceptedSolutionsWarning }>
					<Tooltip
						onCloseClick={ this.hideAcceptedSolutionsWarning }
						pos={ "bottom left" }
						trigger={ showAcceptedSolutionsWarning ? "opened" : "closed" }
						render={
							() =>
								<span>
									Вы не получите баллы за задачу,<br/>
									если посмотрите чужие решения.<br/>
									<br/>
									<Button use={ "danger" } onClick={ this.showAcceptedSolutions }>
										Всё равно посмотреть
									</Button>
								</span>
						}>
					<EyeOpened/>{ !isMobile() && `Посмотреть решения` }
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
				<Modal.Header>Решения</Modal.Header>
				<Modal.Body>
					<p>Изучите решения ваших коллег. Проголосуйте за решения, в которых вы нашли что-то новое для
						себя.</p>
					<DownloadedHtmlContent url={ constructPathToAcceptedSolutions(courseId, slideId) }/>
				</Modal.Body>
			</Modal>
		)
	}

	renderReview = () => {
		return (
			<div className={ styles.reviewsContainer }>
				{ this.renderComments() }
			</div>
		)
	}

	renderComments = () => {
		const { currentSubmission, } = this.state;
		const comments = [];

		for (const [i, review] of currentSubmission.reviews.entries()) {
			const comment = this.renderComment(review, i);
			comments.push(comment);
		}

		return comments;
	}

	renderComment = ({ addingTime, author, comment, finishLine, finishPosition, startLine, startPosition, }, i,) => {
		const { selectedReviewIndex, exerciseCodeDoc, } = this.state;
		const className = classNames(styles.comment, { [styles.selectedReviewCommentWrapper]: selectedReviewIndex === i });

		//const minHeight = exerciseCodeDoc.cm.charCoords({ line: startLine, ch: startPosition }, 'local').top;
		//const offset = Math.max(5, minHeight,);
//TODO style={ { marginTop: `${ offset }px` } }
		return (
			<div key={ i }  className={ className }
				 onClick={ (e) => this.selectComment(e, i) }>
				<div className={ styles.authorWrapper }>
					<Avatar user={ author } size="big" className={ styles.commentAvatar }/>
					<div className={ styles.authorCredentialsWrapper }>
						{ `${ author.firstName } ${ author.lastName }` }
						<span className={ styles.commentLine }>{ `строка ${ startLine + 1 }` }</span>
						{ addingTime && <p className={ styles.addingTime }>{ getDateDDMMYY(addingTime) }</p> }
					</div>
				</div>
				<p className={ styles.commentText }>{ comment }</p>
			</div>
		);
	}

	showFirstComment = () => {
		//TODO
	}

	showFirstBotComment = () => {
		//TODO
	}

	selectComment = (e, i) => {
		const { isEditable } = this.state;
		e.stopPropagation();

		if(!isEditable) {
			this.highlightReview(i);
		}
	}

	highlightReview = (index) => {
		const { reviewTextMarkers, } = this.state;
		const newReviewTextMarkers = [...reviewTextMarkers];
		const reviewTextMarker = newReviewTextMarkers[index];

		this.resetSelectedReviewTextMarker();

		const { from, to, } = reviewTextMarker.marker.find();
		reviewTextMarker.marker.clear();
		reviewTextMarker.marker = this.highlightLine(to.line, to.ch, from.line, from.ch, styles.selectedReviewCode)

		this.setState({
			reviewTextMarkers: newReviewTextMarkers,
			selectedReviewIndex: index,
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

	renderEditButton = () => {
		return (
			<div className={ styles.editButton } onClick={ this.enableEditing }>
				Редактировать
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
								За всё время:<br/>
								{ attemptedUsersCount } студентов пробовали решить<br/>
								задачу, решили – { usersWithRightAnswerCount } <br/>
								<br/>
								Последний раз решили { moment(lastSuccessAttemptDate).startOf("minute").fromNow() }
							</span>
					}>
						Решило: { usersWithRightAnswerCount }
					</Tooltip>
				</span>
		);
	}

	enableEditing = (e) => {
		e.stopPropagation();

		this.clearAllTextMarkers();

		this.setState({
			isEditable: true,
			showedOutput: null,
			valueChanged: true,
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
			valueChanged: false,
			isEditable: true,
			currentSubmission: null,
			showedOutput: null,
		})
	}

	clearAllTextMarkers = () => {
		const { reviewTextMarkers, } = this.state;

		reviewTextMarkers.forEach(({ marker }) => marker.clear());

		this.setState({
			reviewTextMarkers: [],
			selectedReviewIndex: -1,
		});
	}

	loadNewTry = () => {
		const { slideId } = this.props;

		this.resetCode();
		this.refreshPreviousDraft(slideId);
	}

	toggleOutput = () => {
		const { currentSubmission, showedOutput, } = this.state;

		this.setState({
			showedOutput: showedOutput ? null : currentSubmission.output.split('\n'),
		})
	}

	showAcceptedSolutionsWarning = () => {
		this.setState({
			showAcceptedSolutionsWarning: true,
		})
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
		e.stopPropagation()
		this.hideAcceptedSolutionsWarning();
	}

	closeAcceptedSolutions = () => {
		this.setState({
			showAcceptedSolutions: false,
		})
	}

	sendExercise = () => {
		const { value, } = this.state;
		const { courseId, slideId, } = this.props;

		api.post(`slides/${ courseId }/${ slideId }/exercise/submit`,
			api.createRequestParams({ solution: value }))
			.then(r => {
				/*
				actualOutput: null
				executionServiceName: "ulearn"
				expectedOutput: null
				ignored: false
				isCompileError: false
				isInternalServerError: true
				isRightAnswer: false
				message: "К сожалению, из-за большой нагрузки мы не смогли оперативно проверить ваше решение. Мы попробуем проверить его позже, просто подождите и обновите страницу. "
				sentToReview: false
				styleMessages: null
				submissionId: 0
				 */
				if(r.isRightAnswer) {
				} else {
					this.setState({
						showedOutputError: true,
						showedOutput: r,
					})
				}
			});
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
		})
	}

	onCursorActivity = () => {
		const { currentSubmission, exerciseCodeDoc, isEditable, } = this.state;
		const cursor = exerciseCodeDoc.getCursor();
		const { line, ch } = cursor;

		if(!isEditable && currentSubmission) {
			const reviewIndex = currentSubmission.reviews.findIndex(r =>
				r.startLine <= line && r.startPosition <= ch
				&& r.finishLine >= line && r.finishPosition >= ch
			);
			if(reviewIndex >= 0) {
				this.highlightReview(reviewIndex);
			}
		}
	}

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

			default:
				require('codemirror/mode/xml/xml');
				return 'text/html';
		}
	}

	saveExerciseCodeDraft = (id) => {
		const { value, refreshPreviousDraftLastId, } = this.state;

		if(id === undefined) {
			id = refreshPreviousDraftLastId;
		}

		const solutions = JSON.parse(localStorage['exercise_solutions'] || '{}');
		solutions[id] = value;

		localStorage['exercise_solutions'] = JSON.stringify(solutions);
	}

	refreshPreviousDraft = (id) => {
		const { refreshPreviousDraftLastId, } = this.state;
		if(id === undefined) {
			id = refreshPreviousDraftLastId;
		}

		this.setState({
			refreshPreviousDraftLastId: id,
		})

		const solutions = JSON.parse(localStorage['exercise_solutions'] || '{}');

		if(solutions[id] !== undefined) {
			this.resetCode();
			this.setState({
				value: solutions[id],
			})
		}
	}
}

CodeMirror.propTypes = {
	courseId: PropTypes.string,
	slideId: PropTypes.string,
	className: PropTypes.string,
	language: PropTypes.string.isRequired,
	code: PropTypes.string,
	isAuthenticated: PropTypes.bool,
}

export default CodeMirror;