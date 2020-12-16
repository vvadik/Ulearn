import React from "react";

import classNames from "classnames";
import { isMobile, isTablet, } from "src/utils/getDeviceType.js";

import ShowAfterDelay from "src/components/ShowAfterDelay/ShowAfterDelay";
import DownloadedHtmlContent from "src/components/common/DownloadedHtmlContent.js";
import { Button, ThemeContext, Tooltip, Modal } from "ui";
import { DocumentLite, EyeOpened, Lightbulb, Refresh } from "icons";
import { darkTheme } from 'ui/internal/ThemePlayground/darkTheme';

import styles from './Controls.less';

import texts from "../Exercise.texts";

interface Props {
	countOfHints: number,
	hideSolutions: boolean,
	isShowAcceptedSolutionsAvailable: boolean,
	isEditable: boolean,
	valueChanged: boolean,
	submissionLoading: boolean,
	hasOutput: boolean,
	showOutput: boolean,
	attemptsStatistics: {
		attemptedUsersCount: number,
		usersWithRightAnswerCount: number,
		lastSuccessAttemptDate?: string,
	},
	acceptedSolutionsUrl: string,
	showedHintsCount: number,

	onsSendExerciseButtonClicked: () => void,
	onResetButtonClicked: () => void,
	onShowOutputButtonClicked: () => void,
	onVisitAcceptedSolutions: () => void,
	showHint: () => void,
}

interface State {
	resizeTimeout?: NodeJS.Timeout,
	showControlsText: boolean,
	showAcceptedSolutionsWarning: boolean,
	showAcceptedSolutions: boolean,
}

const isControlsTextSuits = (): boolean => !isMobile() && !isTablet();


class Controls extends React.Component<Props, State> {
	state = {
		resizeTimeout: undefined,
		showControlsText: isControlsTextSuits(),
		showAcceptedSolutionsWarning: false,
		showAcceptedSolutions: false,
	};

	componentDidMount = (): void => {
		window.addEventListener("resize", this.onWindowResize);
	};

	componentWillUnmount = (): void => {
		window.removeEventListener("resize", this.onWindowResize);
	};

	onWindowResize = (): void => {
		const { resizeTimeout, } = this.state;

		const throttleTimeout = 66;

		//resize event can be called rapidly, to prevent performance issue, we throttling event handler
		if(!resizeTimeout) {
			this.setState({
				resizeTimeout: setTimeout(() => {
					this.setState({
						resizeTimeout: undefined,
						showControlsText: isControlsTextSuits(),
					});
				}, throttleTimeout),
			});
		}
	};

	render = (): React.ReactNode => {
		const {
			countOfHints, hideSolutions, isShowAcceptedSolutionsAvailable,
			isEditable, hasOutput, showedHintsCount,
		} = this.props;
		const { showAcceptedSolutions, } = this.state;

		return (
			<div className={ styles.exerciseControlsContainer }>
				{ this.renderSubmitSolutionButton() }
				<ThemeContext.Provider value={ darkTheme }>
					{ countOfHints > 0 && this.renderShowHintButton() }
					{ isEditable && this.renderResetButton() }
					{ !isEditable && hasOutput && this.renderShowOutputButton() }
					{ this.renderShowStatisticsHint() }
				</ThemeContext.Provider>
				{ !hideSolutions
				&& (countOfHints === showedHintsCount || isShowAcceptedSolutionsAvailable)
				&& this.renderShowAcceptedSolutionsButton()
				}
				{ showAcceptedSolutions && this.renderAcceptedSolutions() }
			</div>
		);
	};

	renderSubmitSolutionButton = (): React.ReactNode => {
		const { valueChanged, submissionLoading, onsSendExerciseButtonClicked, } = this.props;

		return (
			<span className={ styles.exerciseControls }>
				<Tooltip pos={ "bottom center" } trigger={ "hover&focus" }
						 render={ this.renderSubmitCodeHint }>
							<Button
								loading={ submissionLoading }
								use={ "primary" }
								disabled={ !valueChanged }
								onClick={ onsSendExerciseButtonClicked }>
								{ texts.controls.submitCode.text }
							</Button>
				</Tooltip>
			</span>
		);
	};

	renderSubmitCodeHint = (): React.ReactNode => {
		const { valueChanged } = this.props;

		return valueChanged ? null : <span>{ texts.controls.submitCode.hint }</span>;
	};

	renderShowHintButton = (): React.ReactNode => {
		const { showControlsText, } = this.state;
		const { countOfHints, showHint, showedHintsCount, } = this.props;

		const noHintsLeft = showedHintsCount === countOfHints;
		const hintClassName = classNames(styles.exerciseControls, { [styles.noHintsLeft]: noHintsLeft });

		return (
			<span className={ hintClassName } onClick={ showHint }>
				<Tooltip pos={ "bottom center" } trigger={ "hover&focus" }
						 render={ () => noHintsLeft ? <span>{ texts.controls.hints.hint }</span> : null }>
					<span className={ styles.exerciseControlsIcon }>
						<Lightbulb/>
					</span>
					{ showControlsText && texts.controls.hints.text }
				</Tooltip>
			</span>
		);
	};

	renderResetButton = (): React.ReactNode => {
		const { showControlsText, } = this.state;
		const { onResetButtonClicked, } = this.props;

		return (
			<span className={ styles.exerciseControls } onClick={ onResetButtonClicked }>
				<span className={ styles.exerciseControlsIcon }>
					<Refresh/>
				</span>
				{ showControlsText && texts.controls.reset.text }
			</span>
		);
	};

	renderShowOutputButton = (): React.ReactNode => {
		const { showControlsText, } = this.state;
		const { showOutput, onShowOutputButtonClicked, } = this.props;

		return (
			<span className={ styles.exerciseControls } onClick={ onShowOutputButtonClicked }>
				<span className={ styles.exerciseControlsIcon }>
					<DocumentLite/>
				</span>
				{ showControlsText && (showOutput ? texts.controls.output.hide : texts.controls.output.show) }
			</span>
		);
	};

	renderShowStatisticsHint = (): React.ReactNode => {
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
								{ texts.controls.statistics.buildStatistics(attemptedUsersCount,
									usersWithRightAnswerCount, lastSuccessAttemptDate) }
							</span>
					}>
						{ texts.controls.statistics.buildShortText(usersWithRightAnswerCount) }
					</Tooltip>
				</span>
		);
	};

	renderShowAcceptedSolutionsButton = (): React.ReactNode => {
		const { showAcceptedSolutionsWarning, showControlsText, } = this.state;

		return (
			<span className={ styles.exerciseControls } onClick={ this.showAcceptedSolutionsWarning }>
					<Tooltip
						onCloseClick={ this.hideAcceptedSolutionsWarning }
						pos={ "bottom left" }
						trigger={ showAcceptedSolutionsWarning ? "opened" : "closed" }
						render={ this.renderAcceptedSolutionsHint }>
						<span className={ styles.exerciseControlsIcon }>
							<EyeOpened/>
						</span>
						{ showControlsText && texts.controls.acceptedSolutions.text }
					</Tooltip>
				</span>
		);
	};

	showAcceptedSolutionsWarning = (): void => {
		const { isShowAcceptedSolutionsAvailable } = this.props;

		if(isShowAcceptedSolutionsAvailable) {
			this.showAcceptedSolutions();
		} else {
			this.setState({
				showAcceptedSolutionsWarning: true,
			});
		}
	};

	renderAcceptedSolutionsHint = (): React.ReactNode => {
		return (
			<span>
				{ texts.controls.acceptedSolutions.buildWarning() }
				<Button use={ "danger" } onClick={ this.showAcceptedSolutions }>
					{ texts.controls.acceptedSolutions.continue }
				</Button>
			</span>);
	};

	hideAcceptedSolutionsWarning = (): void => {
		this.setState({
			showAcceptedSolutionsWarning: false,
		});
	};

	showAcceptedSolutions = (e?: React.MouseEvent<HTMLButtonElement, MouseEvent>): void => {
		const { onVisitAcceptedSolutions, } = this.props;

		onVisitAcceptedSolutions();

		this.setState({
			showAcceptedSolutions: true,
		});

		if(e) {
			e.stopPropagation();
		}

		this.hideAcceptedSolutionsWarning();
	};

	closeAcceptedSolutions = (): void => {
		this.setState({
			showAcceptedSolutions: false,
		});
	};

	renderAcceptedSolutions = (): React.ReactNode => {
		const { acceptedSolutionsUrl, } = this.props;

		return (
			<ShowAfterDelay>
				<Modal onClose={ this.closeAcceptedSolutions }>
					<Modal.Header>
						{ texts.acceptedSolutions.title }
					</Modal.Header>
					<Modal.Body>
						{ texts.acceptedSolutions.content }
						<DownloadedHtmlContent url={ acceptedSolutionsUrl }/>
					</Modal.Body>
				</Modal>
			</ShowAfterDelay>
		);
	};
}

export default Controls;
