import React, { Component } from "react";
import api from "../../../api";
import PropTypes from 'prop-types';
import Loader from "@skbkontur/react-ui/components/Loader/Loader";
import Input from "@skbkontur/react-ui/components/Input/Input";
import Icon from "@skbkontur/react-ui/components/Icon/Icon";
import GroupInfo from "./GroupInfo";
import GroupHeader from "./GroupHeader";
// import GroupsSettingsPage from "./GroupSettingsPage";


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

class GroupsList extends Component {

	render() {
		if (this.props.loading) {
			return (
				<Loader type="big" active />
			)
		}
		return (
			<div className="groups-wrapper">
				<Input className="search-field" placeholder="Начните вводить название группы" leftIcon={<Icon name="Search" />} />
				{ this.props.groups.map(group =>
					<GroupInfo
						key={group.id}
						group={group}
					/>) }
			</div>
		);
	}
}

GroupsList.propTypes = {
	groups: PropTypes.array,
	loading: PropTypes.bool
};