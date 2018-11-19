import React, { Component } from 'react';
import PropTypes from "prop-types";
import api from "../../../../api";
import Modal from '@skbkontur/react-ui/components/Modal/Modal';
import Select from '@skbkontur/react-ui/components/Select/Select';
import Button from '@skbkontur/react-ui/components/Button/Button';
import Checkbox from '@skbkontur/react-ui/components/Checkbox/Checkbox';
import getPluralForm from "../../../../utils/getPluralForm";

import './style.less';

class CopyGroupModal extends Component {
	constructor(props) {
		super(props);
		this.state = {
			// form
			groupId: null,
			courseId: null,
			changeOwner: true,
			// ui
			groups: [],
			error: null,
			// store
			instructors: [],
			courses: [],
		};
	}

	componentDidMount() {
		let currentCourseId = this.props.courseId;

		this.loadCourses();
		this.loadCourseInstructors(currentCourseId);
	}

	loadCourses = () => {
		api.courses.getUsersCourses().then(json => {
			let courses = json.courses;
			this.setState({
				courses: courses
			});
		});
	};

	loadGroups = (courseId) => {
		api.groups.getCourseGroups(courseId)
			.then(json => {
				let groups = json.groups;
				this.setState({
					groups: groups,
				});
			}).catch(console.error);
	};

	loadCourseInstructors(courseId) {
		api.users.getUsersCourse(courseId)
			.then(json => {
				let instructors = json.instructors;
				this.setState({
					instructors: instructors,
				});
			}).catch(console.error);
	};

	render() {
		const { onClose } = this.props;
		let currentCourseId = this.props.courseId;
		const courseTitle = this.getCourseTitle(currentCourseId);
		return (
		<Modal onClose={onClose} width={640}> { /*FIXME*/ }
			<Modal.Header>Скопировать группу</Modal.Header>
				<form onSubmit={this.onSubmit} method="post">
					<Modal.Body>
						<p className="modal-text">Новая группа будет создана для курса <b>«{ courseTitle }»</b>.
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
						>
							Cкопировать
						</Button>
					</Modal.Footer>
				</form>
			</Modal>
		)
	}

	renderCourseSelect() {
		const { course } = this.state;
		return (
			<label className="select-course">
				<p className="course-info">
					Вы можете выбрать курс, в котором являетесь преподавателем
				</p>
				<Select
					autofocus
					required
					items={this.getCourseOptions()}
					onChange={this.onCourseChange}
					width="200"
					placeholder="Курс"
					value={course}
					error={this.hasError()}
					use="default"
				/>
			</label>
		)
	}

	renderGroupSelect() {
		const { groupId, groups } = this.state;
		return (
		<label className="select-group">
			<p className="group-info">
				Вам доступны только те группы,в которых вы являетесь преподавателем
			</p>
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
			{ this.changeOwnerWindow() && this.renderChangeOwner() }
		</label>
		);
	};

	renderEmptyGroups() {
		return (
			<p className="empty-group-text"><b>В выбранном вами курсе нет доступных групп</b></p>
		)
	}

	renderChangeOwner() {
		const currentCourseId = this.props.courseId;
		const { groupId, changeOwner } = this.state;
		const group = this.getGroup(groupId);
		const courseTitle = this.getCourseTitle(currentCourseId);

		return (
			<div className="change-owner-block">
				<p>Владелец этой группы <b>{group.owner.visible_name}</b> не является преподавателем курса
					<b>«{ courseTitle }»</b>. Вы можете сделать себя владельцем скопированной группы.
				</p>
				<Checkbox checked={changeOwner} onChange={(_, value) => this.setState({ changeOwner: value })}>Сделать меня владельцем группы</Checkbox>
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

	changeOwnerWindow = () => {
		const { groupId, instructors } = this.state;
		const group = this.getGroup(groupId);

		if (!group) {
			return false;
		}

		const ownerId = group.owner.id;
		const instructorsId = instructors.map(instructor => instructor.id);

		return !(instructorsId.includes(ownerId));
	};

	onSubmit = async (e) => {
		const { groupId, changeOwner } = this.state;
		const currentCourseId = this.props.courseId;
		const { onClose, onSubmit } = this.props;
		e.preventDefault();
		if (!currentCourseId || !groupId) {
			this.setState({
				error: 'Выберите курс',
			});
			return;
		}

		const newGroup = await api.groups.copyGroup(groupId, currentCourseId, changeOwner);

		onClose();
		onSubmit(newGroup.id);
	};
}

CopyGroupModal.propTypes = {
	courseId: PropTypes.string,
	onClose: PropTypes.func,
	onSubmit: PropTypes.func,
};

export default CopyGroupModal;