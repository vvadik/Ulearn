import React, { useState } from 'react';
import { UnControlled, } from "react-codemirror2";
import CodeMirror from "./CodeMirror";

import classNames from "classnames";
import PropTypes from 'prop-types';

import styles from './CodeMirror.less';
import texts from "src/components/course/Course/Slide/Blocks/CodeMirror/CodeMirror.texts";


function CodeMirrorUncontrolled({ language, code, className, }) {
	const opts = {
		mode: CodeMirror.loadLanguageStyles(language),
		lineNumbers: true,
		scrollbarStyle: 'null',
		theme: 'default',
		readOnly: true,
		matchBrackets: true,
	}
	const lines = code.split('\n');
	const [collapseEditor, showAllCode] = useState(lines.length > 20);
	const value = collapseEditor ? lines.splice(0, 5).join('\n') : code;

	return (
		<div className={ classNames(styles.wrapper, className) }>
			<UnControlled
				editorDidMount={ onEditorMount }
				className={ classNames(styles.editor, { [styles.lastLinesFading]: collapseEditor }) }
				options={ opts }
				value={ value }
			/>
			{ collapseEditor &&
			<React.Fragment>
				<div className={ styles.showAllCodeButton } onClick={ showAllCodeButtonClicked }>
					{ texts.controls.showAllCode.text }
				</div>
			</React.Fragment>
			}
		</div>
	);

	function onEditorMount(editor) {
		editor.setSize('auto', '100%')
	}

	function showAllCodeButtonClicked() {
		showAllCode(false);
	}
}

CodeMirrorUncontrolled.propTypes = {
	value: PropTypes.string,
	className: PropTypes.string,
}

export default CodeMirrorUncontrolled;
