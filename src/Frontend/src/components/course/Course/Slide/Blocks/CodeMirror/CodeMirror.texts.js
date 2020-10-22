import React from "react";

import { Link } from "react-router-dom";

import getPluralForm from "src/utils/getPluralForm";
import { accountPath } from "src/consts/routes";

import moment from "moment";

const texts = {
	submissions: {
		newTry: 'Новая попытка',
		getSubmissionCaption: (submission) => submission ? texts.getSubmissionDate(submission.timestamp) : texts.submissions.newTry,
	},

	headers: {
		allTestPassedHeader: 'Все тесты пройдены, задача сдана',
	},

	acceptedSolutions: {
		title: 'Решения',
		content: <p>Изучите решения ваших коллег. Проголосуйте за решения, в которых вы нашли что-то новое для
			себя.</p>,

	},

	mocks: {
		headerSuccess: 'Решение отправлено на ревью – 5 баллов. После ревью преподаватель поставит итоговый балл',
		wrongResult: 'Неверный результат',
	},

	output: {
		text: 'Вывод программы',

		userOutput: 'Вывод вашей программы',
		expectedOutput: 'Ожидаемый вывод',
	},

	checkups: {
		showReview: 'Посмотреть',

		self: {
			title: 'Самопроверка',
			text: 'Посмотрите, всё ли вы учли и отметьте сделанное',

			checks: [
				'Проверьте оформление',
				'Проверьте, у всех полей и методов правильно выбраны модификаторы доступа.',
				'Метод точно работает корректно?',
			],
		},
		teacher: {
			title: 'Код-ревью',
			countTeacherReviews: (reviewsCount) => `Преподаватель оставил ${ reviewsCount } ${ getPluralForm(reviewsCount, 'комментарий', 'комментария', 'комментариев') }. `,
		},
		bot: {
			title: 'Ulearn Bot',
			countBotComments: (botCommentsLength) => `Бот нашёл ${ botCommentsLength } ${ getPluralForm(botCommentsLength, 'ошибку', 'ошибки', 'ошибок') }. `,
		},
	},

	controls: {
		submitCode: {
			text: 'Отправить',
			hint: 'Начните писать код',
		},
		hints: {
			text: 'Взять подсказку',
			hint: 'Подсказки закончились',
		},

		reset: {
			text: 'Начать сначала',
		},

		output: {
			show: 'Показать вывод',
			hide: 'Скрыть вывод',
		},

		acceptedSolutions: {
			text: 'Посмотреть решения',
			buildWarning: () => <React.Fragment>
				Вы не получите баллы за задачу,<br/>
				если посмотрите чужие решения.<br/>
				<br/>
			</React.Fragment>,
			continue: 'Всё равно посмотреть',
		},

		edit: {
			text: 'Редактировать',
		},

		statistics: {
			buildShortText: (usersWithRightAnswerCount) =>
				<React.Fragment>Решило: { usersWithRightAnswerCount }</React.Fragment>,
			buildStatistics: (attemptedUsersCount, usersWithRightAnswerCount, lastSuccessAttemptDate) =>
				<React.Fragment>
					За всё время:<br/>
					{ attemptedUsersCount } студентов пробовали решить<br/>
					задачу, решили – { usersWithRightAnswerCount } <br/>
					<br/>
					Последний раз решили { moment(lastSuccessAttemptDate).startOf("minute").fromNow() }
				</React.Fragment>,
		},
	},

	getSubmissionDate: (timestamp) => {
		return moment(timestamp).format('DD MMMM YYYY в HH:mm');
	}
};

export default texts;