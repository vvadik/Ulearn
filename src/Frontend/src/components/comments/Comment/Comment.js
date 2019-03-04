import React, { Component } from 'react';
import moment from "moment";
import Hint from "@skbkontur/react-ui/components/Hint/Hint";
import Avatar from "../../common/Avatar/Avatar";
import CommentSendForm from "../CommentSendForm/CommentSendForm";
import Like from "./Like/Like";
import InstructorActions from "./Kebab/InstructorActions";
import Header from "./Header/Header";
import Marks from "./Marks/Marks";
import CommentActions from "./CommentActions/CommentActions";

import styles from "./Comment.less";

class Comment extends Component {
	constructor(props) {
		super(props);
		this.state = {
			showEditForm: false,
			sending: props.sending,
		};
	}

	render() {
		const {actions, children, comment, userRoles } = this.props;
		// const { comment } = commentState;

		return (
			<div className={styles.comment}>
				<Avatar user={comment.author} size='big' />
				<div className={styles.content}>
					<Header name={comment.author.visibleName}>
						<Like
							checked={comment.isLiked}
							count={comment.likesCount}
							onClick={() => actions.handleLikeClick(comment.id)} />
						<Marks
							isApproved={comment.isApproved}
							isCorrectAnswer={comment.isCorrectAnswer}
							isPinnedToTop={comment.isPinnedToTop} />
						{this.canModerateComments(userRoles, 'editPinAndRemoveComments') ?
							<InstructorActions
								handleShowEditForm={this.handleShowEditForm}
								commentId={comment.id}
								actions={actions}
								isApproved={comment.isApproved} />
							: null}
					</Header>
					<Hint pos="bottom" text={comment.publishTime} disableAnimations={false} useWrapper={false}>
						<div className={styles.timeSinceAdded}>
							{moment(comment.publishTime).fromNow()}
						</div>
					</Hint>
					{this.state.showEditForm ? this.renderEditCommentForm() : this.renderComment()}
					{children}
				</div>
			</div>
		);
	}

	renderComment() {
		const {comment, user, userRoles, getUserSolutionsUrl, hasReplyAction, actions, handleShowReplyForm} = this.props;
		const url = getUserSolutionsUrl(comment.author.id);
		return (
			<React.Fragment>
				<p className={styles.text}>
					<span dangerouslySetInnerHTML={{__html: comment.renderedText}} />
				</p>
				<CommentActions
					handleShowReplyForm={handleShowReplyForm}
					handleShowEditForm={this.handleShowEditForm}
					comment={comment}
					user={user}
					userRoles={userRoles}
					url={url}
					hasReplyAction={hasReplyAction}
					actions={actions}
					canModerateComments={this.canModerateComments} />
			</React.Fragment>
		)
	}

	renderEditCommentForm() {
		const {comment, actions } = this.props;

		return (
			<CommentSendForm
				commentId={comment.id}
				handleSubmit={actions.handleEditComment}
				handleShowForm={this.handleShowEditForm}
				submitTitle={'Сохранить'}
				onCancel={() => this.handleShowEditForm(false)}
				sending={this.state.sending} />
		)
	}

	canModerateComments = (role, accesses) => {
		return role.isSystemAdministrator || role.courseRole === 'CourseAdmin' ||
			(role.courseRole === 'Instructor' && role.courseAccesses.includes(accesses))
	};

	handleShowEditForm = (flag) => {
		this.setState({
			showEditForm: flag,
		})
	};
}

export default Comment;