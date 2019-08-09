import translateTextareaToCode from '../../src/codeTranslator/codemirror';
import translateTextToKatex from '../../src/codeTranslator/katex';

export default function translateCode(element, settings = {}) {
	for (const { id, selector, translateFunction } of translators) {
		for (const selectedElement of selector(element)) {
			if (selectedElement.style.display === 'none') {
				continue;
			}

			translateFunction(selectedElement, settings[id]);
		}
	}
}

const translators = [
	{
		id: 'codeMirror',
		selector: (element) => element.querySelectorAll('textarea'),
		translateFunction: (element, settings = { additionalSettings: {}, withMarginAuto: false }) => {
			translateTextareaToCode(element, settings.additionalSettings, settings.withMarginAuto)
		},
	},
	{
		id: 'katex',
		selector: (element) => element.querySelectorAll('span.tex, div.tex'),
		translateFunction: (element, settings = { additionalSettings: {} }) => translateTextToKatex(element, settings.additionalSettings),
	},
];
