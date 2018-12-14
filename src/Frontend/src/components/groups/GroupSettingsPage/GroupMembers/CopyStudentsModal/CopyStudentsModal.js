import React, {Component} from "react";
import PropTypes from "prop-types";
import api from "../../../../api";
import Select from "@skbkontur/react-ui/components/Select/Select";
import Modal from "@skbkontur/react-ui/components/Modal/Modal";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Toast from "@skbkontur/react-ui/components/Toast/Toast";

import styles from './style.less';
import getPluralForm from "../../../../utils/getPluralForm";

export default class CopyStudentsModal extends Component {
	constructor(props) {
		super(props);
		this.state = {
			groupId: null,
			courseId: null,
			groups: [],
			courses: [],
			error: null,
		};
	}

	componentDidMount() {
		api.courses.getUsersCourses().then(json => {
			let courses = json.courses;
			this.setState({
				courses: courses
			});
		});
	}

	loadGroups = (courseId) => {
		api.groups.getCourseGroups(courseId)
			.then(json => {
				let groups = json.groups;
				this.setState({
					groups: groups,
				});
			}).catch(console.error);
	};

	render() {
		const { onClose } = this.props;
		return (
			<Modal onClose={onClose} width={640}>
				<Modal.Header>Скопировать группу из курса</Modal.Header>
				<form onSubmit={this.onSubmit}>
					<Modal.Body>
						<p className={styles["modal-text"]}>Студенты будут добавлены в выбранную группу.
							Скопируются все данные студента, в том числе прогресс студента. (Но это не точно)
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
		const { courseId } = this.state;
		return (
			<React.Fragment>
				<p className={styles["course-info"]}>
					Выберите курс, в который будут скопированы студенты
				</p>
				<label className={styles["select-course"]}>
					<Select
						autofocus
						required
						items={this.getCourseOptions()}
						onChange={this.onCourseChange}
						width="200"
						placeholder="Курс"
						value={courseId}
						error={this.hasError()}
						use="default"
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
					Выберите группу, в которую будут скопированы студентов
				</p>
				<label className={styles["select-course"]}>
					<Select
						autofocus
						required
						items={this.getGroupOptions()}
						onChange={this.onGroupChange}
						width="200"
						placeholder="Группа"
						value={groupId}
						error={this.hasError()}
						use="default"
						disabled={groups.length === 0}
					/>
					{ this.onCheckGroups() && this.renderEmptyGroups() }
				</label>
			</React.Fragment>
		)
	}

	renderEmptyGroups() {
		return (
			<p className={styles["empty-group-text"]}><b>В выбранном вами курсе нет доступных групп</b></p>
		)
	}

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

	getGroupOptions = () => {
		const { groups } = this.state;

		return groups.map(group => [group.id, `${group.name}: ${group.students_count} 
		${getPluralForm(group.students_count, 'студент', 'студента', 'студентов')}`]);
	};

	onGroupChange = (_, value) => {
		this.setState({ groupId: value });
	};

	hasError = () => {
		return this.state.error !== null;
	};

	onCheckGroups = () => {
		const { courseId, groups } = this.state;
		if (!groups) {
			return false;
		}
		return (courseId && groups.length === 0);
	};

	onSubmit = (e) => {
		const { groupId, courseId } = this.state;
		const { studentIds, currentGroupId, onClose } = this.props;
		const students = [...studentIds];

		e.preventDefault();

		if (!courseId || !groupId) {
			this.setState({
				error: 'Выберите курс',
			});
			return;
		}

		api.groups.copyStudents(currentGroupId, groupId, students)
			.then(response => response)
			.catch(console.error);

		onClose();
		Toast.push('Скопировано');
	};
};

CopyStudentsModal.propTypes = {
	onCloseModal: PropTypes.func,
	currentGroupId: PropTypes.number,
	studentIds: PropTypes.object,
};