import React from "react";

import texts from "./ExerciseFormHeader.texts";
import styles from "./ExerciseFormHeader.less";
import {
	AutomaticExerciseCheckingProcessStatus as ProcessStatus,
	AutomaticExerciseCheckingResult as CheckingResult,
	SolutionRunStatus,
	SubmissionInfo
} from "src/models/exercise";
import getPluralForm from "src/utils/getPluralForm.js";

interface ExerciseFormHeaderProps {
	solutionRunStatus: SolutionRunStatus | null, // Если результаты посылки, сделанной только что и не содержащей submission
	selectedSubmission: SubmissionInfo | null, // Если результаты выбранной посылки
	waitingForManualChecking?: boolean, // True, если именно это решение ожидает ревью
	prohibitFurtherManualChecking?: boolean, // True, если ревью по задаче включено, но запрещено для задачи этого студента преподавателем
	selectedSubmissionIsLast?: boolean,
	score?: number
}

type StyleAndText = { style: string, text: string } | null;

class ExerciseFormHeader extends React.Component<ExerciseFormHeaderProps> {
	render(): React.ReactNode {
		const styleAndText = this.getStyleAndText();
		if(!styleAndText) {
			return null;
		}
		const { style, text } = styleAndText;
		return (
			<div className={ style }>
				{ text }
			</div>
		);
	}

	getStyleAndText(): StyleAndText {
		const { solutionRunStatus, selectedSubmission } = this.props;
		if(solutionRunStatus && solutionRunStatus !== SolutionRunStatus.Success) {
			return this.getStyleAndTextForCheckingResponse(solutionRunStatus);
		}
		if(selectedSubmission) {
			return this.getStyleAndTextForSelectedSubmission(selectedSubmission);
		}
		return null;
	}

	getStyleAndTextForSelectedSubmission(selectedSubmission: SubmissionInfo): StyleAndText {
		const { waitingForManualChecking, prohibitFurtherManualChecking, selectedSubmissionIsLast, score } = this.props;
		const { automaticChecking, manualCheckingPassed, } = selectedSubmission;
		if(automaticChecking) {
			switch (automaticChecking.processStatus) {
				case ProcessStatus.Done:
					switch (automaticChecking.result) {
						case CheckingResult.RightAnswer:
							return ExerciseFormHeader.getStyleAndTextAllTestPassedWithScore(waitingForManualChecking,
								prohibitFurtherManualChecking, manualCheckingPassed, !!selectedSubmissionIsLast, score);
						case CheckingResult.CompilationError:
							return { style: styles.errorHeader, text: texts.compilationError };
						case CheckingResult.WrongAnswer:
							return { style: styles.errorHeader, text: texts.wrongAnswer };
						case CheckingResult.NotChecked:
						default:
							console.error(new Error(`checkingResult has unknown value ${ automaticChecking.result }`));
							return null;
					}
				case ProcessStatus.Running:
					return { style: styles.header, text: texts.running };
				case ProcessStatus.Waiting:
					return { style: styles.header, text: texts.waiting };
				case ProcessStatus.ServerError:
					return { style: styles.header, text: texts.serverError };
				case ProcessStatus.WaitingTimeLimitExceeded:
					return { style: styles.header, text: texts.waitingTimeLimitExceeded };
				default:
					console.error(new Error(`processStatus has unknown value ${ automaticChecking.result }`));
					return null;
			}
		} else {
			return ExerciseFormHeader.getStyleAndTextNoTests(waitingForManualChecking,
				prohibitFurtherManualChecking, selectedSubmission.manualCheckingPassed);
		}
	}

	getStyleAndTextForCheckingResponse(solutionRunStatus: SolutionRunStatus): StyleAndText {
		switch (solutionRunStatus) {
			case SolutionRunStatus.Ignored:
				return { style: styles.header, text: texts.serverMessage };
			case SolutionRunStatus.InternalServerError:
				return { style: styles.header, text: texts.serverError };
			case SolutionRunStatus.SubmissionCheckingTimeout:
				return { style: styles.header, text: texts.serverMessage };
			case SolutionRunStatus.CompilationError:
				return { style: styles.errorHeader, text: texts.compilationError };
			case SolutionRunStatus.Success:
				console.error(new Error(`solutionRunStatus cant be Success in getStyleAndTextForCheckingResponse`));
				return null;
			default:
				console.error(new Error(`solutionRunStatus has unknown value ${ solutionRunStatus }`));
				return null;
		}
	}

	static getStyleAndTextAllTestPassedWithScore(waitingForManualChecking: boolean | undefined,
		prohibitFurtherManualChecking: boolean | undefined,
		manualCheckingPassed: boolean, isLastSubmission: boolean, score?: number | null
	): StyleAndText {
		const styleAndText = ExerciseFormHeader.getStyleAndTextAllTestPassed(waitingForManualChecking,
			prohibitFurtherManualChecking, manualCheckingPassed, isLastSubmission);
		if(!styleAndText) {
			return styleAndText;
		}
		if(isLastSubmission && score) {
			const plural = getPluralForm(score, 'балл', 'балла', 'баллов')
			styleAndText.text = `${ score } ${ plural }. ${ styleAndText.text }`;
		}
		return styleAndText;
	}

	static getStyleAndTextAllTestPassed(waitingForManualChecking: boolean | undefined,
		prohibitFurtherManualChecking: boolean | undefined,
		manualCheckingPassed: boolean, isLastSubmission: boolean
	): StyleAndText {
		if(manualCheckingPassed) {
			return { style: styles.successHeader, text: texts.allTestPassedWasReviewed };
		}
		if(waitingForManualChecking) {
			return { style: styles.successHeader, text: texts.allTestPassedPendingReview };
		}
		if(prohibitFurtherManualChecking && isLastSubmission) {
			return { style: styles.successHeader, text: texts.allTestPassedProhibitFurtherReview };
		}
		if(isLastSubmission) {
			return { style: styles.successHeader, text: texts.allTestPassedNoReview };
		}
		return { style: styles.successHeader, text: texts.allTestPassed };
	}

	static getStyleAndTextNoTests(waitingForManualChecking: boolean | undefined,
		prohibitFurtherManualChecking: boolean | undefined,
		manualCheckingPassed: boolean
	)
		: StyleAndText {
		if(manualCheckingPassed) {
			return { style: styles.successHeader, text: texts.noTestsWasReviewed };
		}
		if(waitingForManualChecking) {
			return { style: styles.header, text: texts.noTestsPendingReview };
		}
		if(prohibitFurtherManualChecking) {
			return { style: styles.header, text: texts.noTestsProhibitFurtherReview };
		}
		return null;
	}
}

export { ExerciseFormHeader, ExerciseFormHeaderProps }
