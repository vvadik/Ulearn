import React, {Component} from 'react';
import PropTypes from "prop-types";
import BaseTextarea from "@skbkontur/react-ui/components/Textarea/Textarea";
import {boldIcon, codeIcon, italicIcon} from "../SVGIcons/SVGIcon";
import MarkdownButton from "./MarkdownButton";

import styles from "./commentSendForm.less";

const markupByOperation = {
	bold: {
		markup: '**',
		description: "жирный",
		hotkey: {
			asText: 'Ctrl + B',
			ctrl: true,
			key: "b"
		},
		icon: boldIcon,
	},
	italic: {
		markup: '_',
		description: "курсив",
		hotkey: {
			asText: 'Ctrl + I',
			ctrl: true,
			key: "i"
		},
		icon: italicIcon,
	},
	code: {
		markup: '```',
		description: "блок кода",
		hotkey: {
			asText: 'Alt + Q',
			alt: true,
			key: "q"
		},
		icon: codeIcon,
	},
};

// monkey patch
class Textarea extends BaseTextarea {
	get selectionRange() {
		if (!this.node) {
			throw new Error('Cannot call "selectionRange" on unmounted Input');
		}

		return {
			start: this.node.selectionStart,
			end: this.node.selectionEnd,
		};
	};
}

class MarkdownEditor extends Component {
	textarea = React.createRef();

	render() {
		const {hasError, text} = this.props;

		return (
			<React.Fragment>
				<Textarea
					ref={this.textarea}
					value={text}
					width={'100%'}
					error={hasError}
					maxRows={15}
					rows={4}
					onChange={this.handleChange}
					onKeyDown={this.handleKeyDown}
					autoResize
					placeholder="Комментарий" />
				<div className={styles.footer}>
					{ this.props.children }
					<MarkdownButton
						markupByOperation={markupByOperation}
						onClick={this.handleClick} />
				</div>
			</React.Fragment>
		)
	}

	handleChange = (e, value) => {
		const {onChange} = this.props;

		onChange(value);
	};

	handleKeyDown = (e) => {
		for (let operation of Object.values(markupByOperation)) {
			if (e.key === operation.hotkey.key &&
				(e.ctrlKey || e.metaKey) === !!operation.hotkey.ctrl &&
				e.altKey === !!operation.hotkey.alt) {
				this.transformTextToMarkdown(operation);
				return;
			}
		}
	};

	handleClick = (operation) => {
		this.transformTextToMarkdown(operation);
	};

	transformTextToMarkdown = (operation) => {
		const {onChange, text} = this.props;
		const range = this.textarea.current.selectionRange;

		let {finalText, finalSelectionRange} = MarkdownEditor.wrapRangeWithMarkdown(text, range, operation);

		onChange(finalText).then(() => {
			this.textarea.current.setSelectionRange(
				finalSelectionRange.start,
				finalSelectionRange.end,
			);
		});
	};

	static wrapRangeWithMarkdown(text, range, operation) {
		if (typeof text !== "string") {
			throw new TypeError("text should be a string");
		}
		if (typeof range !== "object" || range === null
			|| !('start' in range) || !('end' in range)
			|| typeof range.start !== 'number' || typeof range.end !== 'number'
		) {
			throw new TypeError("range should be an object with `start` and `end` properties of type `number`");
		}
		if (range.start < 0 || range.end < range.start || range.end > text.length) {
			throw new RangeError("range should be within 0 and text.length");
		}
		const before = text.substr(0, range.start);
		const target = text.substr(range.start, range.end - range.start);
		const after = text.substr(range.end);
		const formatted = operation.markup + target + operation.markup;
		const finalText = before + formatted + after;
		const finalSelectionRange = {};

		if (target.length === 0) {
			finalSelectionRange.start = range.start + operation.markup.length;
			finalSelectionRange.end = range.end + operation.markup.length;
		} else {
			finalSelectionRange.start = range.start;
			finalSelectionRange.end = range.start + formatted.length;
		}

		return {finalText, finalSelectionRange};
	}
}

MarkdownEditor.propTypes = {
	hasError: PropTypes.bool,
	text: PropTypes.string,
};

export default MarkdownEditor;