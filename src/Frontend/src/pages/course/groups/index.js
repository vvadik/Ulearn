import React, { Component } from "react";
import api from "../../../api/index";
import PropTypes from 'prop-types';
import GroupsList from "./GroupPage/GroupsList";
import GroupHeader from "./GroupPage/GroupHeader";

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
		}
	};

	onTabClick = (id) => {
		this.setState({
			filter: id
		})
	};

	componentDidMount() {
		let courseId = this.props.match.params.courseId;
		api.groups.getCourseGroups(courseId).then(json => {
			let groups = json.groups;
			this.setState({
				loading: false,
				groups: groups,
			});
		}).catch(console.error)
	};

	render() {
		return (
			<React.Fragment>
				<GroupHeader
					onTabClick={this.onTabClick}
					filter={this.state.filter}
					openModal={this.openModal}
					closeModal={this.closeModal}
				/>
				<GroupsList
					loading={this.state.loading}
					groups={this.filteredGroups}
				/>
			</React.Fragment>
		)
	};

	get filteredGroups() {
		return this.state.groups.filter((group) => {
			if (this.state.filter === "archived") {
				return group.is_archived;
			} else {
				return !group.is_archived ;
			}
		});
	};
}

GroupsPage.propTypes = {
	history: PropTypes.object,
	location: PropTypes.object,
	match: PropTypes.object
};

export default GroupsPage;
