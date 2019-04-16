import React, { Component } from "react";
import PropTypes from "prop-types";
import { userRoles, user } from "../commonPropTypes";
import api from "../../../api";
import { TABS, ROLES } from "../../../consts/general";
import Tabs from "@skbkontur/react-ui/components/Tabs/Tabs";
import CommentsList from "../CommentsList/CommentsList";

import styles from "./CommentsView.less";

class CommentsView extends Component {
	constructor(props) {
		super(props);

		this.state = {
			instructorComments: [],
			commentPolicy: {},
			activeTab: TABS.allComments,
			openModal: false,
		};

		this.headerRef = React.createRef();
	}

	static defaultProps = {
		commentsApi: api.comments
	};

	componentDidMount() {
		const {courseId, slideId, userRoles} = this.props;

		this.loadCommentPolicy(courseId);

		if (this.isInstructor(userRoles))
			this.loadComments(courseId, slideId);
	};

	loadCommentPolicy = (courseId) => {
		this.props.commentsApi.getCommentPolicy(courseId)
			.then(commentPolicy => {
				this.setState ({
					commentPolicy: commentPolicy,
				})
			})
			.catch(console.error);
	};

	loadComments = (courseId, slideId) => {
		this.props.commentsApi.getComments(courseId, slideId, true)
		.then(json => {
			let comments = json.topLevelComments;
			this.setState({
				instructorComments: comments,
			});
		})
		.catch(console.error);
	};

	render() {
		const {user, userRoles, courseId, slideId, slideType, commentsApi} = this.props;

		return (
			<div className={styles.wrapper}>
				{this.renderHeader()}
				<div className={styles.commentsContainer} key={this.state.activeTab}>
					<CommentsList
						slideType={slideType}
						headerRef={this.headerRef}
						forInstructors={this.state.activeTab === TABS.instructorsComments}
						commentsApi={commentsApi}
						commentPolicy={this.state.commentPolicy}
						user={user}
						userRoles={userRoles}
						slideId={slideId}
						courseId={courseId}>
					</CommentsList>
				</div>
			</div>
		)
	};

	renderHeader() {
		const {userRoles} = this.props;
		const {activeTab} = this.state;
		const commentsCount = this.state.instructorComments.length;

		return (
			<header className={styles.header} ref={this.headerRef}>
				<div className={styles.headerRow}>
					<h1 className={styles.headerName}>Комментарии</h1>
				</div>
				{this.isInstructor(userRoles) &&
				<div className={styles.tabs}>
					<Tabs value={activeTab} onChange={this.handleTabChange}>
						<Tabs.Tab id={TABS.allComments}>К слайду</Tabs.Tab>
						<Tabs.Tab id={TABS.instructorsComments}>
							Для преподавателей
							{(activeTab === TABS.allComments && commentsCount > 0) &&
							<span className={styles.commentsAmount}>{commentsCount}</span>}
						</Tabs.Tab>
					</Tabs>
				</div>}
			</header>
		)
	};

	isCourseAdmin(userRoles) {
		return userRoles.isSystemAdministrator ||
			userRoles.courseRole === ROLES.courseAdmin;
	}

	isInstructor(userRoles) {
		return this.isCourseAdmin(userRoles) ||
			userRoles.courseRole === ROLES.instructor;
	}

	handleTabChange = (_, id) => {
		if (id !== this.state.activeTab) {
			this.setState({
				activeTab: id,
			});

			window.history.pushState ? window.history.pushState({}, "", window.location.pathname) :
				window.location.hash = "";
		}
	};
}

CommentsView.propTypes = {
	user: user.isRequired,
	userRoles: userRoles.isRequired,
	courseId: PropTypes.string.isRequired,
	slideId: PropTypes.string.isRequired,
	slideType: PropTypes.string.isRequired,
};

export default CommentsView;