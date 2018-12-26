import React, { Component } from 'react';
import PropTypes from "prop-types";
import api from "../../../../api";
import Modal from '@skbkontur/react-ui/components/Modal/Modal';
import Select from '@skbkontur/react-ui/components/Select/Select';
import Button from '@skbkontur/react-ui/components/Button/Button';
import Checkbox from '@skbkontur/react-ui/components/Checkbox/Checkbox';
import getPluralForm from "../../../../utils/getPluralForm";

import styles from "./style.less";

class CopyGroupModal extends Component {

	state = {
		groupId: null,
		courseId: null,
		changeOwner: true,
		groups: [],
		error: null,
		instructors: [],
		courses: [],
		loadGroup: false,
	};

	componentDidMount() {
		const currentCourseId = this.props.courseId;

		this.loadCourses();
		this.loadCourseInstructors(currentCourseId);
	}

	loadCourses = () => {
		api.courses.getUserCourses()
			.then(json => {
			let courses = json.courses;
			this.setState({
				courses,
			});
		}).catch(console.error);
	};

	loadCourseInstructors = (courseId) => {
		api.users.getCourseInstructors(courseId)
			.then(json => {
				let instructors = json.instructors;
				this.setState({
					instructors,
				});
			}).catch(console.error);
	};

	loadGroups = (courseId) => {
		api.groups.getCourseGroups(courseId)
			.then(json => {
				let groups = json.groups;
				this.setState({
					groups,
				});
			}).catch(console.error);
	};

	render() {
		const { onCloseModal } = this.props;
		const courseTitle = this.getCourseTitle(this.props.courseId);

		return (
			<Modal onClose={onCloseModal} width={640}>
				<Modal.Header>Скопировать группу из курса</Modal.Header>
				<form onSubmit={this.onSubmit}>
					<Modal.Body>
						<p className={styles["common-info"]}>Новая группа будет создана для курса <b>«{ courseTitle }»</b>.
							Скопируются все настройки группы (в том числе владелец),
							в неё автоматически добавятся участники из копируемой группы.
							Преподаватели тоже будут добавлены в группу, если у них есть права на
							курс <b>«{ courseTitle }»</b>.
						</p>
						{ this.renderCourseSelect() }
						{ this.renderGroupSelect() }
					</Modal.Body>
					<Modal.Footer>
						<Button
							use="primary"
							size="medium"
							type="submit"
							disabled={!this.state.groupId}
							loading={this.state.loadGroup}>
							Cкопировать
						</Button>
					</Modal.Footer>
				</form>
			</Modal>
		)
	}

	renderCourseSelect() {
		const { courseId } = this.state;

		return (
			<React.Fragment>
				<p className={styles["course-info"]}>
					Вы можете выбрать курс, в котором являетесь преподавателем
				</p>
				<label className={styles["select-course"]}>
					<Select
						autofocus
						required
						items={this.getCourseOptions()}
						onChange={this.onCourseChange}
						width="200"
						placeholder="Выберите курс"
						value={courseId}
						error={this.hasError()}
					/>
				</label>
			</React.Fragment>
		)
	}

	renderGroupSelect() {
		const { groupId, groups } = this.state;

		return (
			<React.Fragment>
				<p className={styles["group-info"]}>
					Вам доступны только те группы, в которых вы являетесь преподавателем
				</p>
				<label className={styles["select-group"]}>
					<Select
						autofocus
						required
						items={this.getGroupOptions()}
						onChange={this.onGroupChange}
						width="200"
						placeholder="Выберите группу"
						value={groupId}
						error={this.hasError()}
						use="default"
						disabled={groups.length === 0}
					/>
					{ this.checkGroups() && this.renderEmptyGroups() }
					{ this.checkOwner() && this.renderChangeOwner() }
				</label>
			</React.Fragment>
		);
	};

	renderEmptyGroups() {
		return (
			<p className={styles["empty-group-info"]}>В выбранном вами курсе нет доступных групп</p>
		)
	}

	renderChangeOwner() {
		const currentCourseId = this.props.courseId;
		const { groupId, changeOwner } = this.state;
		const group = this.getGroup(groupId);
		const courseTitle = this.getCourseTitle(currentCourseId);

		return (
			<div className={styles["change-owner-block"]}>
				<p className={styles["change-owner-info"]}>Владелец этой группы <b>{group.owner.visible_name}</b> не является преподавателем курса
					<b>«{ courseTitle }»</b>. Вы можете сделать себя владельцем скопированной группы.
				</p>
				<Checkbox checked={changeOwner} onChange={this.onChangeOwner}>Сделать меня владельцем группы</Checkbox>
			</div>
		)
	}

	hasError = () => {
		return this.state.error !== null;
	};

	getCourseTitle = (courseId) => {
		const { courses } = this.state;
		const course = courses.find(c => c.id === courseId);

		return course && course.title;
	};

	getGroup = (groupId) => {
		const { groups } = this.state;

		return groups.find(g => g.id === groupId);
	};

	getCourseOptions = () => {
		const { courses } = this.state;

		return courses.map(course => [course.id, course.title]);
	};

	onCourseChange = (_, value) => {

		this.setState({
			courseId: value,
			groupId: null
		});

		this.loadGroups(value);
	};

	checkGroups = () => {
		const { courseId, groups } = this.state;
		if (!groups) {
			return false;
		}

		return (courseId && groups.length === 0);
	};

	getGroupOptions = () => {
		const { groups } = this.state;

		return groups.map(group => [group.id, `${group.name}: ${group.students_count} 
		${getPluralForm(group.students_count, 'студент', 'студента', 'студентов')}`]);
	};

	onGroupChange = (_, value) => {
		this.setState({ groupId: value });
	};

	checkOwner = () => {
		const { groupId, instructors } = this.state;
		const group = this.getGroup(groupId);

		if (!group) {
			return false;
		}

		const ownerId = group.owner.id;
		const instructorsId = instructors.map(instructor => instructor.id);

		return !(instructorsId.includes(ownerId));
	};

	onChangeOwner = (_, value) => this.setState({ changeOwner: value });

	onSubmit = async (e) => {
		const { groupId, changeOwner } = this.state;
		const currentCourseId = this.props.courseId;
		const { onCloseModal, onSubmit } = this.props;
		e.preventDefault();
		if (!currentCourseId || !groupId) {
			this.setState({
				error: 'Выберите курс',
			});
			return;
		}

		this.setState({ loadGroup: true });
		const newGroup = await api.groups.copyGroup(groupId, currentCourseId, changeOwner);
		this.setState({ loadGroup: false });

		onCloseModal();
		onSubmit(newGroup.id);
	};
}

CopyGroupModal.propTypes = {
	courseId: PropTypes.string,
	onCloseModal: PropTypes.func,
	onSubmit: PropTypes.func,
};

export default CopyGroupModal;