import React from 'react';

import { UnControlled } from "react-codemirror2";

import PropTypes from 'prop-types';
import classNames from 'classnames';

import 'codemirror/lib/codemirror';

import styles from './CodeMirror.less';

function CodeMirror({ language, code, className, }) {
	const opts = {
		mode: loadLanguageStyles(language),
		lineNumbers: true,
		readOnly: 'true',
		scrollbarStyle: 'null',
		lineWrapping: true,
	};

	return (
		<div className={ classNames(styles.wrapper, className) }>
			<UnControlled
				editorDidMount={ onEditorMount }
				className={ styles.editor }
				options={ opts }
				value={ code }
			/>
		</div>
	);

	function onEditorMount(editor) {
		editor.setSize('auto', 'auto');
	}

	function loadLanguageStyles(language) {
		switch (language.toLowerCase()) {
			case 'csharp':
				require('codemirror/mode/clike/clike');
				return `text/x-csharp`;
			case 'java':
				require('codemirror/mode/clike/clike');
				return `text/x-java`;

			case 'javascript':
				require('codemirror/mode/javascript/javascript');
				return `text/javascript`;
			case 'typescript':
				require('codemirror/mode/javascript/javascript');
				return `text/typescript`;
			case 'jsx':
				require('codemirror/mode/jsx/jsx');
				return `text/jsx`;

			case 'python2':
				require('codemirror/mode/python/python');
				return `text/x-python`;
			case 'python3':
				require('codemirror/mode/python/python');
				return `text/x-python`;

			case 'css':
				require('codemirror/mode/css/css');
				return `text/css`;

			case 'haskell':
				require('codemirror/mode/haskell/haskell');
				return `text/x-haskell`;

			default:
				require('codemirror/mode/xml/xml');
				return 'text/html';
		}
	}
}

CodeMirror.propTypes = {
	className: PropTypes.string,
	language: PropTypes.string.isRequired,
	code: PropTypes.string.isRequired,
}

export default CodeMirror;
