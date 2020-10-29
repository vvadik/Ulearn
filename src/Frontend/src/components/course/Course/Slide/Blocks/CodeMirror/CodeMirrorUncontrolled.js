import React from 'react';
import { UnControlled, } from "react-codemirror2";
import CodeMirror from "./CodeMirror";

import classNames from "classnames";
import PropTypes from 'prop-types';

import styles from './CodeMirror.less';


function CodeMirrorUncontrolled({ language, code, className, }) {
	const opts = {
		mode: CodeMirror.loadLanguageStyles(language),
		lineNumbers: true,
		scrollbarStyle: 'null',
		theme: 'default',
		readOnly: true,
		matchBrackets: true,
	}

	return (
		<div className={ classNames(styles.wrapper, className) }>
			<UnControlled
				editorDidMount={ (editor) => editor.setSize('auto', '100%') }
				className={ styles.editor }
				options={ opts }
				value={ code }
			/>
		</div>
	);
}

CodeMirrorUncontrolled.propTypes = {
	value: PropTypes.string,
	className: PropTypes.string,
}

export default CodeMirrorUncontrolled;
