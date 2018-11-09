import React, { Component } from 'react';
import api from "../../../api/index";
import PropTypes from "prop-types";
import Tabs from "@skbkontur/react-ui/components/Tabs/Tabs";
import Button from "@skbkontur/react-ui/components/Button/Button";
import GroupMembers from "../../../components/groups/GroupSettingsPage/GroupMembers/GroupMembers";
import GroupSettings from "../../../components/groups/GroupSettingsPage/GroupSettings/GroupSettings";

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
		const { group, open } = this.state;
		return (
			<React.Fragment>
				<div className="wrapper">
					<div className="content-wrapper">
						<div className="content">
							<h2>{ group.name }</h2>
							<div className="tabs-container">
								<Tabs value={open} onChange={this.onChangeTab}>
									<Tabs.Tab id="settings">Настройки</Tabs.Tab>
									<Tabs.Tab id="members">Участники</Tabs.Tab>
								</Tabs>
							</div>
							{ (open === "settings") &&
								<GroupSettings group={group}
											   onChangeSettings={this.onChangeSettings}/> }
							{ (open === "members")  && <GroupMembers group={group} /> }
							<Button onClick={this.onClick} size="medium" use="primary">Сохранить</Button>
						</div>
					</div>
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
		const { group, updatedFields } = this.state;
		this.setState({
			group: {
				...group,
				[field]: value
			},
			updatedFields: {
				...updatedFields,
				[field]: value
			}
		});
	};

	onClick = async () => {
		const { group, updatedFields } = this.state;
		await api.groups.saveGroupSettings(group.id, updatedFields);
	};
}

GroupSettingsPage.propTypes = {
	history: PropTypes.object,
	location: PropTypes.object,
	match: PropTypes.object
};

export default GroupSettingsPage;




