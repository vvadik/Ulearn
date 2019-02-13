import React, {Component} from 'react';
import PropTypes from "prop-types";
import Button from "@skbkontur/react-ui/components/Button/Button";
import BaseTextarea from "@skbkontur/react-ui/components/Textarea/Textarea";
import Avatar from "../../common/Avatar/Avatar";
import MarkdownButton from "./MarkdownButton";
import {Mobile, NotMobile} from "../../../utils/responsive";
import {boldIcon, codeIcon, italicIcon} from '../SVGIcons/SVGIcon';

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

class CommentSendForm extends Component {
	constructor(props) {
		super(props);
		this.state = {
			text: '',
			commentId: props.commentId,
			error: null,
		};
	}

	textarea = React.createRef();

	static getDerivedStateFromProps(props, state) {
		if (props.commentId !== state.commentId) {
			return {
				text: '',
				commentId: props.commentId,
			}
		}

		return null;
	}

	render() {
		const {author, sending} = this.props;

		return (
			<React.Fragment>
				<div className={styles.commentSendForm}>
					<Avatar user={author} size='big'/>
					<form className={styles.commentSend} onSubmit={this.onSubmit}>
						<Textarea
							ref={this.textarea}
							value={this.state.text}
							width={'100%'}
							error={this.hasError()}
							maxRows={15}
							rows={4}
							onChange={this.onChange}
							onKeyDown={this.onKeyPress}
							autoResize
							placeholder="Комментарий"
							disabled={sending} />
						<div className={styles.commentButtons}>
							<div className={styles.sendButtonDesktop}>
								<NotMobile>
									<Button
										use="primary"
										size="medium"
										type="submit"
										loading={sending}>
										Оставить комментарий
									</Button>
								</NotMobile>
							</div>
							<div className={styles.markdownButtons}>
								<span className={styles.markdownText}>Поддерживаем markdown</span>
								{ this.renderMarkdownOperation() }
							</div>
						</div>
						<Mobile>
							<Button
								use="primary"
								size="small"
								type="submit"
								loading={sending}>
								Оставить комментарий
							</Button>
						</Mobile>
					</form>
				</div>
			</React.Fragment>
		)
	}

	renderMarkdownOperation() {
		return Object.values(markupByOperation).map(operation =>
			<MarkdownButton
				hint={this.renderHint(operation)}
				icon={operation.icon}
				width={20}
				height={18}
				onClick={this.onClick(operation)} />
		);
	};

	renderHint({description, markup, hotkey}) {
		return <span className={styles._yellow}>{markup}<span className={styles._white}>{description}</span>{markup}
			<br/><span className={styles._yellow}>{hotkey.asText}</span>
		</span>;
	};

	onChange = (e, value) => {
		this.setState({text: value});

		if (this.state.text) {
			this.setState({
				error: null,
			})
		}
	};

	onKeyPress = (e) => {
		for (let op of Object.values(markupByOperation)) {
			if (e.key === op.hotkey.key &&
				(e.ctrlKey || e.metaKey) === !!op.hotkey.ctrl &&
				e.altKey === !!op.hotkey.alt) {
				this.transformTextToMarkdown(op);
				return;
			}
		}
	};

	onClick = (operation) => () => {
		this.transformTextToMarkdown(operation);
	};

	transformTextToMarkdown = (operation) => {
		const range = this.textarea.current.selectionRange;
		let text = this.state.text;

		let {finalText, finalSelectionRange} = CommentSendForm.wrapRangeWithMarkdown(text, range, operation);

		this.setState({ text: finalText }, () => {
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

	onSubmit = (event) => {
		const {onSubmit} = this.props;
		const text = this.state.text;

		event.preventDefault();

		if (!text) {
			this.setState({
				error: "Заполните поле комментария",
			});
			return;
		}

		onSubmit(text);
	};

	hasError = () => {
		return this.state.error !== null;
	};
}

const accountModel = PropTypes.shape({
	id: PropTypes.string,
	url: PropTypes.string,
});

CommentSendForm.propTypes = {
	/** Идентифицирует комментарий, с которым работает компонент.
	 * При изменении идентификатора текст в поле ввода очищается. При сохранении того же идентификатора - текст сохраняется. */
	commentId: PropTypes.string,
	author: accountModel,
	sending: PropTypes.bool,
	error: PropTypes.oneOf([PropTypes.string, PropTypes.object]),
	onSubmit: PropTypes.func,
};

export default CommentSendForm;