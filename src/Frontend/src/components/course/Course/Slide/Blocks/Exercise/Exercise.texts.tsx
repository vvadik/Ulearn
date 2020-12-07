import React from "react";

import getPluralForm from "src/utils/getPluralForm.js";
import { SubmissionInfo } from "src/models/exercise";
import { convertDefaultTimezoneToLocal, getMoment } from "src/utils/momentUtils.js";
import { Language } from "src/consts/languages";
import { capitalize } from "src/utils/stringUtils";

const texts = {
	submissions: {
		newTry: 'Новая попытка',
		getSubmissionCaption: (submission: SubmissionInfo, selectedSubmissionIsLastSuccess: boolean,
			waitingForManualChecking: boolean
		): React.ReactNode => {
			const { timestamp, manualCheckingPassed } = submission;
			const timestampCaption = texts.getSubmissionDate(timestamp);
			if(manualCheckingPassed) {
				return timestampCaption + ", прошло код-ревью";
			} else if(waitingForManualChecking && selectedSubmissionIsLastSuccess) {
				return timestampCaption + ", ожидает код-ревью";
			}
			return timestampCaption;
		},
	},

	getLanguageCaption: (language: Language, languageNames: EnumDictionary<Language, string> | null): string => {
		if (languageNames !== null && languageNames[language] !== undefined)
			return languageNames[language];
		return language === Language.CSharp ? "C#" : capitalize(language);
	},

	acceptedSolutions: {
		title: 'Решения',
		content: <p>Изучите решения ваших коллег. Проголосуйте за решения, в которых вы нашли что-то новое для
			себя.</p>,

	},

	mocks: {
		headerSuccess: 'Решение отправлено на ревью – 5 баллов. После ревью преподаватель поставит итоговый балл',
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
			countTeacherReviews: (reviewsCount: number): string => `Преподаватель оставил ${ reviewsCount } ${ getPluralForm(
				reviewsCount, 'комментарий', 'комментария', 'комментариев') }. `,
		},
		bot: {
			title: 'Ulearn Bot',
			countBotComments: (botCommentsLength: number): string => `Бот нашёл ${ botCommentsLength } ${ getPluralForm(
				botCommentsLength, 'ошибку', 'ошибки', 'ошибок') }. `,
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
			buildWarning: (): React.ReactNode => <React.Fragment>
				Вы не получите баллы за задачу,<br/>
				если посмотрите чужие решения.<br/>
				<br/>
			</React.Fragment>,
			continue: 'Всё равно посмотреть',
		},

		edit: {
			text: 'Редактировать',
		},

		showAllCode: {
			text: 'Показать код полностью',
		},

		copyCode: {
			text: 'Скопировать код',
			onCopy: 'Код скопирован в буфер обмена',
		},

		statistics: {
			buildShortText: (usersWithRightAnswerCount: number): React.ReactNode =>
				<React.Fragment>Решило: { usersWithRightAnswerCount }</React.Fragment>,
			buildStatistics: (attemptedUsersCount: number, usersWithRightAnswerCount: number,
				lastSuccessAttemptDate: number
			): React.ReactNode =>
				lastSuccessAttemptDate
					? <React.Fragment>
						За всё время:<br/>
						{ attemptedUsersCount } { getPluralForm(attemptedUsersCount, 'студент пробовал',
					'студента пробовали', 'студентов пробовали') } решить
						задачу.<br/>
						{ getPluralForm(attemptedUsersCount, 'Решил', 'Решили', 'Решили') } { usersWithRightAnswerCount }
						<br/>
						<br/>
						Последний раз решили { getMoment(lastSuccessAttemptDate) }
					</React.Fragment>
					: 'Эту задачу ещё никто не решал',
		},
	},

	getSubmissionDate: (timestamp: string): string => {
		return convertDefaultTimezoneToLocal(timestamp).format('DD MMMM YYYY в HH:mm');
	}
};

export default texts;
