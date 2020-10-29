export const processStatuses = {
	serverError: 'ServerError', // Ошибка на сервере или статус SandoxError в чеккере
	done: 'Done', // Проверена
	waiting: 'Waiting', // В очереди на проверку
	running: 'Running', // Проверяется
	waitingTimeLimitExceeded: 'WaitingTimeLimitExceeded', // Задача выкинута из очереди, потому что её никто не взял на проверку
};

export const checkingResults = {
	notChecked: 'NotChecked',
	compilationError: 'CompilationError',
	rightAnswer: 'RightAnswer',
	wrongAnswer: 'WrongAnswer',
};

export const solutionRunStatuses = {
	success: 'Success',
	internalServerError: 'InternalServerError', // Ошибка в проверяющей системе, подробности могут быть в Message. Если submission создан, он лежит в Submission, иначе null.
	ignored: 'Ignored', // Сервер отказался обрабатывать решение, причина написана в Message. Cлишком частые запросы на проверку или слишком длинный код.
	submissionCheckingTimeout: 'SubmissionCheckingTimeout', // Ждали, но не дожадлись проверки
	// В случае ошибки компиляции Submission создается не всегда. Не создается для C#-задач. Тогда текст ошибки компиляции кладется в Message.
	// Если Submission создался, то об ошибках компиляции пишется внутри Submission -> AutomaticChecking.
	compileError: 'CompileError',
}
