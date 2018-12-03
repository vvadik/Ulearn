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
import ComboboxSearch from "./ComboboxSearch";
import getWordForm from "../../../../utils/getWordForm";

import styles from './style.less';

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
			<div className={styles["group-settings-wrapper"]}>
				<div className={styles["teachers-block"]}>
					<h4>Преподаватели</h4>
					<p>
						Преподаватели могут видеть список участников группы, проводить код-ревью
						и проверку тестов, выставлять баллы и смотреть ведомость.
					</p>
					<div className={styles["teacher-block"]}>
						<Avatar user={owner} size={styles["_big"]}/>
						<div className={styles["teacher-block-name"]}>
							<div>{owner.visible_name}</div>
							<span className={styles["teacher-status"]}>Владелец</span>
						</div>
					</div>
					{(accesses.length > 0) && this.renderTeachers()}
					<label className={styles["teacher-block-search"]}>
						<p>Добавить преподавателя:</p>
						<ComboboxSearch
							selected={selected}
							courseId={courseId}
							accesses={accesses}
							owner={owner}
							onAddTeacher={this.onAddTeacher}/>
					</label>
				</div>
				<div className={styles["students-block"]}>
					<h4>Студенты</h4>
					<div className={styles["students-block-invite"]}>
						<p>Отправьте своим студентам ссылку для вступления в группу:</p>
						{inviteLink &&
						<Input
							type="text"
							value={`https://ulearn.me/Account/JoinGroup?hash=${group.invite_hash}`}
							readOnly
							selectAllOnFocus
						/>
						}
						<div className={styles["toggle-invite"]}>
							<label className={styles["toggle-label"]}>
								<Toggle
									checked={inviteLink}
									onChange={this.toggleHash}
									color="default">
								</Toggle>
								Ссылка в группу {inviteLink ? 'включена' : 'выключена'}
							</label>
						</div>
					</div>
					<div className={styles["students-list"]}>
						{(students.length >0) &&
						<GroupStudents
							students={students}
							group={group}
							onDeleteStudent={this.onDeleteStudent}/>}
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
				<div className={styles["teacher-block"]}>
					<Avatar user={item.user} size={styles["_big"]} />
					<div className={styles["teacher-block-name"]}>
						<div>{item.user.visible_name}</div>
						<span className={styles["teacher-status"]}>
							Полный доступ {getWordForm('предоставила', 'предоставил', item.user.gender)}
							{' '} {item.granted_by.visible_name} {moment(item.grant_time).fromNow()}
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
				grant_time: `${new Date()}`
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

	onDeleteStudent = (student) => {
		const { group } = this.props.group;
		const updateStudents = this.state.students.filter(item => item.user.id !== student);

		this.setState({
			students: updateStudents,
		});

		api.groups.deleteStudent(group.id, student)
			.then(response => response.json())
			.catch(console.error)
	};
}

GroupMembers.propTypes = {
	courseId: PropTypes.string,
	group: PropTypes.object,
	onChangeSettings: PropTypes.func,
	onChangeGroupOwner: PropTypes.func,
};

export default GroupMembers;
