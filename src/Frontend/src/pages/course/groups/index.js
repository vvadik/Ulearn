import React, { Component } from "react";
import api from "../../../api";
import PropTypes from 'prop-types';
import GroupsList from "../../../components/groups/GroupMainPage/GroupList/GroupsList";
import GroupHeader from "../../../components/groups/GroupMainPage/GroupHeader/GroupHeader";

import "./groupsPage.less";

class AbstractPage extends Component {

}

class GroupsPage extends AbstractPage {
	constructor(props) {
		super(props);
		this.state = {
			loading: true,
			groups: [],
			filter: "active",
			courseId: "",
			archiveGroups: [],
		}
	};

	componentDidMount() {
		let courseId = this.props.match.params.courseId;

		api.groups.getCourseGroups(courseId).then(json => {
			let groups = json.groups;
			this.setState({
				loading: false,
				courseId: courseId,
				groups: groups
			});
		}).catch(console.error);

		api.groups.getCourseArchiveGroups(courseId).then(json => {
		let archiveGroups = json.groups;
		this.setState({
			loading: false,
			courseId: courseId,
			archiveGroups: archiveGroups
		});
		}).catch(console.error)
	};

	render() {
		return (
			<div className="wrapper">
				<div className="content-wrapper">
					<GroupHeader
						onTabClick={this.onTabClick}
						filter={this.state.filter}
						courseId={this.state.courseId}
						onSubmit={this.onSubmit}
					/>
					<GroupsList
						loading={this.state.loading}
						groups={this.filteredGroups}
						deleteGroup={this.deleteGroup}
						makeArchival={this.makeArchival}
					/>
				</div>
			</div>
		)
	};

	onTabClick = (id) => {
		this.setState({
			filter: id
		})
	};

	get filteredGroups() {
		if (this.state.filter === "archived") {
			return this.state.archiveGroups;
		} else {
			return this.state.groups;
		}
	};

	makeArchival = async (group, groupId) => {
		const newSettings = {
			is_archived: true
		};
		await api.groups.saveGroupSettings(groupId, newSettings);
		const { groups, archiveGroups } = this.state;
		const updatedGroups = groups.filter(group => groupId !== group.id);
		const updatedGroup = {...group, is_archived: true};
		const updatedArchiveGroups = [updatedGroup, ...archiveGroups];

		this.setState({
			groups: updatedGroups,
			archiveGroups: updatedArchiveGroups,
		});
	};

	deleteGroup = async (group, groupId) => {
		const groups = this.filteredGroups;

		if (groups.includes(group)) {
			groups.splice(groups.indexOf(group), 1);
			this.setState({
				groups: groups,
			})
		}
		await api.groups.deleteGroup(groupId);
	};

	onSubmit = async (groupId) => {
		const group = await api.groups.getGroup(groupId);
		const groups = this.filteredGroups;
		groups.unshift(group);
		this.setState({
			groups: groups,
		})
	}
}

GroupsPage.propTypes = {
	history: PropTypes.object,
	location: PropTypes.object,
	match: PropTypes.object
};

export default GroupsPage;
