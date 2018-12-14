import React, { Component } from "react";
import { withRouter } from "react-router-dom";
import { Helmet } from "react-helmet";
import connect from "react-redux/es/connect/connect";
import PropTypes from 'prop-types';
import api from "../../../api";
import GroupList from "../../../components/groups/GroupMainPage/GroupList/GroupList";
import GroupHeader from "../../../components/groups/GroupMainPage/GroupHeader/GroupHeader";

import styles from "./mainPage.less";

class AbstractPage extends Component {
 // TODO: выяснить у Андрея, зачем оно. И реализоваьть или удалить.
}

class GroupListPage extends AbstractPage {
	constructor(props) {
		super(props);
		this.state = {
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
		this.loadingActiveGroups();
	};

	loadingActiveGroups = () => {
		let courseId = this.props.match.params.courseId;
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

	loadingArchivedGroups = () => {
		let courseId = this.props.match.params.courseId;
		const { loadingArchived, loadedArchived } = this.state;

		if (loadedArchived || loadingArchived) {
			return;
		}

		this.setState({
			loadingArchived: true,
		});

		api.groups.getCourseArchiveGroups(courseId)
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
		let courseId = this.props.match.params.courseId;
		const courseById = this.props.courses.courseById;
		const course = courseById[courseId];
		if (course === undefined) {
			return;
		}

		return (
			<div className={styles.wrapper}>
				<Helmet>
					<title>Группы в курсе {course.title.toLowerCase()}</title>
				</Helmet>
				<div className={styles["content-wrapper"]}>
					<GroupHeader
						onTabChange={this.onTabChange}
						filter={this.state.filter}
						courseId={courseId}
						createGroup={this.createGroup}
						copyGroup={this.copyGroup}
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
			this.loadingActiveGroups();
		} else {
			this.loadingArchivedGroups();
		}
	};

	createGroup = async (groupId) => {
		let courseId = this.props.match.params.courseId;
		const newGroup = await api.groups.getGroup(groupId);
		const groups = this.filteredGroups;

		this.setState({
			groups: [newGroup, ...groups],
		});

		this.props.history.push(`/${courseId}/groups/${groupId}`);
	};

	copyGroup = async (groupId) => {
		let courseId = this.props.match.params.courseId;
		const { groups } = this.state;
		const copyGroup = await api.groups.getGroup(groupId);

		this.setState({
			groups: [copyGroup, ...groups],
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
			.then(response => response)
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
			.then(response => response)
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
}

GroupListPage.propTypes = {
	match: PropTypes.object,
	courses: PropTypes.object,
};

GroupListPage = connect(GroupListPage.mapStateToProps)(GroupListPage);

export default withRouter(GroupListPage);