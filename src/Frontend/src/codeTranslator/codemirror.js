import CodeMirror from 'codemirror';

import 'codemirror/lib/codemirror';
import '../codeTranslator/codemirror.less';

import 'codemirror/mode/clike/clike';
import 'codemirror/mode/javascript/javascript';

export default function translateTextareaToCode(textarea, additionalSettings, withMarginAuto) {
	const cm = CodeMirror.fromTextArea(textarea, { ...additionalSettings, ...defaultSettings });

	if (withMarginAuto) {
		cm.setSize('50%', 'auto');
	} else {
		cm.setSize('auto', 'auto');
	}
}

const defaultSettings = {
	mode: "text/x-csharp",
	readOnly: 'nocursor',
};
