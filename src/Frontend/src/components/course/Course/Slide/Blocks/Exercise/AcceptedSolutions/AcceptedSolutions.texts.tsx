import React from "react";
import getPluralForm from "src/utils/getPluralForm";
import { ShortUserInfo } from "src/models/users";
import getGenderForm from "src/utils/getGenderForm";

const texts = {
	title: 'Чужие решения',
	instructorTabName: 'Как преподаватель',
	studentTabName: 'Как студент',
	studentInstructions: 'Проголосуйте за решения, в которых вы нашли что-то новое для себя',
	instructorInstructions: <>
		Выберите решения, которые будут покзываться студентам в списке «Выбранные
		преподавателями».
		Список рекомендованных решений один на весь курс, т.е. вы редактируете глобальный список.
	</>,
	noSubmissions: 'Никто еще не решил задачу',
	promotedSolutionsHeader: 'Выбранные преподавателями',
	solutionsHeader: 'Новые решения',
	likedSolutionsHeader: 'Последние полайканные студентами решения',
	promoteHint: 'Рекомендовать',
	unpromoteHint: 'Убрать из рекомендованных',
	getPromotedByText: (user: ShortUserInfo) => <>{ getGenderForm(user.gender, "Рекомендовала ", "Рекомендовал ") }
		<i>{ user.visibleName }</i></>,
	getDisabledLikesHint: (count: number): string =>
		count + ' ' + getPluralForm(count, 'студент лайкнул', 'студента лайкнули', 'студентов лайкнули')
};

export default texts;
