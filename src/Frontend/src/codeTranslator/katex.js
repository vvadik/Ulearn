import katex from 'katex';

import "katex/dist/katex.min.css";
import renderMathInElement from 'katex/dist/contrib/auto-render'

export default function translateTextToKatex(element, additionalSettings) {
	const text = element.innerText;

	if (text && element.dataset.transformation !== katexTransformed) {
		element.dataset.transformation = katexTransformed;
		element.maxWidth = '90%';
		renderMathInElement(
			element,
			{
				delimiters: [
					{left: "$$", right: "$$", display: true},
					{left: "\\[", right: "\\]", display: true},
					{left: "$", right: "$", display: false},
					{left: "\\(", right: "\\)", display: false}
				]
			}
		);
		//katex.render(text, element, { ...additionalSettings, ...defaultSetting });
	}
}

const defaultSetting = {
	throwOnError: false,
};

const katexTransformed = 'katex';
