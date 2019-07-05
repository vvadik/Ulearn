const combineToObject = (question, answer) => {
	return {
		question: question,
		answer: answer
	}
};

const getLongQuestion =
	() => 'Если я спрошу очень большой вопрос, то сломается ли верстка? Если да, то почему? Как это поправить? Или не стоит? как думаешь? Почему?';

const getLongAnswer =
	() => 'Большой ответ также может сломать все, что захочет, должно быть стоит лучше приглядываться к стилям. И так же надо написать ещё 5 слов чтобы было побольше слов.';

const shortQuestionsExample = [
	combineToObject('Почему 1 больше 2?', 'Потому что 2 меньше 1... наверное'),
	combineToObject('Это так?', 'Нет, не так'),
	combineToObject(getLongQuestion(), getLongAnswer())
];

export default shortQuestionsExample;
