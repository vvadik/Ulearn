export const processStatuses = {
	SERVER_ERROR: 'serverError', // Ошибка на сервере или статус SandoxError в чеккере
	DONE: 'done', // Проверена
	WAITING: 'waiting', // В очереди на проверку
	RUNNING: 'running', // Проверяется
	WAITING_TIME_LIMIT_EXCEEDED: 'waitingTimeLimitExceeded', // Задача выкинута из очереди, потому что её никто не взял на проверку
};

export const checkingResults = {
	NOT_CHECKED: 'notChecked',
	COMPILATION_ERROR: 'compilationError',
	RIGHT_ANSWER: 'rightAnswer',
	WRONG_ANSWER: 'wrongAnswer',
};
