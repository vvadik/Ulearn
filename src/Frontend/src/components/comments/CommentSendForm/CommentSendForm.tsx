import React, { Component } from "react";
import classNames from 'classnames';

import { Button } from "ui";
import Avatar from "../../common/Avatar/Avatar";
import MarkdownEditor from "./MarkdownEditor/MarkdownEditor";

import { isMobile } from "src/utils/getDeviceType";

import { ShortUserInfo } from "src/models/users";

import styles from "./CommentSendForm.less";

interface Props {
	text: string;
	author?: ShortUserInfo;
	commentId: string;
	sendStatus?: string;
	submitTitle: string;
	cancelTitle?: string;
	className?: string;

	isForInstructors?: boolean;
	isShowFocus: {
		inSendForm?: boolean;
		inEditForm?: boolean;
		inReplyForm?: boolean;
	};
	sending?: boolean;

	handleCancel: () => void;
	handleSubmit: (commentId: string, text: string) => void;
}

interface State {
	text: string;
	error: null | string;
	commentId: string;
	status?: string;
}

class CommentSendForm extends Component<Props, State> {
	constructor(props: Props) {
		super(props);

		this.state = {
			text: props.text || "",
			error: null,
			commentId: props.commentId,
			status: props.sendStatus,
		};
	}

	static getDerivedStateFromProps(props: Props, state: State): Partial<State> | null {
		if(props.commentId !== state.commentId) {
			return {
				text: props.text || "",
				commentId: props.commentId,
			};
		}

		return null;
	}

	render(): React.ReactElement {
		const { author, isForInstructors, isShowFocus, className } = this.props;
		const { error, text } = this.state;

		const classes = classNames(className, styles.formContainer);
		const avatarClasses = classNames(styles.avatar, styles.visibleOnDesktopAndTablet);

		return (
			<div className={ classes }>
				{ author && (<Avatar className={ avatarClasses } user={ author } size="big"/>) }
				<form className={ styles.form } onSubmit={ this.handleSubmit }>
					<MarkdownEditor
						isShowFocus={ isShowFocus }
						isForInstructors={ isForInstructors }
						hasError={ error !== null }
						text={ text }
						handleChange={ this.handleChange }
						handleSubmit={ this.handleSubmit }>
						<div className={ styles.buttons }>
							{ this.renderSubmitButton() }
							{ this.renderCancelButton() }
						</div>
					</MarkdownEditor>
				</form>
			</div>
		);
	}

	renderSubmitButton(): React.ReactElement {
		const { submitTitle = "Оставить комментарий", sending } = this.props;

		return (
			<Button
				use="primary"
				size={ isMobile() ? "small" : "medium" }
				type="submit"
				loading={ sending }>
				{ submitTitle }
			</Button>
		);
	}

	renderCancelButton(): React.ReactNode {
		const { handleCancel, cancelTitle = "Отменить" } = this.props;

		if(!handleCancel) {
			return null;
		}

		return (
			<Button
				use="default" //secondary
				size={ isMobile() ? "small" : "medium" }
				type="button"
				onClick={ handleCancel }>
				{ cancelTitle }
			</Button>
		);
	}

	handleSubmit = (event: React.SyntheticEvent): void => {
		event.preventDefault();

		const { text } = this.state;

		if(!text) {
			this.setState({
				error: "Заполните поле комментария",
			});
			return;
		}

		const { commentId, handleSubmit } = this.props;

		handleSubmit(commentId, text);
	};

	handleChange = (text: string, callback?: () => void): void => {
		this.setState({ text, error: null }, callback);
	};
}

export default CommentSendForm;
