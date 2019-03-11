import React, { Component } from 'react';
import PropTypes from "prop-types";
import { userRoles, user } from "../commonPropTypes";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Icon from "@skbkontur/react-icons";
import Tabs from "@skbkontur/react-ui/components/Tabs/Tabs";
import CommentsList from "../CommentsList/CommentsList";
import Error404 from "../../common/Error/Error404";
import CommentPolicySettings from "./CommentPolicySettings";

import styles from './CommentsWrapper.less';

class CommentsWrapper extends Component {
	constructor(props) {
		super(props);

		this.state = {
			comments: [],
			activeTab: "allComments",
			openModal: false,
			status: '',
		};
	}

	componentDidMount() {
		this.loadComments(this.props.courseId, this.props.slideId, true);
	};

	loadComments = (courseId, slideId, forInstructors) => {
		this.props.commentsApi.getComments(courseId, slideId, forInstructors)
		.then(json => {
			let comments = json.topLevelComments;
			this.setState({
				comments: comments,
			});
		})
		.catch(() => {
			this.setState({
				status: 'error',
			});
		})
	};

	render() {
		const {user, userRoles, courseId, slideId, commentsApi} = this.props;
		const {activeTab} = this.state;
		const forInstructors = activeTab === 'commentsForInstructors';

		if (this.state.status === "error") {
			return <Error404 />;
		}

		return (
			<div className={styles.wrapper}>
				{this.renderHeader()}
				{this.state.openModal && <CommentPolicySettings handleOpenModal={this.handleOpenModal} />}
				<div className={styles.commentsContainer}>
					<CommentsList
						forInstructors={forInstructors}
						commentsApi={commentsApi}
						user={user}
						userRoles={userRoles}
						slideId={slideId}
						courseId={courseId}>
						{activeTab === "allComments" &&
						<p>
							К этому слайду ещё нет коммаентариев. Вы можете начать беседу со студентами,
							добавив комментарий.
						</p>}
						{activeTab === "commentsForInstructors" &&
						<p>
							К этому слайду нет комментариев преподавателей. Вы можете начать беседу с преподавателями,
							добавив комментарий.
						</p>}
					</CommentsList>
				</div>
			</div>
		)
	};

	renderHeader() {
		const {userRoles} = this.props;
		const commentsCount = this.state.comments.length;
		return (
			<header className={styles.header}>
				<div className={styles.headerRow}>
					<h1 className={styles.headerName}>Комментарии</h1>
					{(userRoles.isSystemAdministrator || userRoles.courseRole === 'CourseAdmin') &&
					<Button size="medium" icon={<Icon.Settings />}
							onClick={() => this.handleOpenModal(true)}>Настроить</Button>}
				</div>
				{(userRoles.isSystemAdministrator || userRoles.courseRole === 'CourseAdmin' ||
					userRoles.courseRole === 'Instructor') &&
				<Tabs value={this.state.activeTab} onChange={this.handleTabChange}>
					<Tabs.Tab id="allComments">К слайду</Tabs.Tab>
					<Tabs.Tab id="commentsForInstructors">
						Для преподавателей
						<span className={styles.commentsCount}>{commentsCount}</span>
					</Tabs.Tab>
				</Tabs>}
			</header>
		)
	};

	handleTabChange = (_, id) => {
		this.setState({
			activeTab: id,
		});
	};

	handleOpenModal = (flag) => {
		this.setState({
			openModal: flag,
		})
	};
}

CommentsWrapper.propTypes = {
	user: user,
	userRoles: userRoles,
	courseId: PropTypes.string,
	slideId: PropTypes.string,
	commentsApi: PropTypes.objectOf(PropTypes.func),
};

export default CommentsWrapper;