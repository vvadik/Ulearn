import translateTextareaToCode from 'src/codeTranslator/codemirror';
import translateTextToKatex from 'src/codeTranslator/katex';

export default function translateCode(
	element: HTMLElement,
	settings: { [id: string]: Record<string, unknown> } = {},
	excludeTranslatorsByIds: { [id: string]: boolean } = {}
): void {
	for (const { id, selector, translateFunction } of translators) {
		if(excludeTranslatorsByIds[id]) {
			continue;
		}
		const elements = selector(element);

		for (const element of elements) {
			if(element.style.display === 'none') {
				continue;
			}

			translateFunction(element, settings[id]);
		}
	}
}

const translators = [
	{
		id: 'codeMirror',
		selector: (element: HTMLElement) => element.querySelectorAll('.code') as NodeListOf<HTMLElement>,
		translateFunction: (element: HTMLElement, additionalSettings = {}) => {
			translateTextareaToCode(element as HTMLTextAreaElement, additionalSettings);
		},
	},
	{
		id: 'katex',
		selector: (element: HTMLElement) => element.querySelectorAll('.tex') as NodeListOf<HTMLElement>,
		translateFunction: (element: HTMLElement, additionalSettings = {}) => {
			translateTextToKatex(element, additionalSettings);
		},
	},
];
