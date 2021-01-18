import katex from 'katex';

import "katex/dist/katex.min.css";
import renderMathInElement from 'katex/dist/contrib/auto-render'

export default function translateTextToKatex(element: HTMLElement, additionalSettings: Record<string, unknown>): void {
	const text = element.innerText;

	if(text && element.dataset.transformation !== katexTransformed) {
		element.dataset.transformation = katexTransformed;
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
	}
}

const defaultSetting = {
	throwOnError: false,
};

const katexTransformed = 'katex';
