import React, { Component } from 'react';
import PropTypes from "prop-types";
import { userRoles, userType, comment, commentStatus } from "../commonPropTypes";
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

	render() {
		const { actions, children, commentEditing, comment, userRoles } = this.props;

		return (
			<div className={styles.comment}>
				<Avatar user={comment.author} size='big' />
				<div className={styles.content}>
					<Header name={comment.author.visibleName}>
						<Like
							checked={comment.isLiked}
							count={comment.likesCount}
							onClick={() => actions.handleLikeClick(comment.id, comment.isLiked)} />
						<Marks
							isApproved={comment.isApproved}
							isCorrectAnswer={comment.isCorrectAnswer}
							isPinnedToTop={comment.isPinnedToTop} />
						{this.canModerateComments(userRoles, 'editPinAndRemoveComments') ?
							<InstructorActions
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
					{comment.id === commentEditing.commentId ? this.renderEditCommentForm() : this.renderComment()}
					{children}
				</div>
			</div>
		);
	}

	renderComment() {
		const {comment, user, userRoles, getUserSolutionsUrl, hasReplyAction, actions} = this.props;
		const url = getUserSolutionsUrl(comment.author.id);
		return (
			<React.Fragment>
				<p className={styles.text}>
					<span dangerouslySetInnerHTML={{__html: comment.renderedText}} />
				</p>
				<CommentActions
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
		const { comment, actions, commentEditing } = this.props;

		return (
			<CommentSendForm
				commentId={comment.id}
				handleSubmit={actions.handleEditComment}
				text={comment.text}
				submitTitle={'Сохранить'}
				sending={commentEditing.sending}
				onCancel={() => actions.handleShowEditForm(null)}
			/>
		)
	}

	canModerateComments = (role, accesses) => {
		return role.isSystemAdministrator || role.courseRole === 'CourseAdmin' ||
			(role.courseRole === 'Instructor' && role.courseAccesses.includes(accesses))
	};
}

Comment.propTypes = {
	user: userType.isRequired,
	userRoles: userRoles.isRequired,
	comment: comment.isRequired,
	actions: PropTypes.objectOf(PropTypes.func),
	children: PropTypes.array,
	getUserSolutionsUrl: PropTypes.func,
	commentEditing: commentStatus,
	hasReplyAction: PropTypes.bool,
};

export default Comment;