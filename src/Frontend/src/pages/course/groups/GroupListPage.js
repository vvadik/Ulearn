import React, { Component } from "react";
import { withRouter } from "react-router-dom";
import { Helmet } from "react-helmet";
import connect from "react-redux/es/connect/connect";
import PropTypes from 'prop-types';
import api from "../../../api";
import GroupList from "../../../components/groups/GroupMainPage/GroupList/GroupList";
import GroupHeader from "../../../components/groups/GroupMainPage/GroupHeader/GroupHeader";

import styles from "./groupListPage.less";

class AbstractPage extends Component {
 // TODO: выяснить у Андрея, зачем оно. И реализоваьть или удалить.
}

class GroupListPage extends AbstractPage {
	constructor(props) {
		super(props);
		this.state = {
			courseId: this.props.match.params.courseId.toLowerCase(),
			groups: [],
			archiveGroups: [],
			filter: "active",
			loadingArchived: false,
			loadingActive: false,
			loadedArchived: false,
			loadedActive: false,
		}
	};

	componentDidMount() {
		let { courseId } = this.state;

		this.loadActiveGroups(courseId);
		this.props.enterToCourse(courseId);
	};

	loadActiveGroups = (courseId) => {
		const { loadingActive, loadedActive } = this.state;

		if (loadedActive || loadingActive) {
			return;
		}

		this.setState({
			loadingActive: true,
		});

		api.groups.getCourseGroups(courseId)
			.then(json => {
			let groups = json.groups;
			this.setState({
				loadingActive: false,
				loadedActive: true,
				groups,
			});
		}).catch(console.error);
	};

	loadArchivedGroups = (courseId) => {
		const { loadingArchived, loadedArchived } = this.state;

		if (loadedArchived || loadingArchived) {
			return;
		}

		this.setState({
			loadingArchived: true,
		});

		api.groups.getCourseArchivedGroups(courseId)
			.then(json => {
			let archiveGroups = json.groups;
			this.setState({
				loadingArchived: false,
				loadedArchived: true,
				archiveGroups,
			});
		}).catch(console.error);
	};

	render() {
		let { courseId } = this.state;
		const courseById = this.props.courses.courseById;
		const course = courseById[courseId];
		if (course === undefined) {
			return;
		}

		return (
			<div className={styles.wrapper}>
				<Helmet defer={false}>
					<title>Группы в курсе {course.title.toLowerCase()}</title>
				</Helmet>
				<div className={styles["content-wrapper"]}>
					<GroupHeader
						onTabChange={this.onTabChange}
						filter={this.state.filter}
						course={course}
						addGroup={this.addGroup}
						groups={this.state.groups}
					/>
					<GroupList
						courseId={courseId}
						groups={this.filteredGroups}
						deleteGroup={this.deleteGroup}
						toggleArchived={this.toggleArchived}
						loading={this.loading}
					/>
				</div>
			</div>
		)
	};

	onTabChange = (id) => {
		this.setState({
			filter: id,
		});

		if (id === "active") {
			this.loadActiveGroups(this.state.courseId);
		} else {
			this.loadArchivedGroups(this.state.courseId);
		}
	};

	addGroup = async (groupId) => {
		let courseId = this.state.courseId;
		const groups = this.filteredGroups;

		const newGroup = await api.groups.getGroup(groupId);

		this.setState({
			groups: [newGroup, ...groups],
		});

		this.props.history.push(`/${courseId}/groups/${groupId}`);
	};

	get filteredGroups() {
		if (this.state.filter === "archived") {
			return this.state.archiveGroups;
		} else {
			return this.state.groups;
		}
	};

	deleteGroup = (group, groupsName) => {
		api.groups.deleteGroup(group.id)
			.catch(console.error);

		const updateGroups = this.state[groupsName].filter(g => group.id !== g.id);

		this.setState({
			[groupsName]: updateGroups,
		});
	};

	toggleArchived = (group, isArchived) => {
		const newSettings = {
			is_archived: isArchived
		};

		api.groups.saveGroupSettings(group.id, newSettings)
			.catch(console.error);

		group = { ...group, ...newSettings };

		if (isArchived) {
			this.moveGroup(group, 'groups', 'archiveGroups');
		} else {
			this.moveGroup(group, 'archiveGroups', 'groups');
		}
	};

	moveGroup = (group, moveFrom, moveTo) => {
		const groupsMoveFrom = this.state[moveFrom].filter(g => group.id !== g.id);
		const groupsMoveTo = [group, ...this.state[moveTo]].sort((a, b) => a.name.localeCompare(b.name));

		this.setState({
			[moveFrom]: groupsMoveFrom,
			[moveTo]: groupsMoveTo
		});
	};

	get loading() {
		if (this.state.filter === "archived") {
			return this.state.loadingArchived;
		} else {
			return this.state.loadingActive;
		}
	};

	static mapStateToProps(state) {
		return {
			courses: state.courses,
		}
	}

	static mapDispatchToProps(dispatch) {
		return {
			enterToCourse: (courseId) => dispatch({
				type: 'COURSES__COURSE_ENTERED',
				courseId: courseId
			}),
		}
	}
}

GroupListPage.propTypes = {
	history: PropTypes.object,
	match: PropTypes.object,
	courses: PropTypes.object,
	enterToCourse: PropTypes.func,
};

GroupListPage = connect(GroupListPage.mapStateToProps, GroupListPage.mapDispatchToProps)(GroupListPage);

export default withRouter(GroupListPage);