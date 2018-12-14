import React, { Component } from 'react';
import PropTypes from "prop-types";
import {Helmet} from "react-helmet";
import api from "../../../api/index";
import Tabs from "@skbkontur/react-ui/components/Tabs/Tabs";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Toast from "@skbkontur/react-ui/components/Toast/Toast";
import GroupMembers from "../../../components/groups/GroupSettingsPage/GroupMembers/GroupMembers";
import GroupSettings from "../../../components/groups/GroupSettingsPage/GroupSettings/GroupSettings";

import styles from "./groupPage.less";

class GroupPage extends Component {

	state = {
		group: {},
		open: "settings",
		updatedFields: {},
		error: false,
		loadSettings: false,
		loading: false,
		scores: [],
		scoresId: [],
	};

	componentDidMount() {
		let groupId = this.props.match.params.groupId;

		this.loadGroupScores(groupId);
		this.loadGroup(groupId);
	};

	loadGroup = (groupId) => {
		api.groups.getGroup(groupId)
			.then(group => {
			this.setState({
				group,
				loading: !this.state.scores ? true : false,
			});
		}).catch(console.error);

		this.setState({
			loading: true,
		});
	};

	loadGroupScores = (groupId) => {
		api.groups.getGroupScores(groupId)
			.then(json => {
			let scores = json.scores;
			this.setState({
				scores: scores,
				loading: this.state.group.id ? false : true,
			});
		}).catch(console.error);

		this.setState({
			loading: true,
		});
	};

	render() {
		let courseId = this.props.match.params.courseId;
		const { group, open, loadSettings, loading, scores, updatedFields, error } = this.state;

		return (
			<div className={styles.wrapper}>
				<Helmet>
					<title>{`Группа ${group.name}`}</title>
				</Helmet>
				<div className={styles["content-wrapper"]}>
					<header className={styles["group-header"]}>
						<h2 className={styles["group-name"]}>{ group.name }</h2>
						<div className={styles["tabs-container"]}>
							<Tabs value={open} onChange={this.onChangeTab}>
								<Tabs.Tab id="settings">Настройки</Tabs.Tab>
								<Tabs.Tab id="members">Участники</Tabs.Tab>
							</Tabs>
						</div>
					</header>
					<div className={styles.content}>
						{ (open === "settings") &&
							<form onSubmit={this.loadingSettings}>
								<GroupSettings
									loading={loading}
									name={updatedFields.name}
									group={group}
									scores={scores}
									error={error}
									onChangeName={this.onChangeName}
									onChangeSettings={this.onChangeSettings}
									onChangeScores={this.onChangeScores} />
								<Button
									size="medium"
									use="primary"
									type="submit"
									loading={loadSettings}>
									Сохранить
								</Button>
							</form> }
						{ (open === "members")  &&
							<GroupMembers
								courseId={courseId}
								group={group}
								onChangeGroupOwner={this.onChangeGroupOwner}
								onChangeSettings={this.onChangeSettings}/>
						}
					</div>
				</div>
			</div>
		)
	}

	onChangeTab = (_, v) => {
		this.setState({
			open: v,
		})
	};

	onChangeName = (value) => {
		const { updatedFields } = this.state.updatedFields;

		this.setState({
			updatedFields: {
				...updatedFields,
				name: value,
			}
		});
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
	};

	onChangeGroupOwner = (user) => {
		const { group } = this.state;
		const updatedGroupAccesses = group.accesses.filter(item =>
			item.user.id !== user.id);
		const updatedGroup = { ...group, owner: user, updatedGroupAccesses };
		this.setState({
			group: updatedGroup,
		});
	};

	onChangeScores = (key, field, value) => {
		const { scores } = this.state;
		const updatedScores = scores
			.map(item => item.id === key ? {...item, [field]: value } : item);

		const scoresInGroup = updatedScores
			.filter(item => item[field] === true)
			.map(item => item.id);

		this.setState({
			scores: updatedScores,
			scoresId: scoresInGroup,
		});
	};

	loadingSettings = (e) => {
		const { group, updatedFields, scoresId } = this.state;

		Toast.push('Настройки сохранены');

		e.preventDefault();

		this.setState({
			loadSettings: true,
			group: {
			...group,
				name: updatedFields.name,
			}
		});

		api.groups.saveGroupSettings(group.id, updatedFields)
			.then(group => {
				this.setState({
					loadSettings: false,
					group: group,
				});
			}).catch(console.error);

		api.groups.saveScoresSettings(group.id, scoresId)
			.then(response => {
				this.setState({
					loadSettings: false,
				});
				return response;
			})
			.catch(console.error);
	};
}

GroupPage.propTypes = {
	history: PropTypes.object,
	location: PropTypes.object,
	match: PropTypes.object,
};

export default GroupPage;




