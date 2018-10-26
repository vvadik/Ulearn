import { Component } from 'react';
import React from "react";
import api from "../../../../api/index";
import PropTypes from "prop-types";
import Tabs from "@skbkontur/react-ui/components/Tabs/Tabs";
import Button from "@skbkontur/react-ui/components/Button/Button";
import GroupMembers from "./GroupMembers";
import GroupSettings from "./GroupSettings";

import "./groupSettings.less";

class GroupSettingsPage extends Component {
	constructor(props) {
		super(props);
		this.state = {
			group: {},
			open: "settings",
			updatedFields: {},
		}
	};

	componentDidMount() {
		let groupId = this.props.match.params.groupId;
		api.groups.getGroup(groupId).then(group => {
			this.setState({
				group: group,
			});
		}).catch(console.error)
	};

	render() {
		return (
			<React.Fragment>
				<div className="wrapper">
					<div className="content-container">
						<h2>{ this.state.group.name }</h2>
						<div className="tabs-container">
							<Tabs value={this.state.open} onChange={this.onChangeTab}>
								<Tabs.Tab id="settings">Настройки</Tabs.Tab>
								<Tabs.Tab id="members">Участники</Tabs.Tab>
							</Tabs>
						</div>
						{ (this.state.open === "settings") && 
							<GroupSettings group={this.state.group}
										   onChangeSettings={this.onChangeSettings}/> }
						{ (this.state.open === "members")  && <GroupMembers /> }
					</div>
					<Button onClick={this.onClick} size="medium" use="primary">Сохранить</Button>
				</div>
			</React.Fragment>
		)
	}

	onChangeTab = (_, v) => {
		this.setState({
			open: v
		})
	};

	onChangeSettings = (field, value) => {
		const { group } = this.state;
		this.setState({
			group: {
				...group,
				name: value,
				[field]: value
			},
		});
		console.log(group);
	};

	onClick = () => {
		const { group } = this.state;
		api.groups.saveGroupSettings(group.id, group);
	};
}

GroupSettingsPage.propTypes = {
	history: PropTypes.object,
	location: PropTypes.object,
	match: PropTypes.object
};

export default GroupSettingsPage;




