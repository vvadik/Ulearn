import React, { Component } from "react";
import PropTypes from "prop-types";
import { userRoles, userType, comment, commentStatus } from "../commonPropTypes";
import moment from "moment";
import Link from "@skbkontur/react-ui/components/Link/Link";
import Avatar from "../../common/Avatar/Avatar";
import CommentSendForm from "../CommentSendForm/CommentSendForm";
import Like from "./Like/Like";
import KebabActions from "./Kebab/KebabActions";
import Header from "./Header/Header";
import Marks from "./Marks/Marks";
import CommentActions from "./CommentActions/CommentActions";
import scrollIntoView from "./../../../utils/scrollIntoView";
import { ACCESSES, ROLES } from "../../../consts/general";

import styles from "./Comment.less";

class Comment extends Component {
	ref = React.createRef();

	componentDidMount() {
		if (window.location.hash === `#comment-${this.props.comment.id}`) {
			scrollIntoView(this.ref);
		}
	};

	render() {
		const {children, commentEditing, comment, userRoles, user} = this.props;
		const canViewProfiles = (user.systemAccesses && user.systemAccesses.includes(ACCESSES.viewAllProfiles)) ||
			userRoles.isSystemAdministrator;

		const profileUrl = `${window.location.origin}/Account/Profile?userId=${comment.author.id}`;

		return (
			<div className={styles.comment} ref={this.ref}>
				<a className={styles.commentAnchor} name={`comment-${this.props.comment.id}`} />
				{canViewProfiles ? <Link href={profileUrl}><Avatar user={comment.author} size="big" /></Link> :
					<Avatar user={comment.author} size="big" />}
				<div className={styles.content}>
					{this.renderHeader(profileUrl, canViewProfiles)}
					<div className={styles.timeSinceAdded}>
						{moment(comment.publishTime).fromNow()}
					</div>
					{comment.id === commentEditing.commentId ? this.renderEditCommentForm() : this.renderComment()}
					{children}
				</div>
			</div>
		);
	}

	renderHeader(profileUrl, canViewProfiles) {
		const {actions, comment, userRoles, user, slideType, getUserSolutionsUrl} = this.props;
		const canViewStudentsGroup = (user.systemAccesses &&
			user.systemAccesses.includes(ACCESSES.viewAllGroupMembers)) ||
			(userRoles.courseAccesses && userRoles.courseAccesses.includes(ACCESSES.viewAllGroupMembers)) ||
			userRoles.isSystemAdministrator;

		return (
			<Header
				profileUrl={profileUrl}
				canViewProfiles={canViewProfiles}
				name={comment.author.visibleName}>
				<Like
					isLiked={comment.isLiked}
					count={comment.likesCount}
					onClick={() => actions.handleLikeClick(comment.id, comment.isLiked)} />
				<Marks
					canViewStudentsGroup={canViewStudentsGroup}
					comment={comment} />
				{(user.isAuthenticated && (user.id === comment.author.id ||
					this.canModerateComments(userRoles, ACCESSES.editPinAndRemoveComments))) &&
				<KebabActions
					user={user}
					url={getUserSolutionsUrl(comment.author.id)}
					canModerateComments={this.canModerateComments}
					userRoles={userRoles}
					slideType={slideType}
					comment={comment}
					actions={actions} />}
			</Header>
		)
	}

	renderComment() {
		const {comment, user, userRoles, hasReplyAction, actions, slideType, getUserSolutionsUrl} = this.props;
		return (
			<>
				<p className={styles.text}>
					<span dangerouslySetInnerHTML={{__html: comment.renderedText}} />
				</p>
				{user.isAuthenticated &&
				<CommentActions
					slideType={slideType}
					comment={comment}
					user={user}
					url={getUserSolutionsUrl(comment.author.id)}
					userRoles={userRoles}
					hasReplyAction={hasReplyAction}
					actions={actions}
					canModerateComments={this.canModerateComments} />}
			</>
		)
	}

	renderEditCommentForm() {
		const {comment, actions, commentEditing} = this.props;

		return (
			<CommentSendForm
				commentId={comment.id}
				handleSubmit={actions.handleEditComment}
				text={comment.text}
				submitTitle={"Сохранить"}
				sending={commentEditing.sending}
				onCancel={() => actions.handleShowEditForm(null)} />
		)
	}

	canModerateComments = (role, accesses) => {
		return role.isSystemAdministrator || role.courseRole === ROLES.courseAdmin ||
			(role.courseRole === ROLES.instructor && role.courseAccesses.includes(accesses))
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
	slideType: PropTypes.string,
};

export default Comment;