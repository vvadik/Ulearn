import React, {Component} from 'react';
import PropTypes from "prop-types";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Avatar from "../../common/Avatar/Avatar";
import MarkdownEditor from "./MarkdownEditor";

import styles from "./commentSendForm.less";

class CommentSendForm extends Component {
	state = {
		text: '',
		error: null,
		commentId: null,
	};

	static getDerivedStateFromProps(props, state) {
		if (props.commentId !== state.commentId) {
			return {
				text: props.text || '',
				commentId: props.commentId,
			};
		}

		return null;
	}

	editor = React.createRef();

	render() {
		const { author } = this.props;
		const { error, text } = this.state;

		return (
			<React.Fragment>
				<div className={styles.commentSendForm}>
					{author && (
						<div className={styles.avatar}>
							<Avatar user={author} size='big' />
						</div>
					)}
					<form className={styles.commentSend} onSubmit={this.handleSubmit}>
						<MarkdownEditor
							ref={this.editor}
							hasError={error !== null}
							text={text}
							onChange={this.handleChange}
						>
							<div className={styles.buttons}>
								{this.renderSubmitButton()}
								{this.renderCancelButton()}
							</div>
						</MarkdownEditor>
					</form>
				</div>
			</React.Fragment>
		)
	}

	renderSubmitButton() {
		const { submitTitle = 'Оставить комментарий', sending } = this.props;

		return (
			<Button
				use="primary"
				size="medium"
				type="submit"
				loading={sending}>
				{submitTitle}
			</Button>
		);
	}

	renderCancelButton() {
		const { onCancel, cancelTitle = 'Отменить' } = this.props;

		if (!onCancel) {
			return null;
		}

		return (
			<Button
				use="secondary"
				size="medium"
				type="button"
				onClick={onCancel}>
				{cancelTitle}
			</Button>
		);
	}

	handleSubmit = (event) => {
		event.preventDefault();

		const { text } = this.state;
		const { onSubmit } = this.props;

		if (!text) {
			this.setState({
				error: "Заполните поле комментария",
			});
			return;
		}

		onSubmit(text);
	};

	handleChange = (text) => {
		return new Promise((resolve) => {
			this.setState({ text, error: null }, resolve);
		});
	}
}

CommentSendForm.propTypes = {
	/** Идентифицирует комментарий, с которым работает компонент.
	 * При изменении идентификатора текст в поле ввода очищается.
	 * При сохранении того же идентификатора - текст сохраняется.*/
	commentId: PropTypes.string,
	author: Avatar.propTypes,
	sending: PropTypes.bool,
	error: PropTypes.oneOf([PropTypes.string, PropTypes.object]),
	onSubmit: PropTypes.func,
};

export default CommentSendForm;