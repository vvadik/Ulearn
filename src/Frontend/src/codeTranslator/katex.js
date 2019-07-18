import katex from 'katex';

export default function translateTextToKatex(element, additionalSettings) {
	const text = element.innerText;

	if (text && element.title !== tranformedTitle) {
		element.title = tranformedTitle;
		katex.render(text, element, {...additionalSettings, ...defaultSetting});
	}
}

const defaultSetting = {
	throwOnError: false,
};

const tranformedTitle = 'katex transformed';
