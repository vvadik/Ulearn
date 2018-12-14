import React, {Component} from "react";
import PropTypes from "prop-types";
import moment from "moment";
import 'moment/locale/ru';
import "moment-timezone";
import api from "../../../../api";
import Icon from "@skbkontur/react-icons";
import Kebab from "@skbkontur/react-ui/components/Kebab/Kebab";
import MenuItem from "@skbkontur/react-ui/components/MenuItem/MenuItem";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import ComboboxSearch from "./Combobox/ComboboxSearch";
import Loader from "@skbkontur/react-ui/components/Loader/Loader";
import Avatar from "./Avatar/Avatar";
import GroupStudents from "./GroupStudents/GroupStudents";
import InviteBlock from "./InviteBlock/InviteBlock";
import getWordForm from "../../../../utils/getWordForm";

import styles from './style.less';

class GroupMembers extends Component {

	state = {
		accesses: [],
		selected: null,
		students: [],
		loadingTeachers: false,
		loadingStudents: false,
	};

	componentDidMount() {
		const groupId = this.props.group.id;

		this.loadGroupAccesses(groupId);
		this.loadStudents(groupId);
	}

	loadGroupAccesses = (groupId) => {
		this.setState({
			loadingTeachers: true,
		});

		api.groups.getGroupAccesses(groupId)
			.then(json => {
			let accesses = json.accesses;
			this.setState({
				accesses,
				loadingTeachers: false,
			});
		}).catch(console.error);
	};

	loadStudents = (groupId) => {
		this.setState({
			loadingStudents: true,
		});

		api.groups.getStudents(groupId)
			.then(json => {
			let students = json.students;
			this.setState({
				students,
				loadingStudents: false,
			});
		}).catch(console.error);
	};

	render() {
		const { accesses, students, loadingStudents, loadingTeachers } = this.state;
		const { group } = this.props;
		const owner = group.owner;

		return (
			<div className={styles.wrapper}>
				<div className={styles.teachers}>
					<h4 className={styles["teachers-header"]}>Преподаватели</h4>
					<p className={styles["teachers-info"]}>
						Преподаватели могут видеть список участников группы, проводить код-ревью
						и проверку тестов, выставлять баллы и смотреть ведомость.
					</p>
					<Loader type="big" active={loadingTeachers}>
						<div className={styles["teacher-block"]}>
							<Avatar user={owner} size={styles.big} />
							<div className={styles["teacher-name"]}>
								<div>{ owner.visible_name }</div>
								<span className={styles["teacher-status"]}>Владелец</span>
							</div>
						</div>
						{ (accesses.length > 0) && this.renderTeachers() }
					</Loader>
					{ this.renderTeachersSearch() }
				</div>
				<div className={styles["students-block"]}>
					<h4 className={styles["students-header"]}>Студенты</h4>
					<InviteBlock group={group} onChangeSettings={this.props.onChangeSettings}/>
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
					<Avatar user={item.user} size={styles.big} />
					<div className={styles["teacher-name"]}>
						<div>{item.user.visible_name}</div>
						<span className={styles["teacher-status"]}>
							Полный доступ {getWordForm('предоставила', 'предоставил', this.props.group.owner.gender)}
							{' '} {item.granted_by.visible_name} {' '}
							{ moment(grantTime(item.grant_time)).fromNow() }
						</span>
					</div>
					<div className={styles["teacher-action"]}>
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
			<label className={styles["teacher-search"]}>
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
	group: PropTypes.object,
	onChangeSettings: PropTypes.func,
	onChangeGroupOwner: PropTypes.func,
};

export default GroupMembers;
