import translateTextareaToCode from '../../src/codeTranslator/codemirror';
import translateTextToKatex from '../../src/codeTranslator/katex';

export default function translateCode(element) {
	for (const codeMirrorElement of element.querySelectorAll('textarea')) {
		if (codeMirrorElement.style.display === 'none') {
			continue;
		}

		translateTextareaToCode(codeMirrorElement);
	}

	for (const katexElement of element.querySelectorAll('span.tex, div.tex')) {
		if (katexElement.style.display === 'none') {
			continue;
		}

		translateTextToKatex(katexElement);
	}
}
