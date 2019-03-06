import React, { Component } from 'react';
import PropTypes from "prop-types";
import { userRoles, user } from "../commonPropTypes";
import api from "../../../api";
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
			loadingComments: false,
			loadedComments: false,
			openModal: false,
			status: '',
		};
	}

	// get courseId() {
	// 	return this.props.match.params.courseId.toLowerCase();
	// }
	//
	// get slideId() {
	// 	return this.props.match.params.courseId.slideId.toLowerCase();
	// }

	componentDidMount() {
		this.loadComments(this.courseId, this.slideId, false);
	};

	loadComments = (courseId, slideId, isForInstructor) => {
		const { loadedComments, loadingComments } = this.state;

		if (loadedComments || loadingComments) {
			return;
		}

		this.setState({
			loadingComments: true,
		});

		api.comments.apiRequests.getComments(courseId, slideId, isForInstructor)
		.then(json => {
			let comments = json.comments;
			this.setState({
				loadedComments: true,
				comments: comments,
			});
		})
		.catch(() => {
			this.setState({
				status: 'error',
			});
		})
		.finally(() =>
			this.setState({
				loadingComments: false,
			})
		);
	};

	render() {
		const { user, userRoles } = this.props;
		const { comments, activeTab } = this.state;

		// if (this.state.status === "error") {
		// 	return <Error404 />;
		// }

		return (
			<div className={styles.wrapper}>
				{ this.renderHeader() }
				{this.state.openModal && <CommentPolicySettings handleOpenModal={this.handleOpenModal} />}
				<div className={styles.commentsContainer}>
					<CommentsList comments={comments} user={user} userRoles={userRoles} slideId={this.slideId} courseId={this.courseId}>
						{ activeTab === "allComments" &&
						<p>
							К этому слайду ещё нет коммаентариев. Вы можете начать беседу со студентами,
							добавив комментарий.
						</p> }
						{ activeTab === "commentsForInstructors" &&
						<p>
							К этому слайду нет комментариев преподавателей. Вы можете начать беседу с преподавателями,
							добавив комментарий.
						</p> }
					</CommentsList>
				</div>
			</div>
		)
	};

	renderHeader() {
		const {userRoles} = this.props;
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
					<Tabs.Tab id="commentsForInstructors">Для преподавателей</Tabs.Tab>
				</Tabs>}
			</header>
		)
	};

	handleTabChange = (_, id) => {
		this.setState({
			activeTab: id,
		});

		if (id === "allComments") {
			this.loadComments(this.courseId, this.slideId, false);
		} else {
			this.loadComments(this.courseId, this.slideId, true);
		}
	};

	handleOpenModal = (flag) => {
		this.setState({
			openModal: flag,
		})
	};
}

CommentsWrapper.propTypes = {
	courseId: PropTypes.string,
	slideId: PropTypes.string,
	user: user,
	userRoles: userRoles,
};

export default CommentsWrapper;