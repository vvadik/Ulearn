import React, {Component} from "react";
import PropTypes from "prop-types";
import moment from "moment";
import 'moment/locale/ru';
import "moment-timezone";
import Checkbox from "@skbkontur/react-ui/components/Checkbox/Checkbox";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import Select from "@skbkontur/react-ui/components/Select/Select";
import Icon from "@skbkontur/react-icons";
import Avatar from "./Avatar";
import getWordForm from "../../../../utils/getWordForm";

import styles from './style.less';
import CopyGroupModal from "../../GroupMainPage/CopyGroupModal/CopyGroupModal";

export default class GroupStudents extends Component {
	constructor(props) {
		super(props);

		this.state = {
			studentsId: {},
			checkedAll: false,
			modalOpen: false,
			course: '',
			value: '',
		};
	}

	render() {
		const { students } = this.props;
 		// console.log(students);
		const grantTime = (grantTime) => moment.tz(grantTime, 'Europe/Moscow').tz('Asia/Yekaterinburg').format();

		return(
			<React.Fragment>
				<div>
					<div className={styles["students-actions"]}>
						<Checkbox
							checked={this.state.checkedAll}
							onChange={this.onCheckAllStudents}>
							<span>Выбрать всех</span>
						</Checkbox>
						{this.renderStudentActions()}
					</div>
					<div>
						{students.map(item =>
						<div className={styles["student-block"]}
							key={item.user.id}>
							<Checkbox
								checked={this.state.studentsId[item.user.id] || false}
								onChange={(_, value) => this.onCheckStudent(item.user.id, _, value)}>
								<Avatar user={item.user} size={styles["_small"]} />
								{ item.user.visible_name } {' '}
								<span className={styles["students-action__text"]}>
									{ getWordForm('вступила', 'вступил', item.user.gender) }
									{' '} { moment(grantTime(item.adding_time)).fromNow() }
								</span>
							</Checkbox>
						</div>
					)}
					</div>
				</div>
				{ this.state.modalOpen && <CopyGroupModal onClose={this.onCloseModal}/> }
			</React.Fragment>
		)
	}

	renderStudentActions() {
		const { studentsId } = this.state;
		const studentsIdToArray = Object.keys(studentsId);
 		let buttonState = `${styles["students-action"]}`;
		if (studentsIdToArray.length > 0) {
			buttonState = `${styles["students-action"]} ${styles["_active"]}`
		} else {
			buttonState = `${styles["students-action"]}`
		}

		return (
		<React.Fragment>
			<button
				className={buttonState}
				disabled={!this.state.checked}
				onClick={this.onOpenModal}>
				<Gapped gap={3}>
					<Icon name="Copy" />
					<span className={styles["students-action__text"]}>Копировать</span>
				</Gapped>
			</button>
			<button
				className={buttonState}
				disabled={!this.state.checked}
				onClick={this.props.onDeleteStudents}
				>
				<Gapped gap={3}>
					<Icon name="Trash" />
					<span className={styles["students-action__text"]}>Удалить студентов</span>
				</Gapped>
			</button>
		</React.Fragment>
		);
	}

	renderStudentsActions() {
		return (
			<React.Fragment>
				<label className={styles["select-course"]}>
					<p className={styles["course-info"]}>
					Выберите курс, из которого хотите скопировать студентов
					</p>
					<Select
						autofocus
						items={this.state.course}
						onChange={this.onCourseChange}
						width="200"
						placeholder="Курс"
						value={this.state.course}
						error={this.hasError()}
						use="default"
					/>
				</label>
				<label className={styles["select-course"]}>
					<p className={styles["course-info"]}>
					Выберите группу, в которую хотите скопировать студентов
					</p>
					<Select
						autofocus
						items={this.state.course}
						onChange={this.onCourseChange}
						width="200"
						placeholder="Курс"
						value={this.state.course}
						error={this.hasError()}
						use="default"
					/>
				</label>
			</React.Fragment>
		)
	}

	onOpenModal = () => {
		this.setState({
			modalOpen: true,
		})
	};

	onCloseModal = () => {
		this.setState({
			modalOpen: false,
		})
	};

	onCheckAllStudents = (_, value) => {
		const { students } = this.props;
		const updatedStudentsId = students.map(item => ({[item.user.id]: value}))
			.reduce(function(result, item) {
			let key = Object.keys(item)[0];
			result[key] = item[key];
			return result;
			}
		);
		console.log('studentsId', updatedStudentsId);

		this.setState({
			checkedAll: value,
			studentsId: {
				...updatedStudentsId,
			},
		});
	};

	onCheckStudent = (id, _, value) => {
		const { studentsId } = this.state;
		const { students } = this.props;
		const studentsIdToArray = Object.keys(studentsId);
		const studentsToArrayOfIds = students.map(item => item.user.id);
		studentsId[id] = value;

		this.setState({
			studentsId,
			});

		if ((value === false)) {
			this.setState({
				studentsId,
			});
		}
		console.log('checkOne', studentsId);

		if ((studentsToArrayOfIds.length - 1 === studentsIdToArray.length) &&
			studentsIdToArray.length > 0) {
			this.setState({
				checkedAll: true,
			});
		} else {
			this.setState({
				checkedAll: false,
			})
		}
		// console.log('idFromObject:', studentsIdToArray);
		// console.log('idFromArray', studentsToArrayOfIds);
	};

	getCourseOptions = () => {
		const { course } = this.state;
		return course;
	};

	onCourseChange = (_, value) => {
		this.setState({
			course: value,
		});
	};
}

GroupStudents.propTypes = {
	students: PropTypes.array,
};