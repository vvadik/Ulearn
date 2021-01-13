import CodeMirror from 'codemirror';

import 'codemirror/lib/codemirror';

import 'codemirror/mode/clike/clike';
import 'codemirror/mode/javascript/javascript';

export default function translateTextareaToCode(
	textarea: HTMLTextAreaElement,
	additionalSettings: Record<string, unknown>
): void {
	const cm = CodeMirror.fromTextArea(textarea, { ...additionalSettings, ...defaultSettings });
	cm.setSize('auto', 'auto');
}

const defaultSettings = {
	mode: "text/x-csharp",
	readOnly: 'nocursor',
};
