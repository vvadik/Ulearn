import translateTextareaToCode from 'src/codeTranslator/codemirror';
import translateTextToKatex from 'src/codeTranslator/katex';

export default function translateCode(element, settings = {}) {
	for (const { id, selector, translateFunction } of translators) {
		const elements = selector(element);

		for (const element of elements) {
			if (element.style.display === 'none') {
				continue;
			}

			translateFunction(element, settings[id]);
		}
	}
}

const translators = [
	{
		id: 'codeMirror',
		selector: (element) => element.querySelectorAll('.code'),
		translateFunction: (element, settings = { additionalSettings: {} }) => {
			translateTextareaToCode(element, settings.additionalSettings)
		},
	},
	{
		id: 'katex',
		selector: (element) => element.querySelectorAll('.tex'),
		translateFunction: (element, settings = { additionalSettings: {} }) => {
			translateTextToKatex(element, settings.additionalSettings)
		},
	},
];
