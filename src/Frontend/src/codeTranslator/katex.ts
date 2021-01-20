import katex, { KatexOptions } from 'katex';

import "katex/dist/katex.min.css";
import renderMathInElement from 'katex/dist/contrib/auto-render.js';

export function translateTextToKatex(element: HTMLElement, additionalSettings: KatexOptions): void {
	const text = element.innerText;

	if(text && element.dataset.transformation !== katexTransformed) {
		element.dataset.transformation = katexTransformed;
		katex.render(text, element, { ...additionalSettings, ...defaultSettingTextToKatex });
	}
}


export function translateTexInHtml(element: HTMLElement, additionalSettings: Record<string, unknown>): void {
	const text = element.innerText;

	if(text && element.dataset.transformation !== katexTransformed) {
		element.dataset.transformation = katexTransformed;

		renderMathInElement(
			element,
			{
				...additionalSettings,
				...defaultSettingTexInHtml,
			}
		);
	}
}

const defaultSettingTextToKatex: Partial<KatexOptions> = {
	throwOnError: false,
};

const defaultSettingTexInHtml = {
	delimiters: [
		{ left: "$$", right: "$$", display: true },
		{ left: "\\[", right: "\\]", display: true },
		{ left: "$", right: "$", display: false },
		{ left: "\\(", right: "\\)", display: false }
	],
};

const katexTransformed = 'katex';
