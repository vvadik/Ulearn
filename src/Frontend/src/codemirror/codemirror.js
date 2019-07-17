import CodeMirror from 'codemirror';
import 'codemirror/lib/codemirror.css';
import 'codemirror/lib/codemirror';
import 'codemirror/mode/clike/clike';

export default function translateTextareaToCode(textarea, additionalSettings) {
	textarea.innerText = textarea.getAttribute('data-code'); //TODO ROZENTOR remove its after backend implementation

	const cm = CodeMirror.fromTextArea(textarea, {...additionalSettings, ...defaultSettings});
	cm.setSize('100%', '100%');
	cm.setOption('lineNumbers', cm.lineCount() === 0);
}

const defaultSettings = {
	mode: "text/x-csharp",
	readOnly: 'nocursor',
};
