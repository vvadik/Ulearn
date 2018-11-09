import React, { Component } from "react";
import api from "../../../api";
import PropTypes from 'prop-types';
import GroupsList from "../../../components/groups/GroupMainPage/GroupList/GroupsList";
import GroupHeader from "../../../components/groups/GroupMainPage/GroupHeader/GroupHeader";

import "./groupsPage.less";
import connect from "react-redux/es/connect/connect";

class AbstractPage extends Component {
 // TODO: выяснить у Андрея, зачем оно. И реализоваьть или удалить.
}

class GroupsPage extends AbstractPage {
	constructor(props) {
		super(props);
		this.state = {
			groups: [],
			archiveGroups: [],
			filter: "active",
			loading: true,
			loadingArchived: false,
			loadingActive: false
		}
	};

	componentDidMount() {
		this.loadingActiveGroups();
	};

	render() {
		let courseId = this.props.match.params.courseId;
		return (
			<div className="wrapper">
				<div className="content-wrapper">
					<GroupHeader
						onTabChange={this.onTabChange}
						filter={this.state.filter}
						courseId={courseId}
						createGroup={this.createGroup}
						copyGroup={this.copyGroup}
						groups={this.state.groups}
						loading={this.state.loading}
					/>
					<GroupsList
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

	loadingArchivedGroups = () => {
		let courseId = this.props.match.params.courseId;
		const { archiveGroups, loadingArchived } = this.state;
		if (archiveGroups.length !== 0 || loadingArchived) {
			return;
		}

		this.setState({
			// loadingArchived: true,
		});

		api.groups.getCourseArchiveGroups(courseId).then(json => {
			let archiveGroups = json.groups;
			this.setState({
				loadingArchived: false,
				archiveGroups: archiveGroups
			});
		}).catch(console.error);
	};

	loadingActiveGroups = () => {
		let courseId = this.props.match.params.courseId;
		const { groups, loadingActive } = this.state;
		if (groups.length !== 0 || loadingActive) {
			return;
		}

		this.setState({
			// loadingActive: false,
		});

		api.groups.getCourseGroups(courseId).then(json => {
			let groups = json.groups;
			this.setState({
				loadingActive: false,
				groups: groups
			});
		}).catch(console.error);
	};

	get filteredGroups() {
		if (this.state.filter === "archived") {
			return this.state.archiveGroups;
		} else {
			return this.state.groups;
		}
	};

	get loading() {
		if (this.state.filter === "archived") {
			return this.state.loadingArchived;
		} else {
			return this.state.loadingActive;
		}
	};

	toggleArchived = (group, isArchived) => {
		const newSettings = {
			is_archived: isArchived
		};
		api.groups.saveGroupSettings(group.id, newSettings);

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

	copyGroup = async (groupId) => {
		const { groups } = this.state;
		const copyGroup = await api.groups.getGroup(groupId);
		this.setState({
			groups: [copyGroup, ...groups],
		})
	};

	deleteGroup = (group, groupsName) => {
		api.groups.deleteGroup(group.id);
		const updateGroups = this.state[groupsName].filter(g => group.id !== g.id);
		this.setState({
			[groupsName]: updateGroups,
		});
	};

	createGroup = async (groupId) => {
		const newGroup = await api.groups.getGroup(groupId);
		const groups = this.filteredGroups;
		this.setState({
			groups: [newGroup, ...groups],
		})
	};

}

GroupsPage.propTypes = {
	match: PropTypes.object,
};

export default GroupsPage;