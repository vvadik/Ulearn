import React, {Component} from 'react';
import PropTypes from "prop-types";
import BaseTextarea from "@skbkontur/react-ui/components/Textarea/Textarea";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Avatar from "../common/Avatar/Avatar";
import MarkdownButton from "./CommentSendForm/MarkdownButton";

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
							autoResize
							placeholder="Комментарий"
							disabled={sending}
						/>
						<div className={styles.commentButtons}>
							<Button
								use="primary"
								size="medium"
								type="submit"
								loading={sending}>
								Оставить комментарий
							</Button>
							<div className={styles.markdownButtons}>
								<span className={styles.markdownText}>Поддерживаем markdown</span>
								<MarkdownButton
									text="**жирный** Ctrl+B"
									name="bold" width={11}
									onClick={this.onClick('bold')}/>
								<MarkdownButton
									text="*курсив* Ctrl+I"
									name="cursive" width={12}
									onClick={this.onClick('cursive')}/>
								<MarkdownButton
									text="(ссылка)[url] Ctrl+k"
									name="link" width={18}
									onClick={this.onClick('link')}/>
								<MarkdownButton
									text="```код``` Ctrl+Shift+Q"
									name="code" width={20}
									onClick={this.onClick('code')}/>
							</div>
						</div>
					</form>
				</div>
			</div>
		)
	}

	onChange = (_, value) => {
		this.setState({text: value});

		if (this.state.text) {
			this.setState({
				error: null,
			})
		}
	};

	onClick = (type) => () => {
		// let selection = window.getSelection().toString();
		// let selectionEl = document.onblur;
		const range = this.textarea.current.selectionRange;

		const before = this.state.text.substr(0, range.start);
		const target = this.state.text.substr(range.start, range.end - range.start);
		const after = this.state.text.substr(range.end);

		const formatted = this.applyFormat(target, type);
		const offset = formatted.indexOf(target);

		const text = before + formatted + after;

		this.setState({ text }, () => {
			this.textarea.current.setSelectionRange(
				range.start + offset,
				range.end + offset,
			);
		});
	};

	applyFormat(text, type) {
		switch(type) {
			case "bold":
				return `**${text}**`;
			case "cursive":
				return `_${text}_`;
			case "link":
				return `[${text}](${text})`;
			case "code":
				return `\`${text}\``;
			default:
				return text;
		}
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
	commentId: PropTypes.string,
	author: accountModel,
	sending: PropTypes.bool,
	error: PropTypes.oneOf(PropTypes.string, PropTypes.object),
	onSubmit: PropTypes.func,
};


export default CommentSendForm;