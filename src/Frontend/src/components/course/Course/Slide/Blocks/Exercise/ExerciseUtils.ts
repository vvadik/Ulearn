import {
	AutomaticExerciseCheckingResult as CheckingResult,
	SolutionRunStatus, SubmissionInfo
} from "src/models/exercise";

enum SubmissionColor {
	MaxResult = "MaxResult", // Студенту больше ничего не может сделать, ни сийчам ни в будущем
	NeedImprovements = "NeedImprovements", // Студент может доработать задачу сейчас или в будущем
	WrongAnswer = "WrongAnswer", // Тесты не пройдены или ошибка компиляции, показывается даже в старых версиях
	Message = "Message", // Сообщение, ни на что не влияющее, например, старая версия
}

function GetSubmissionColor(
	solutionRunStatus: SolutionRunStatus | null,
	checkingResult: CheckingResult | null, // null если automaticChecking null
	hasSuccessSolution: boolean, // Задача прошла автопроверку или автопроверки нет?
	selectedSubmissionIsLast: boolean, // Это последнее решение, прошедшее тесты?
	selectedSubmissionIsLastSuccess: boolean, // Это последнее решение, прошедшее тесты?
	waitingForManualChecking: boolean, // Студент в целом ожидает ревью?
	isSkipped: boolean,
): SubmissionColor {
	if(solutionRunStatus === SolutionRunStatus.CompilationError
		|| checkingResult === CheckingResult.CompilationError || checkingResult === CheckingResult.WrongAnswer) {
		return SubmissionColor.WrongAnswer;
	}
	if(selectedSubmissionIsLastSuccess) {
		return waitingForManualChecking ? SubmissionColor.NeedImprovements : SubmissionColor.MaxResult;
	}
	return selectedSubmissionIsLast && !isSkipped && (!hasSuccessSolution || waitingForManualChecking)
		? SubmissionColor.NeedImprovements
		: SubmissionColor.Message;
}

function IsSuccessSubmission(submission: SubmissionInfo | null): boolean {
	return !!submission && (submission.automaticChecking == null || submission.automaticChecking.result === CheckingResult.RightAnswer);
}

function HasSuccessSubmission(submissions: SubmissionInfo[]): boolean {
	return submissions.some(IsSuccessSubmission);
}

function SelectedSubmissionIsLast(submissions: SubmissionInfo[], currentSubmission: SubmissionInfo | null): boolean {
	return submissions.length > 0 && submissions[0] === currentSubmission
}

function SelectedSubmissionIsLastSuccess(submissions: SubmissionInfo[], currentSubmission: SubmissionInfo | null
): boolean {
	const successSubmissions = submissions.filter(IsSuccessSubmission);
	return successSubmissions.length > 0 && successSubmissions[0] === currentSubmission;
}

function IsFirstRightAnswer(submissions: SubmissionInfo[], successSubmission: SubmissionInfo): boolean  {
	const successSubmissions = submissions.filter(IsSuccessSubmission);
	return successSubmissions.length > 0 && successSubmissions[successSubmissions.length - 1] === successSubmission;
}

export {
	GetSubmissionColor,
	SubmissionColor,
	HasSuccessSubmission,
	SelectedSubmissionIsLast,
	SelectedSubmissionIsLastSuccess,
	IsFirstRightAnswer
}
