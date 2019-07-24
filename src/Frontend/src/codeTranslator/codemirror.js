import CodeMirror from 'codemirror';

import 'codemirror/lib/codemirror';
import 'codemirror/lib/codemirror.css';

import 'codemirror/mode/clike/clike';
import 'codemirror/mode/javascript/javascript';

export default function translateTextareaToCode(textarea, additionalSettings) {
	const cm = CodeMirror.fromTextArea(textarea, { ...additionalSettings, ...defaultSettings });

	cm.setSize('100%', '100%');
	cm.setOption('lineNumbers', cm.lineCount() > 1);
}

const defaultSettings = {
	mode: "text/x-csharp",
	readOnly: 'nocursor',
};
