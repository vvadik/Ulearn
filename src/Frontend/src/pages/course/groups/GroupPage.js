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
			groupName: null,
			open: "settings",
			updatedFields: {},
			loading: false,
			scores: [],
			scoresId: [],
			accesses: [],
		}
	};

	componentDidMount() {
		let groupId = this.props.match.params.groupId;

		this.loadGroup(groupId);
		this.loadGroupScores(groupId);
		this.loadGroupAccesses(groupId);
	};

	loadGroup = (groupId) => {
		api.groups.getGroup(groupId).then(group => {
			this.setState({
				group,
			});
		}).catch(console.error);
	};

	loadGroupScores = (groupId) => {
		api.groups.getGroupScores(groupId).then(json => {
			let scores = json.scores;
			this.setState({
				scores,
			});
		}).catch(console.error);
	};

	loadGroupAccesses = (groupId) => {
		api.groups.getGroupAccesses(groupId).then(json => {
			let accesses = json.accesses;
			this.setState({
				accesses,
			});
		}).catch(console.error);
	};

	render() {
		const { group, open, loading, scores, accesses } = this.state;
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
									onChangeName={this.onChangeName}
									onChangeSettings={this.onChangeSettings}
									onLoadingSettings={this.onLoadingSettings}
									onChangeScores={this.onChangeScores} /> }
							{ (open === "members")  &&
								<GroupMembers
									group={group}
									accesses={accesses}
									onChangeSettings={this.onChangeSettings}
									onChangeOwner={this.onChangeOwner}
									onRemoveTeacher={this.onRemoveTeacher}/> }
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

	onChangeName = (value) => {
		const { groupName, updatedFields } = this.state;
		this.setState({
			groupName: value,
			updatedFields: {
				...updatedFields,
				name: groupName,
			}
		});
		console.log(groupName);
		console.log(updatedFields);
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
				[field]: value,
			}
		});
			console.log(updatedFields);
	};

	onChangeScores = (key, field, value) => {
		const { scores } = this.state;
		const updatedScores = scores
			.map(item => item.id === key ? {...item, [field]: value } : item);
		console.log(updatedScores);

		const scoresInGroup = updatedScores
			.filter(item => item[field] === true)
			.map(item => item.id);
		console.log(scoresInGroup);

		this.setState({
			scores: updatedScores,
			scoresId: scoresInGroup,
		});
	};

	onChangeOwner = (user) => {
		const { group, accesses } = this.state;

		const updatedGroupAccesses = group.accesses.map(item =>
			item.user.id === user.id ? { ...item, user: group.owner } : item);
		// console.log(updatedGroupAccesses);
		const updatedAccesses = accesses.map(item =>
		item.user.id === user.id ? {...item, user: group.owner} : item);
		// console.log(updatedAccesses);

		const updatedGroup = { ...group, owner: user, accesses: updatedGroupAccesses };
		// console.log(updatedGroup);
		this.setState({
			accesses: updatedAccesses,
			group: updatedGroup,
		});

		api.groups.changeGroupOwner(group.id, user.id)
			.then(response => response)
			.catch(console.error);
	};

	// onUpdateOwner = (user, arr) => {
	// 	const { group } = this.props;
	//
	// 	arr.map(item =>
	// 		item.user.id === user.id ? { ...item, user: group.owner } : { ...item});
	// };

	onRemoveTeacher = (user) => {
		const { accesses } = this.state;
		const updatedAccesses = accesses
			.map(item => item.user.id === user.id ? {...item, user: {}} : item);

		// console.log(updatedAccesses);

		this.setState({
			accesses: updatedAccesses,
		});

		// api.groups.removeAccess(group.id, user.id)
		// .then(response => response)
		// .catch(console.error);
	};

	onLoadingSettings = () => {
		const { group, updatedFields, scoresId } = this.state;

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

		api.groups.saveScoresSettings(group.id, scoresId)
			.then(response => response)
			.catch(console.error);
	};
}

GroupPage.propTypes = {
	history: PropTypes.object,
	location: PropTypes.object,
	match: PropTypes.object
};

export default GroupPage;




