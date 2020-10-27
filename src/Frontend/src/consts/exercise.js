export const processStatuses = {
	SERVER_ERROR: 'servererror', // Ошибка на сервере или статус SandoxError в чеккере
	DONE: 'done', // Проверена
	WAITING: 'waiting', // В очереди на проверку
	RUNNING: 'running', // Проверяется
	WAITING_TIME_LIMIT_EXCEEDED: 'waitingtimelimitexceeded', // Задача выкинута из очереди, потому что её никто не взял на проверку
};

export const checkingResults = {
	NOT_CHECKED: 'notchecked',
	COMPILATION_ERROR: 'compilationerror',
	RIGHT_ANSWER: 'rightanswer',
	WRONG_ANSWER: 'wronganswer',
};
