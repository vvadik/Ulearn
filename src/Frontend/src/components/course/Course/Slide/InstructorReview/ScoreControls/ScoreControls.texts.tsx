import React from "react";

export default {
	scoresText: 'Оценка за работу в % ',
	submitButtonText: 'Оценить',
	changeScoreText: 'Изменить оценку',
	lastReviewScoreText: 'за пред. ревью',
	getCodeReviewToggleText: (exerciseTitle: string): React.ReactText => `Принимать ещё код-ревью у этого студента по практике «${ exerciseTitle }»`,
	getScoreText: (percent: number): React.ReactText => `Работа оценена на ${ percent }%`,
};
