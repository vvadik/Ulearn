import katex from 'katex';

import "katex/dist/katex.min.css";

export default function translateTextToKatex(element, additionalSettings) {
	const text = element.innerText;

	if (text && element.dataset.transformation !== katexTransformed) {
		element.dataset.transformation = katexTransformed;
		element.maxWidth = '90%';
		katex.render(text, element, { ...additionalSettings, ...defaultSetting });
	}
}

const defaultSetting = {
	throwOnError: false,
};

const katexTransformed = 'katex';
