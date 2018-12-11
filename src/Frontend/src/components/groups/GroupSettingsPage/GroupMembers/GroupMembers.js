import React, {Component} from "react";
import PropTypes from "prop-types";
import {CopyToClipboard} from 'react-copy-to-clipboard';
import moment from "moment";
import 'moment/locale/ru';
import "moment-timezone";
import api from "../../../../api";
import Icon from "@skbkontur/react-icons";
import Kebab from "@skbkontur/react-ui/components/Kebab/Kebab";
import Toast from "@skbkontur/react-ui/components/Toast/Toast";
import MenuItem from "@skbkontur/react-ui/components/MenuItem/MenuItem";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import Button from "@skbkontur/react-ui/components/Button/Button";
import ComboboxSearch from "./ComboboxSearch";
import Loader from "@skbkontur/react-ui/components/Loader/Loader";
import Avatar from "./Avatar";
import GroupStudents from "./GroupStudents";
import getWordForm from "../../../../utils/getWordForm";

import styles from './style.less';

class GroupMembers extends Component {

	state = {
		accesses: [],
		selected: null,
		students: [],
		loadingTeachers: false,
		loadingStudents: false,
		copied: false,
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
				loadingTeachers: false,
			});
		}).catch(console.error);

		this.setState({
			loadingTeachers: true,
		});
	};

	loadStudents = (groupId) => {
		api.groups.getStudents(groupId).then(json => {
			let students = json.students;
			this.setState({
				students,
				loadingStudents: false,
			});
		}).catch(console.error);

		this.setState({
			loadingStudents: true,
		});
	};

	render() {
		const { accesses, students, loadingStudents, loadingTeachers } = this.state;
		const { group, courseId } = this.props;
		const owner = group.owner;

		return (
			<div className={styles["group-settings-wrapper"]}>
				<div className={styles["teachers-block"]}>
					<h4>Преподаватели</h4>
					<p>
						Преподаватели могут видеть список участников группы, проводить код-ревью
						и проверку тестов, выставлять баллы и смотреть ведомость.
					</p>
					<Loader type="big" active={loadingTeachers}>
						<div className={styles["teacher-block"]}>
							<Avatar user={owner} size={styles["_big"]} />
							<div className={styles["teacher-block-name"]}>
								<div>{ owner.visible_name }</div>
								<span className={styles["teacher-status"]}>Владелец</span>
							</div>
						</div>
						{ (accesses.length > 0) && this.renderTeachers() }
					</Loader>
					{ this.renderTeachersSearch() }
				</div>
				<div className={styles["students-block"]}>
					<h4>Студенты</h4>
					{ this.renderInviteBlock() }
					<Loader type="big" active={loadingStudents}>
						<div className={styles["students-list"]}>
							{(students.length >0) &&
							<GroupStudents
								students={students}
								group={group}
								onDeleteStudents={this.onDeleteStudents}/>}
						</div>
					</Loader>
				</div>
			</div>
		)
	}

	renderTeachers() {
		const { accesses } = this.state;
		const grantTime = (grantTime) => moment.tz(grantTime, 'Europe/Moscow').tz('Asia/Yekaterinburg').format();

		return (accesses.map(item =>
			<React.Fragment
				key={item.user.id}>
				<div className={styles["teacher-block"]}>
					<Avatar user={item.user} size={styles["_big"]} />
					<div className={styles["teacher-block-name"]}>
						<div>{item.user.visible_name}</div>
						<span className={styles["teacher-status"]}>
							Полный доступ {getWordForm('предоставила', 'предоставил', this.props.group.owner.gender)}
							{' '} {item.granted_by.visible_name} {' '}
							{ moment(grantTime(item.grant_time)).fromNow() }
						</span>
					</div>
					<div className={styles["group-action"]}>
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

	renderTeachersSearch() {
		const { group, courseId } = this.props;
		const { accesses, selected } = this.state;

		return (
			<label className={styles["teacher-block-search"]}>
				<p>Добавить преподавателя:</p>
				<ComboboxSearch
					selected={selected}
					courseId={courseId}
					accesses={accesses}
					owner={group.owner}
					onAddTeacher={this.onAddTeacher}/>
			</label>
		)
	}

	renderInviteBlock() {
		const { group } = this.props;
		const inviteLink = group.is_invite_link_enabled || false;
		return (
			<div>
				{inviteLink && this.renderInviteHash()}
				<span
					className={`${styles["invite-link"]} ${styles[`invite-link${inviteLink ? '_off' : '_on'}`]}`}
					onClick={this.onToggleHash}>
					{inviteLink ? 'Выключить ссылку' : 'Включить ссылку для вступления в группу'}
				</span>
			</div>
		)
	}

	renderInviteHash() {
		const { group } = this.props;

		return (
			<React.Fragment>
				<p className={styles["students-invite-text"]}>Отправьте своим студентам ссылку для вступления в группу:</p>
				<div className={styles["students-invite"]}>
					<CopyToClipboard
						text={`https://ulearn.me/Account/JoinGroup?hash=${group.invite_hash}`}
						onCopy={() => this.setState({copied: true})}>
						<Button use="link" onClick={() => Toast.push('Ссылка скопирована')}>
							<Gapped gap={5}>
								<Icon name="Link" />
								Скопировать ссылку
							</Gapped>
						</Button>
					</CopyToClipboard>
				</div>
			</React.Fragment>
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
				grant_time: moment(),
			});

		this.setState({
			accesses: updatedAccesses,
		});

		api.groups.addGroupAccesses(group.id, item.value)
			.then(response => response)
			.catch(console.error);
	};

	onToggleHash = (event) => {
		const { group } = this.props;
		const inviteLink = group.is_invite_link_enabled || false;
		const field = 'is_invite_link_enabled';
		const updatedField = {[field]: !inviteLink};

		event.preventDefault();

		this.setState({copied: false});
		this.props.onChangeSettings(field, !inviteLink);
		api.groups.saveGroupSettings(group.id, updatedField)
			.then(response => response)
			.catch(console.error);
	};

	onDeleteStudents = (students) => {
		const { group } = this.props;
		const updatedStudents = this.state.students.filter((item) => !students.includes(item.user.id));

		this.setState({
			students: updatedStudents,
		});

		api.groups.deleteStudents(group.id, students)
			.then(response => response.json())
			.catch(console.error);
	};
}

GroupMembers.propTypes = {
	courseId: PropTypes.string,
	group: PropTypes.object,
	onChangeSettings: PropTypes.func,
	onChangeGroupOwner: PropTypes.func,
};

export default GroupMembers;
