import getPluralForm from "src/utils/getPluralForm";

export default {
	getSlideScore: ({ score, maxScore }, showZeroScore = false) => showZeroScore && score === 0
		? `Максимум ${ maxScore } ${ getPluralForm(maxScore, 'балл', 'балла', 'баллов') }`
		: `${ score } ${ getPluralForm(score, 'балл', 'балла', 'баллов') } из ${ maxScore }`,
	skippedHeaderText: "Вы посмотрели чужие решения, поэтому не получите баллы за эту задачу",
	hiddenSlideText: "Студенты не видят этот слайд.",
	hiddenSlideTextWithStudents: (showedGroupsIds) => `Студенты ${ showedGroupsIds.join(', ') } видят этот слайд.`,
}
