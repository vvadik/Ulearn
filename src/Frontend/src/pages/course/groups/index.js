import React, { Component } from "react";
import api from "../../../api";
import Loader from "@skbkontur/react-ui/components/Loader/Loader";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Tabs from "@skbkontur/react-ui/components/Tabs/Tabs";
import Input from "@skbkontur/react-ui/components/Input/Input";
import Icon from "@skbkontur/react-ui/components/Icon/Icon";
import GroupContainer from "./GroupContainer";

import "./groupsPage.less";

class AbstractPage extends Component {

}

class GroupsPage extends AbstractPage {
	constructor(props) {
		super(props);
		this.state = {
			loading: true,
			groups: [],
			filter: "active"
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
				/>
				<GroupsWrapper
					loading = {this.state.loading}
					groups = {this.filteredGroups}
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

export default GroupsPage;

class GroupHeader extends Component {
	constructor() {
		super();
	};

	onChange = (_, v) => {
		this.props.onTabClick(v);
	};

	render() {
		return (
			<div className="group-header">
				<div className="group-header-container">
					<h2>Группы</h2>
					<div className="buttons-container">
						<Button use="primary" size="medium">Создать группу</Button>
						<Button use="default" size="medium">Скопировать группу</Button>
					</div>
				</div>
				<div className="tabs-container">
					<Tabs  value={this.props.filter} onChange={this.onChange}>
						<Tabs.Tab id="active">Активные</Tabs.Tab>
						<Tabs.Tab id="archived">Архивные</Tabs.Tab>
					</Tabs>
				</div>
			</div>
		)
	}
}

class GroupsWrapper extends Component {

	render() {
		if (this.props.loading) {
			return (
				<Loader type="big" active />
			)
		}
		return (
			<div className="groups-wrapper">
				<Input className="search-field" placeholder="найти группу" leftIcon={<Icon name="Search" />} />
				{ this.props.groups.map(group =>
					<GroupContainer
						key={group.id}
						group={group}
					/>) }
			</div>
		);
	}
}