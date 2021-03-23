import React, { Component } from "react";
import moment from "moment";

import { Link, Hint } from "ui";
import Avatar from "../../common/Avatar/Avatar";
import CommentSendForm from "../CommentSendForm/CommentSendForm";
import Like from "./Like/Like";
import KebabActions from "./Kebab/KebabActions";
import Header from "./Header/Header";
import Marks from "./Marks/Marks";
import CommentActions from "./CommentActions/CommentActions";

import { UserInfo, } from "src/utils/courseRoles";
import scrollToView from "src/utils/scrollToView";
import { convertDefaultTimezoneToLocal } from "src/utils/momentUtils";

import { CommentStatus } from "src/consts/comments";
import { CourseAccessType, CourseRoleType, SystemAccessType } from "src/consts/accessType";
import { Comment as CommentType, CommentPolicy } from "src/models/comments";
import { SlideType } from "src/models/slide";
import { ActionsType } from "../CommentsList/CommentsList";

import styles from "./Comment.less";


interface Props {
	courseId: string;
	slideType: SlideType;

	user: UserInfo;

	comment: CommentType;
	commentEditing: CommentStatus;
	commentPolicy: CommentPolicy | null;

	hasReplyAction: boolean;
	isSlideReady: boolean;

	actions: ActionsType;
	getUserSolutionsUrl: (userId: string) => string;

	children: React.ReactNode;
}

interface State {
	isApproved: boolean;
}

class Comment extends Component<Props, State> {
	private ref: React.RefObject<HTMLDivElement> = React.createRef();

	constructor(props: Props) {
		super(props);

		this.state = {
			isApproved: props.comment.isApproved
		};
	}


	componentDidMount(): void {
		this.scrollToComment();
	}

	componentDidUpdate(prevProps: Props): void {
		const { isSlideReady, } = this.props;

		if(isSlideReady && !prevProps.isSlideReady) {
			this.scrollToComment();
		}
	}

	scrollToComment = (): void => {
		const { isSlideReady, } = this.props;

		if(isSlideReady && window.location.hash === `#comment-${ this.props.comment.id }`) {
			scrollToView(this.ref);
		}
	};

	render(): React.ReactNode {
		const { children, commentEditing, comment, user, } = this.props;
		const canViewProfiles = (user.systemAccesses.includes(SystemAccessType.viewAllProfiles)) ||
			user.isSystemAdministrator;
		const profileUrl = `${ window.location.origin }/Account/Profile?userId=${ comment.author.id }`;

		return (
			<div className={ `${ styles.comment } ${ !this.state.isApproved ? styles.isNotApproved : "" }` }
				 ref={ this.ref }>
				<span className={ styles.commentAnchor } id={ `comment-${ this.props.comment.id }` }/>
				{ canViewProfiles ? <Link href={ profileUrl }><Avatar user={ comment.author } size="big"/></Link> :
					<Avatar user={ comment.author } size="big"/> }
				<div className={ styles.content }>
					{ this.renderHeader(profileUrl, canViewProfiles) }
					<div className={ styles.timeSinceAdded }>
						<Hint
							pos="right middle"
							text={ `${ moment(convertDefaultTimezoneToLocal(comment.publishTime)).format(
								"DD MMMM YYYY в HH:mm") }` }>
							{ moment(convertDefaultTimezoneToLocal(comment.publishTime)).startOf("minute").fromNow() }
						</Hint>
					</div>
					{ comment.id === commentEditing.commentId ? this.renderEditCommentForm() : this.renderComment() }
					{ children }
				</div>
			</div>
		);
	}

	renderHeader(profileUrl: string, canViewProfiles: boolean): React.ReactElement {
		const { actions, comment, user, slideType, getUserSolutionsUrl, courseId } = this.props;
		const canSeeKebabActions = user.id && (user.id === comment.author.id ||
			this.canModerateComments(user, CourseAccessType.editPinAndRemoveComments) ||
			this.canModerateComments(user, CourseAccessType.viewAllStudentsSubmissions));

		return (
			<Header
				profileUrl={ profileUrl }
				canViewProfiles={ canViewProfiles }
				name={ comment.author.visibleName }>
				<Like
					canLike={ user.isAuthenticated }
					isLiked={ comment.isLiked }
					count={ comment.likesCount }
					onClick={ this.handleLikeClick }/>
				<Marks
					authorGroups={ comment.authorGroups }
					courseId={ courseId }
					comment={ comment }/>
				{ canSeeKebabActions &&
				<KebabActions
					user={ user }
					url={ getUserSolutionsUrl(comment.author.id) }
					canModerateComments={ this.canModerateComments }
					slideType={ slideType }
					comment={ comment }
					actions={ actions }
					handleCommentBackGround={ this.handleCommentBackground }/> }
			</Header>
		);
	}

	handleLikeClick = (): void => {
		const { actions, comment, } = this.props;

		actions.handleLikeClick(comment.id, comment.isLiked);
	};

	renderComment(): React.ReactNode {
		const {
			comment, user, hasReplyAction, actions, slideType,
			getUserSolutionsUrl
		} = this.props;

		return (
			<>
				<p className={ styles.text }>
					<span className={ styles.textFromServer }
						  dangerouslySetInnerHTML={ { __html: comment.renderedText } }/>
				</p>
				{ user.id &&
				<CommentActions
					slideType={ slideType }
					comment={ comment }
					canReply={ this.canReply(user) }
					user={ user }
					url={ getUserSolutionsUrl(comment.author.id) }
					hasReplyAction={ hasReplyAction }
					actions={ actions }
					canModerateComments={ this.canModerateComments }/> }
			</>
		);
	}

	renderEditCommentForm(): React.ReactElement {
		const { comment, actions, commentEditing } = this.props;
		const focusedEditForm = { inEditForm: comment.id === commentEditing.commentId, };

		return (
			<CommentSendForm
				isShowFocus={ focusedEditForm }
				commentId={ comment.id }
				handleSubmit={ actions.handleEditComment }
				text={ comment.text }
				submitTitle={ "Сохранить" }
				sending={ commentEditing.sending }
				handleCancel={ this.handleShowEditFormCancelClick }/>
		);
	}

	handleShowEditFormCancelClick = (): void => {
		this.props.actions.handleShowEditForm(null);
	};

	canModerateComments = (role: UserInfo, accesses: CourseAccessType): boolean => {
		return role.isSystemAdministrator || role.courseRole === CourseRoleType.courseAdmin ||
			(role.courseRole === CourseRoleType.instructor && role.courseAccesses.includes(accesses));
	};

	canReply = (role: UserInfo): boolean => {
		const { commentPolicy } = this.props;
		if(!commentPolicy) {
			return false;
		}

		return (commentPolicy.areCommentsEnabled && ((role.courseRole === CourseRoleType.student || role.courseRole === null ||
			role.courseRole.length === 0) || role.isSystemAdministrator || role.courseRole === CourseRoleType.courseAdmin ||
			role.courseRole === CourseRoleType.instructor));
	};

	canViewStudentsGroup = (): boolean => {
		const { user } = this.props;

		return (user.systemAccesses && user.systemAccesses.includes(SystemAccessType.viewAllGroupMembers)) ||
			user.isSystemAdministrator;
	};

	handleCommentBackground = (commentId: string, isApproved: boolean): void => {
		this.setState({
			isApproved,
		});
	};
}

export default Comment;
