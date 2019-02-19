import React, {Component} from 'react';
import PropTypes from "prop-types";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Avatar from "../../common/Avatar/Avatar";
import MarkdownEditor from "./MarkdownEditor";
import {Mobile, NotMobile} from "../../../utils/responsive";

import styles from "./commentSendForm.less";
import Action from "../Comment/Action";

// <CommentSendForm key={commentId} />

class CommentSendForm extends Component {
	constructor(props) {
		super(props);
		this.state = {
			error: null,
			commentId: props.commentId,
		};
	}

	static getDerivedStateFromProps(props, state) {
		if (props.commentId !== state.commentId) {
			return {
				text: '',
				commentId: props.commentId,
			}
		}

		return null;
	}

	editor = React.createRef();

	render() {
		const { author, commentId, commentText } = this.props;

		return (
			<React.Fragment>
				<div className={styles.commentSendForm}>
					<Avatar user={author} size='big'/>
					<form className={styles.commentSend} onSubmit={this.onSubmit}>
						<MarkdownEditor
							ref={this.editor}
							hasError={this.hasError}
							text={commentId ? commentText : '' }/>
						{ this.renderAction(this.props.action) }
					</form>
				</div>
			</React.Fragment>
		)
	}

	renderAction(action) {
		const { commentId, sending } = this.props;
		return (
			<React.Fragment>
				<div className={styles.commentButtons}>
					<div className={styles.sendButtonDesktop}>
						<NotMobile>
							<Action commentId={commentId} sending={sending} action={action} />
							{ action === 'edit' && <Action commentId={commentId} sending={sending} action={action} /> }
						</NotMobile>
					</div>
				</div>
				<Mobile>
					<Action commentId={commentId} sending={sending} action={action} />
					{ action === 'edit' && <Action commentId={commentId} sending={sending} action={action} /> }
				</Mobile>
			</React.Fragment>
		)
	}

	onSubmit = (event) => {
		const text = this.editor.current.text;

		const { onSubmit } = this.props;

		event.preventDefault();

		if (!text) {
			this.setState({
				error: "Заполните поле комментария",
			});
			return;
		}

		onSubmit(text);
	};

	hasError = (textValue) => {
		if (textValue) {
			this.setState({
				error: null,
			});
		}
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