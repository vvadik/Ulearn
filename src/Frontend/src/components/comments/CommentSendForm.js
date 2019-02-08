import React, {Component} from 'react';
import PropTypes from "prop-types";
// import KeyboardEventHandler from 'react-keyboard-event-handler';
import BaseTextarea from "@skbkontur/react-ui/components/Textarea/Textarea";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Avatar from "../common/Avatar/Avatar";
import MarkdownButton from "./CommentSendForm/MarkdownButton";
import {Mobile, NotMobile} from "../../utils/responsive";

import styles from "./comment.less";

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

const markupByOperation = {
	bold: '**',
	italic: '_',
	code: '```',
};

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
			<div>
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
							onKeyDown={(e) => (((e.key === 'b' || e.key === 'i') && e.ctrlKey) ||
								(e.key === 'q' && e.altKey)) && this.onKeyPress(e)}
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
								<MarkdownButton
									text={this.renderHint('bold')}
									name="bold" width={11}
									onClick={this.onClick('bold')}/>
								<MarkdownButton
									text={this.renderHint('italic')}
									name="cursive" width={12}
									onClick={this.onClick('italic')}/>
								<MarkdownButton
									text={this.renderHint('code')}
									name="code" width={20}
									onClick={this.onClick('code')}/>
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
			</div>
		)
	}

	renderHint(markdownType) {
		switch(markdownType) {
			case "bold":
				return <span className={styles._yellow}>**<span className={styles._white}>жирный</span>**
					<br/><span className={styles._yellow}>Ctrl+B</span>
				</span>;
			case "italic":
				return  <span className={styles._yellow}>_<span className={styles._white}>курсив</span>_
					<br/><span className={styles._yellow}>Ctrl+I</span>
				</span>;
			case "link":
				return <span>[ссылка](url) <br/>
					<span className={styles._yellow}>Ctrl+K</span>
				</span>;
			case "code":
				return  <span className={styles._yellow}>```<span className={styles._white}>код</span>```
					<br/><span className={styles._yellow}>Ctrl+Shift+Q</span>
				</span>;
			default:
				return <span>обычный текст</span>;
		}
	};

	onKeyPress = (e) => {
		const range = this.textarea.current.selectionRange;
		let text = this.state.text;
		let markdownType = '';
		switch(e.key) {
			case ("Control" || "Meta") && "b":
				markdownType = 'bold';
				break;
			case ("Control" || "Meta") && "i":
				markdownType = 'italic';
				break;
			case "Alt" && "q":
				markdownType = 'code';
				break;
			default:
				return;
		}
		let {finalText, finalSelectionRange} = CommentSendForm.wrapRangeWithMarkdown(text, range, markdownType);
		this.setState({ text: finalText }, () => {
			this.textarea.current.setSelectionRange(
				finalSelectionRange.start,
				finalSelectionRange.end,
			);
		});
	};

	onChange = (e, value) => {
		this.setState({text: value});

		if (this.state.text) {
			this.setState({
				error: null,
			})
		}
	};

	onClick = (markdownType) => () => {
		const range = this.textarea.current.selectionRange;
		let text = this.state.text;
		let {finalText, finalSelectionRange} = CommentSendForm.wrapRangeWithMarkdown(text, range, markdownType);

		this.setState({ text: finalText }, () => {
			this.textarea.current.setSelectionRange(
				finalSelectionRange.start,
				finalSelectionRange.end,
			);
		});
		this.textarea.current.focus();
	};

	static wrapRangeWithMarkdown(text, range, markdownType) {
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
		if (!(markdownType in markupByOperation)) {
			throw new TypeError("unknown markdownType");
		}

		const before = text.substr(0, range.start);
		const target = text.substr(range.start, range.end - range.start);
		const after = text.substr(range.end);
		const formatted = markupByOperation[markdownType] + target + markupByOperation[markdownType];
		const finalText = before + formatted + after;
		const finalSelectionRange = {};

		if (target.length === 0) {
			finalSelectionRange.start = range.start + markupByOperation[markdownType].length;
			finalSelectionRange.end = range.start + markupByOperation[markdownType].length;
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