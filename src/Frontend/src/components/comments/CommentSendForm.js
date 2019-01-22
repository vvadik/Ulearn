import React, { Component } from 'react';
import connect from "react-redux/es/connect/connect";
import PropTypes from "prop-types";
import Textarea from "@skbkontur/react-ui/components/Textarea/Textarea";
import Button from "@skbkontur/react-ui/components/Button/Button";
import LinkIcon from "@skbkontur/react-icons/Link";
import ArrowChevron2Left from "@skbkontur/react-icons/ArrowChevron2Left";
import ArrowChevron2Right from "@skbkontur/react-icons/ArrowChevron2Right";
import Avatar from "../common/Avatar/Avatar";

import styles from "./comment.less";

class CommentSendForm extends Component {
	state = {
		value: '',
		loading: false,
	};

	render() {
		// let userId = this.props.account.id;

		return (
		<div>
			<h1 className={styles.header}>Комментарии</h1>
			<div className={styles["comment-send-form"]}>
				{/*<Avatar user={userId} size='big' />*/}
				<form onSubmit={this.onSubmit}>
					<div className={styles["comment-send"]}>
						<Textarea
							value={this.state.value}
							width={540}
							maxRows={15}
							rows={5}
							onChange={(_, value) => this.setState({ value })}
							autoResize
							placeholder="Комментарий"
						/>
						<div className={styles["comment-buttons"]}>
							<Button
								use="primary"
								size="medium"
								type="submit"
								loading={this.state.loading}>
								Оставить комментарий
							</Button>
							<div className={styles["markdown-buttons"]}>
								<Button use="link">B</Button>
								<Button use="link">I</Button>
								<Button use="link" icon={<LinkIcon />} />
								<Button use="link" icon={`${<ArrowChevron2Left />} ${<ArrowChevron2Right />}`} />
							</div>
						</div>
					</div>
				</form>
			</div>
		</div>
		)
	}

	onSubmit = () => {
		console.log('Hello');
	};

	static mapStateToProps(state) {
		return {
			account: state.account,
		}
	}
}

CommentSendForm.propTypes = {
	account: PropTypes.object,
};

// CommentSendForm = connect(CommentSendForm.mapStateToProps)(CommentSendForm);
export default CommentSendForm;