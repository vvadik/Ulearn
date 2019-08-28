import CodeMirror from 'codemirror';

import 'codemirror/lib/codemirror';
import '../codeTranslator/codemirror.less';

import 'codemirror/mode/clike/clike';
import 'codemirror/mode/javascript/javascript';

export default function translateTextareaToCode(textarea, additionalSettings) {
	const cm = CodeMirror.fromTextArea(textarea, { ...additionalSettings, ...defaultSettings });
	cm.setSize('auto', 'auto');
}

const defaultSettings = {
	mode: "text/x-csharp",
	readOnly: 'nocursor',
};
