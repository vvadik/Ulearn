import React, { Component } from 'react';
import connect from "react-redux/es/connect/connect";
import PropTypes from "prop-types";
import api from "../../api/index";
import Textarea from "@skbkontur/react-ui/components/Textarea/Textarea";
import Button from "@skbkontur/react-ui/components/Button/Button";
import LinkIcon from "@skbkontur/react-icons/Link";
import ArrowChevron2Left from "@skbkontur/react-icons/ArrowChevron2Left";
import ArrowChevron2Right from "@skbkontur/react-icons/ArrowChevron2Right";
import Avatar from "../common/Avatar/Avatar";

import styles from "./comment.less";

class CommentSendForm extends Component {
	state = {
		comments: [],
		text: '',
		loading: false,
	};

	componentDidMount() {
		const slideId = this.props.match.params.slideId;

		this.props.enterToCourse(this.courseId, slideId);
		this.loadComment(this.courseId, slideId, this.isInstructor);
	}

	get courseId() {
		return this.props.match.params.courseId.toLowerCase();
	}

	get role() {
		return this.props.account.roleByCourse[this.courseId];
	}

	_instructor = null;
	get isInstructor() {
		if (this._instructor === null) {
			this._instructor = this.props.account.isSystemAdministrator ||
				['courseAdmin', 'instructor'].includes(this.role);
		}
		return this._instructor;
	}

	loadComment = (courseId, slideId) => {
		this.setState({ loading: true });

		api.comments.getComments(courseId, slideId)
			.then(json => {
				let comments = json.comments;
				this.setState({
					comments: comments,
					loading: false,
				})
			})
			.catch(console.error)
			.finally(this.setState({ loading: false })
			)
	};

	render() {
		let userId = this.props.account.id;

		return (
		<div>
			<h1 className={styles.header}>Комментарии</h1>
			<div className={styles["comment-send-form"]}>
				<Avatar user={userId} size='big' />
				<form onSubmit={this.onSubmit}>
					<div className={styles["comment-send"]}>
						<Textarea
							value={this.state.value}
							width={540}
							maxRows={15}
							rows={5}
							onChange={(_, value) => this.setState({ text: value })}
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
		const text = this.state.text;
		const replyTo = 0;
		const slideId = this.props.match.params;

		this.setState({ loading: true });
		api.comments.addComment(this.courseId, slideId, text, replyTo, this.isInstructor)
			.catch(console.error)
			.finally(this.setState({ loading: false }));
	};

	static mapStateToProps(state) {
		return {
			account: state.account,
		}
	};

	static mapDispatchToProps(dispatch) {
		return {
			enterToCourse: (courseId, slideId) => dispatch({
				type: 'COURSES__COURSE_ENTERED',
				courseId: courseId,
				slideId: slideId,
			}),
		}
	};
}

CommentSendForm.propTypes = {
	account: PropTypes.object,
	match: PropTypes.object,
	enterToCourse: PropTypes.func,
};

CommentSendForm = connect(CommentSendForm.mapStateToProps, CommentSendForm.mapDispatchToProps)(CommentSendForm);
export default CommentSendForm;