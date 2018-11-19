import React, { Component } from 'react';
import PropTypes from "prop-types";
import api from "../../../api/index";
import Tabs from "@skbkontur/react-ui/components/Tabs/Tabs";
import Button from "@skbkontur/react-ui/components/Button/Button";
import GroupMembers from "../../../components/groups/GroupSettingsPage/GroupMembers/GroupMembers";
import GroupSettings from "../../../components/groups/GroupSettingsPage/GroupSettings/GroupSettings";

import "./groupSettings.less";

class GroupPage extends Component {
	constructor(props) {
		super(props);
		this.state = {
			group: {},
			open: "settings",
			updatedFields: {},
			loading: false,
			scores: [],
		}
	};

	componentDidMount() {
		let groupId = this.props.match.params.groupId;

		this.loadGroup(groupId);
		this.loadGroupScores(groupId);

	};

	loadGroup = (groupId) => {
		api.groups.getGroup(groupId).then(group => {
			this.setState({
				group: group,
			});
		}).catch(console.error);
	};

	loadGroupScores = (groupId) => {
		api.groups.getGroupScores(groupId).then(json => {
			let scores = json.scores;
			this.setState({
				scores: scores,
			});
			console.log(scores);
		}).catch(console.error);
	};

	render() {
		const { group, open, loading, scores } = this.state;
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
								<GroupSettings
									group={group}
									scores={scores}
									onChangeSettings={this.onChangeSettings}
									onLoadingSettings={this.onLoadingSettings}
									onChangeScores={this.onChangeScores}/> }
							{ (open === "members")  &&
								<GroupMembers
									group={group}
									onChangeSettings={this.onChangeSettings}/> }
							<Button
								onClick={this.onLoadingSettings}
								size="medium"
								use="primary"
								loading={loading}
							>
								Сохранить
							</Button>
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
		console.log(updatedFields);
		console.log(group);
	};

	onChangeScores = (key, field, value) => {
		const { scores } = this.state;
		console.log(key, field, value);
		const updatedScores = scores
			.map(item => item.id === key ? {...item, [field]: value } : item);
		console.log(updatedScores);
		this.setState({
			scores: updatedScores,
		});
	};

	onLoadingSettings = () => {
		const { group, updatedFields, scores } = this.state;
		console.log(updatedFields);
		this.setState({
			loading: true,
		});
		api.groups.saveGroupSettings(group.id, updatedFields)
			.then(group => {
				this.setState({
					loading: false,
					group: group,
				});
			}).catch(console.error);

		api.groups.saveScoresSettings(group.id, scores)
			.then(scores => {
				this.setState({
					loading: false,
					scores: scores,
				});
			}).catch(console.error);
	};
}

GroupPage.propTypes = {
	history: PropTypes.object,
	location: PropTypes.object,
	match: PropTypes.object
};

export default GroupPage;




