import React, { Component } from "react";
import PropTypes from "prop-types";
import { userRoles, userType, comment, commentStatus, commentPolicy } from "../commonPropTypes";
import moment from "moment";
import Link from "@skbkontur/react-ui/components/Link/Link";
import Avatar from "../../common/Avatar/Avatar";
import CommentSendForm from "../CommentSendForm/CommentSendForm";
import Like from "./Like/Like";
import KebabActions from "./Kebab/KebabActions";
import Header from "./Header/Header";
import Marks from "./Marks/Marks";
import CommentActions from "./CommentActions/CommentActions";
import scrollToView from "../../../utils/scrollToView";
import { ACCESSES, ROLES } from "../../../consts/general";

import styles from "./Comment.less";
import Hint from "@skbkontur/react-ui/components/Hint/Hint";

class Comment extends Component {
	ref = React.createRef();

	componentDidMount() {
		if (window.location.hash === `#comment-${this.props.comment.id}`) {
			scrollToView(this.ref);
		}
	};

	render() {
		const {children, commentEditing, comment, userRoles, user} = this.props;
		const canViewProfiles = (user.systemAccesses && user.systemAccesses.includes(ACCESSES.viewAllProfiles)) ||
			userRoles.isSystemAdministrator;
		const profileUrl = `${window.location.origin}/Account/Profile?userId=${comment.author.id}`;

		return (
			<div className={styles.comment} ref={this.ref}>
				<span className={styles.commentAnchor}  id={`comment-${this.props.comment.id}`} />
				{canViewProfiles ? <Link href={profileUrl}><Avatar user={comment.author} size="big" /></Link> :
					<Avatar user={comment.author} size="big" />}
				<div className={styles.content}>
					{this.renderHeader(profileUrl, canViewProfiles)}
					<div className={styles.timeSinceAdded}>
						<Hint
							pos="right middle"
							text={`${moment(comment.publishTime).local().format('YYYY-MM-DD HH:mm:ss')}`}>
							{moment(comment.publishTime).fromNow()}
						</Hint>
					</div>
					{comment.id === commentEditing.commentId ? this.renderEditCommentForm() : this.renderComment()}
					{children}
				</div>
			</div>
		);
	}

	renderHeader(profileUrl, canViewProfiles) {
		const {actions, comment, userRoles, user, slideType, getUserSolutionsUrl} = this.props;
		const canSeeKebabActions = user.isAuthenticated && (user.id === comment.author.id ||
			this.canModerateComments(userRoles, ACCESSES.editPinAndRemoveComments) ||
			this.canModerateComments(userRoles, ACCESSES.viewAllStudentsSubmissions));

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
					canViewStudentsGroup={this.canViewStudentsGroup}
					comment={comment} />
				{canSeeKebabActions &&
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
		const {comment, user, userRoles, hasReplyAction, actions, slideType,
			getUserSolutionsUrl} = this.props;
		return (
			<>
				<p className={styles.text}>
					<span className={styles.textFromServer} dangerouslySetInnerHTML={{__html: comment.renderedText}} />
				</p>
				{user.isAuthenticated &&
				<CommentActions
					slideType={slideType}
					comment={comment}
					canReply={this.canReply(userRoles)}
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
				handleCancel={() => actions.handleShowEditForm(null)} />
		)
	}

	canModerateComments = (role, accesses) => {
		return role.isSystemAdministrator || role.courseRoles === ROLES.courseAdmin ||
			(role.courseRoles === ROLES.instructor && role.courseAccesses.includes(accesses))
	};

	canReply = (role) => {
		return 	(role.courseRoles === ROLES.student && !this.props.onlyInstructorsCanReply) ||
			(role.isSystemAdministrator || role.courseRoles === ROLES.courseAdmin ||
			role.courseRoles === ROLES.instructor);
	};

	canViewStudentsGroup = () => {
		const {userRoles, user} = this.props;

		return (user.systemAccesses && user.systemAccesses.includes(ACCESSES.viewAllGroupMembers)) ||
			(userRoles.courseAccesses && userRoles.courseAccesses.includes(ACCESSES.viewAllGroupMembers)) ||
			userRoles.isSystemAdministrator;
	};
}

Comment.propTypes = {
	user: userType.isRequired,
	userRoles: userRoles.isRequired,
	comment: comment.isRequired,
	onlyInstructorsCanReply: PropTypes.bool,
	actions: PropTypes.objectOf(PropTypes.func),
	children: PropTypes.array,
	getUserSolutionsUrl: PropTypes.func,
	commentEditing: commentStatus,
	hasReplyAction: PropTypes.bool,
	slideType: PropTypes.string,
};

export default Comment;