import React, { Component } from 'react';
import PropTypes from "prop-types";
import { userRoles, user } from "../commonPropTypes";
import Tabs from "@skbkontur/react-ui/components/Tabs/Tabs";
import CommentsList from "../CommentsList/CommentsList";
import CommentPolicySettings from "./CommentPolicySettings";
import api from "../../../api";

import styles from './CommentsView.less';

const TABS = {
	allComments: 'allComments',
	instructorsComments: 'instructorsComments',
};

const ROLES = {
	systemAdministrator: 'isSystemAdministrator',
	courseAdmin: 'courseAdmin',
	instructor: 'instructor',
	student: 'student',
};

class CommentsView extends Component {
	constructor(props) {
		super(props);

		this.state = {
			instructorComments: [],
			activeTab: "allComments",
			openModal: false,
		};

		this.headerRef = React.createRef();
	}

	static defaultProps = {
		commentsApi: api.comments
	};

	componentDidMount() {
		const { courseId, slideId, userRoles } = this.props;
		if (this.isInstructor(userRoles))
			this.loadComments(courseId, slideId);
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
				{this.state.openModal && <CommentPolicySettings handleOpenModal={this.handleOpenModal} />}
				<div className={styles.commentsContainer} key={this.state.activeTab}>
					<CommentsList
						slideType={slideType}
						headerRef={this.headerRef}
						forInstructors={this.state.activeTab === TABS.instructorsComments}
						commentsApi={commentsApi}
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
		const {activeTab } = this.state;
		const commentsCount = this.state.instructorComments.length;

		return (
			<header className={styles.header} ref={this.headerRef}>
				<div className={styles.headerRow}>
					<h1 className={styles.headerName}>Комментарии</h1>
					{/*{this.isCourseAdmin(userRoles) &&*/}
					{/*<Button size="medium" icon={<Icon.Settings />}*/}
							{/*onClick={() => this.handleOpenModal(true)}>Настроить</Button>}*/}
				</div>
				{this.isInstructor(userRoles) &&
				<div className={styles.tabs}>
					<Tabs value={activeTab} onChange={this.handleTabChange}>
						<Tabs.Tab id={TABS.allComments}>К слайду</Tabs.Tab>
						<Tabs.Tab id={TABS.instructorsComments}>
							Для преподавателей
							{activeTab === TABS.allComments &&
							<span className={styles.commentsCount}>{commentsCount}</span>}
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
		this.setState({
			activeTab: id,
		});


		if (id === TABS.instructorsComments) {
			window.history.pushState ? window.history.pushState({}, '', window.location.pathname) :
			window.location.hash = '';
		}
	};

	handleOpenModal = (openModal) => {
		this.setState({
			openModal: openModal,
		})
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