import React, {Component} from "react";
import PropTypes from "prop-types";
import Icon from "@skbkontur/react-icons";
import moment from "moment";
import 'moment/locale/ru';
import api from "../../../../api";
import Input from "@skbkontur/react-ui/components/Input/Input";
import Toggle from "@skbkontur/react-ui/components/Toggle/Toggle";
import Kebab from "@skbkontur/react-ui/components/Kebab/Kebab";
import MenuItem from "@skbkontur/react-ui/components/MenuItem/MenuItem";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import Avatar from "./Avatar";
import GroupStudents from "./GroupStudents";

import './style.less';
import {ComboboxSearch} from "../GroupSettings/ComboboxSearch";

class GroupMembers extends Component {

	state = {
		accesses: [],
		selected: null,
		students: [],
	};

	componentDidMount() {
		const groupId = this.props.group.id;

		this.loadGroupAccesses(groupId);
		this.loadStudents(groupId);
	}

	loadGroupAccesses = (groupId) => {
		api.groups.getGroupAccesses(groupId).then(json => {
			let accesses = json.accesses;
			this.setState({
				accesses,
			});
		}).catch(console.error);
	};

	loadStudents = (groupId) => {
		api.groups.getStudents(groupId).then(json => {
			let students = json.students;
			this.setState({
				students,
			});
		}).catch(console.error);
	};

	render() {
		const { accesses, students, selected } = this.state;
		const { group, courseId } = this.props;
		const owner = group.owner;
		const inviteLink = group.is_invite_link_enabled || false;

		return (
			<div className="group-settings-wrapper">
				<div className="teachers-block">
					<h4>Преподаватели</h4>
					<p>
						Преподаватели могут видеть список участников группы, проводить код-ревью
						и проверку тестов, выставлять баллы и смотреть ведомость.
					</p>
					<div className="teacher-block">
						<Avatar user={owner} />
						<div className="teacher-block-name">
							<div>{owner.visible_name}</div>
							<span className="teacher-status">Владелец</span>
						</div>
					</div>
					{(accesses.length > 0) && this.renderTeachers()}
					<label className="teacher-block-search">
						<p>Добавить преподавателя:</p>
						<ComboboxSearch
							selected={selected}
							courseId={courseId}
							accesses={accesses}
							owner={owner}
							onAddTeacher={this.onAddTeacher}/>
					</label>
				</div>
				<div className="students-block">
					<h4>Студенты</h4>
					<div className="students-block-invite">
						<p>Отправьте своим студентам ссылку для вступления в группу:</p>
						{inviteLink &&
						<Input
							type="text"
							value={`https://ulearn.me/Account/JoinGroup?hash=${group.invite_hash}`}
							readOnly
							selectAllOnFocus
						/>
						}
						<div className="toggle-invite">
							<label className="toggle-label">
								<Toggle
									checked={inviteLink}
									onChange={this.toggleHash}
									color="default">
								</Toggle>
								Ссылка для вступления в группу {inviteLink ? 'включена' : 'выключена'}
							</label>
						</div>
					</div>
					<div className="students-list">
						{(students.length >0) && <GroupStudents students={students} />}
					</div>
				</div>
			</div>
		)
	}

	renderTeachers() {
		const { accesses } = this.state;

		return (accesses.map(item =>
			<React.Fragment
				key={item.user.id}>
				<div className="teacher-block">
					<Avatar user={item.user} />
					<div className="teacher-block-name">
						<div>{item.user.visible_name}</div>
						<span className="teacher-status">
							Полный доступ предоставил(а) {item.granted_by.visible_name}{moment(item.grant_time).fromNow()}
						</span>
					</div>
					<div className="group-action">
						{this.renderKebab(item.user)}
					</div>
				</div>
			</React.Fragment>
			)
		)
	};

	renderKebab(user) {
		return (
			<Kebab size="large">
				<MenuItem onClick={() => this.onChangeOwner(user)}>
					<Gapped gap={5}>
						<Icon name="User" />
						Сделать владельцем
					</Gapped>
				</MenuItem>
				<MenuItem onClick={() => this.onRemoveTeacher(user)}>
					<Gapped gap={5}>
					<Icon name="Delete" />
					Забрать доступ
					</Gapped>
				</MenuItem>
			</Kebab>
		)
	}

	onChangeOwner = (user) => {
		const { accesses } = this.state;
		const { group } = this.props;
		const updatedAccesses = accesses.map(item =>
			item.user.id === user.id ? {...item, user: group.owner} : item);

		this.setState({
			accesses: updatedAccesses,
		});

		this.props.onChangeGroupOwner(user);

		api.groups.changeGroupOwner(group.id, user.id)
			.then(response => response)
			.catch(console.error);
	};

	onRemoveTeacher = (user) => {
		const { accesses } = this.state;
		const updatedAccesses = accesses
			.filter(item => item.user.id !== user.id);
		this.setState({
			accesses: updatedAccesses,
		});

		api.groups.removeAccess(this.props.group.id, user.id)
			.then(response => response)
			.catch(console.error);
	};

	onAddTeacher = (item) => {
		this.setState({
			selected: item,
			});
		this.onLoadTeacher(item);
	};

	onLoadTeacher = (item) => {
		const { accesses } = this.state;
		const { group } = this.props;
		const updatedAccesses = accesses
			.filter(i => i.user.id !== item.value)
			.concat({
				user: item,
				granted_by: group.owner,
				grant_time: ''
			});

		this.setState({
			accesses: updatedAccesses,
		});

		api.groups.addGroupAccesses(group.id, item.value)
			.then(response => response)
			.catch(console.error);
	};

	toggleHash = (value) => {
		this.props.onChangeSettings('is_invite_link_enabled', value);
	};
}

GroupMembers.propTypes = {
	courseId: PropTypes.string,
	group: PropTypes.object,
	onChangeSettings: PropTypes.func,
	onChangeGroupOwner: PropTypes.func,
};

export default GroupMembers;
