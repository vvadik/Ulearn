import React, { Component } from 'react';
import PropTypes from "prop-types";
import { userType } from "../commonPropTypes";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Avatar from "../../common/Avatar/Avatar";
import MarkdownEditor from "./MarkdownEditor/MarkdownEditor";
import { NotMobile } from "../../../utils/responsive";

import styles from "./CommentSendForm.less";

class CommentSendForm extends Component {
	constructor(props) {
		super(props);

		this.state = {
			text: props.text || '',
			error: null,
			commentId: props.commentId,
			status: props.isSuccessSend,
		};
	}

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
		const {author, isForInstructors} = this.props;
		const {error, text} = this.state;

		return (
			<div className={styles.commentSendForm}>
				{author && (
					<NotMobile>
						<div className={styles.avatar}>
							<Avatar user={author} size='big' />
						</div>
					</NotMobile>
				)}
				<form className={styles.commentSend} onSubmit={this.handleSubmit}>
					<MarkdownEditor
						ref={this.editor}
						isForInstructors={isForInstructors}
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
		)
	}

	renderSubmitButton() {
		const {submitTitle = 'Оставить комментарий', sending} = this.props;

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
		const {onCancel, cancelTitle = 'Отменить'} = this.props;

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

		const {text} = this.state;

		if (!text) {
			this.setState({
				error: "Заполните поле комментария",
			});
			return;
		}

		const {commentId, handleSubmit} = this.props;

		handleSubmit(commentId, text);
	};

	handleChange = (text) => {
		return new Promise((resolve) => {
			this.setState({text, error: null}, resolve);
		});
	};

}

CommentSendForm.propTypes = {
	author: userType,
	commentId: PropTypes.number,
	handleSubmit: PropTypes.func,
	handleAddNewComment: PropTypes.func,
	isSuccessSend: PropTypes.string,
	isForInstructors: PropTypes.bool,
	sending: PropTypes.bool,
	submitTitle: PropTypes.string,
	onCancel: PropTypes.func,
	cancelTitle: PropTypes.string,
};

export default CommentSendForm;