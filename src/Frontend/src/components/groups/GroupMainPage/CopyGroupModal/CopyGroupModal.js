import React, { Component } from 'react';
import PropTypes from "prop-types";
import api from "../../../../api";
import Modal from '@skbkontur/react-ui/components/Modal/Modal';
import Gapped from '@skbkontur/react-ui/components/Gapped';
import Select from '@skbkontur/react-ui/components/Select/Select';
import Button from '@skbkontur/react-ui/components/Button/Button';
import Checkbox from '@skbkontur/react-ui/components/Checkbox/Checkbox'

import './style.less';

class CopyGroupModal extends Component {

	constructor(props) {
		super(props);
		this.state = {
			group: null,
			courseId: null,
			instructors: [],
			courseTitle: '',
			hasError: false,
			error: null,
			changeOwnerWindow: false,
			changeOwner: true,
			coursesGroups: {},
			groups: [],
			courses: [],
		};
	}

	componentDidMount() {
		api.courses.getUsersCourses().then(json => {
			let courses = json.courses;
			this.setState({
				courses: courses
			});
		});

		let { courseId } = this.props;
		api.courses.getCourseTitle(courseId).then(json => {
			let courseTitle = json.title;
			this.setState({
				courseTitle: courseTitle,
			});
		}).catch(console.error);
	}

	render() {
		const { closeModal } = this.props;
		const { courseTitle } = this.state;
		return (
		<Modal onClose={closeModal} width={640}>
			<Modal.Header>Скопировать группу</Modal.Header>
				<form  onSubmit={this.onSubmit}>
					<Modal.Body>
						<p>Новая группа будет создана для курса <b>"{ courseTitle }"</b>.
							Скопируются все настройки группы (в том числе владелец),
							в неё автоматически добавятся участники из копируемой группы.
							Преподаватели тоже будут добавлены в группу, если у них есть права на
							курс <b>"{ courseTitle }"</b>.
						</p>
						{ this.renderSelect()}
					</Modal.Body>
					<Modal.Footer>
						<Button
							use="primary"
							size="medium"
							type="submit"
						>
							Cкопировать
						</Button>
					</Modal.Footer>
				</form>
			</Modal>
		)
	}

	renderSelect() {
		const { group, course, hasError } = this.state;
		return (
			<React.Fragment>
				<label>
					<Gapped gap={20}>
						Выберите курс
						<Select
							autofocus
							required
							items={this.getCourses()}
							onChange={this.newCourseValue}
							width="200"
							placeholder="Курс"
							value={course}
							error={hasError}
						/>
					</Gapped>
				</label>
				<label>
					<Gapped gap={20}>
						Выберите группу
						<Select
							autofocus
							required
							items={this.getGroups()}
							onChange={this.newGroupValue}
							width="200"
							placeholder="Выберите группу"
							value={group}
							error={hasError}
						/>
					</Gapped>
				</label>
				{ this.state.changeOwnerWindow && this.renderChangeOwner() }
			</React.Fragment>
		)
	}

	renderChangeOwner() {
		const { group, courseTitle, changeOwner } = this.state;
		return (
			<div className="change-owner-block">
				<p>Владелец этой группы {group.owner.visible_name} не является преподавателем курса
					<b>"{ courseTitle }"</b>. Вы можете сделать себя владельцем скопированной группы.
				</p>
				<Checkbox checked={changeOwner} onChange={(_, value) => this.setState({ changeOwner: value })}>Сделать меня владельцем группы</Checkbox>
			</div>
		)
	}

	getCourses = () => {
		const { courses } = this.state;
		return [
			...courses.map(course => [course.id, course.title ])
		];
	};

	newCourseValue = (_, value) => {
		this.setState({ courseId: value });
		this.loadGroups(value);
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

	getGroups = () => {
		const { groups } = this.state;
		return [
			...groups.map(group => [group, `${group.name}: ${group.students_count}`])
		];
	};

	newGroupValue = (_, value) => {
		this.setState({ group: value });
		this.loadCourseInstructors(value);
	};

	loadCourseInstructors(group) {
		let { courseId } = this.props;

		api.users.getUsersCourse(courseId)
			.then(json => {
			let instructors = json.instructors;
			this.setState({
				instructors: instructors,
			});
				this.checkOwner(group);
		}).catch(console.error);
	};

	checkOwner = (group) => {
		const { instructors } = this.state;
		const ownerId = group.owner.id;
		const instructorsId = instructors.map(instructor => instructor.id);
		if (!(instructorsId.includes(ownerId))) {
			this.setState({
				changeOwnerWindow: true,
			})
		}
	};

	onSubmit = async (e) => {
		const { group, courseId, changeOwner } = this.state;
		const { closeModal, copyGroup } = this.props;
		e.preventDefault();
		if (!courseId || (courseId && !group)) {
			this.setState({
				hasError: true,
				error: 'Выберите курс',
			});
			return;
		}

		const newGroup = await api.groups.copyGroup(group.id, courseId, changeOwner);
		console.log(newGroup);
		closeModal();
		copyGroup(newGroup.id);
	};
}

CopyGroupModal.propTypes = {
	closeModal: PropTypes.func,
	courseId: PropTypes.string,
	groups: PropTypes.array,
	copyGroup: PropTypes.func,
	courses: PropTypes.object,
};

export default CopyGroupModal;