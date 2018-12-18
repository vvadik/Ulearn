import React, {Component} from "react";
import PropTypes from "prop-types";
import moment from "moment";
import 'moment/locale/ru';
import "moment-timezone";
import Checkbox from "@skbkontur/react-ui/components/Checkbox/Checkbox";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import Icon from "@skbkontur/react-icons";
import Avatar from "../Avatar/Avatar";
import CopyStudentsModal from "../CopyStudentsModal/CopyStudentsModal";
import getWordForm from "../../../../../utils/getWordForm";

import styles from './style.less';

class GroupStudents extends Component {

	state = {
		studentIds: new Set(),
		modalOpen: false,
	};

	render() {
		const { students, group } = this.props;
		const { studentIds, modalOpen } = this.state;
		const studentsArrayOfIds = students.map(item => item.user.id);

		return(
			<React.Fragment>
				<div className={styles["actions-block"]}>
					<Checkbox
						checked={studentIds.size === studentsArrayOfIds.length || false}
						onChange={this.onCheckAllStudents}>
						Выбрать всех
					</Checkbox>
					{this.renderStudentActions()}
				</div>
				{this.renderStudents()}
				{ modalOpen &&
				<CopyStudentsModal
					currentGroupId={group.id}
					studentIds={studentIds}
					onClose={this.onCloseModal}/> }
			</React.Fragment>
		)
	}

	renderStudentActions() {
		const { studentIds } = this.state;
 		let buttonClass = `${styles.action}`;

		return (
		<React.Fragment>
			<button
				className={studentIds.size > 0 ? `${buttonClass} ${styles["button-copy"]}` : buttonClass}
				disabled={studentIds.size === 0}
				onClick={this.onOpenModal}>
				<Gapped gap={3}>
					<Icon name="Copy" />
					<span className={styles["action-text"]}>Скопировать в группу...</span>
				</Gapped>
			</button>
			<button
				className={studentIds.size > 0 ? `${buttonClass} ${styles["button-delete"]}` : buttonClass}
				disabled={studentIds.size === 0}
				onClick={this.onDeleteStudents}
				>
				<Gapped gap={3}>
					<Icon name="Trash" />
					<span className={styles["action-text"]}>Исключить из группы</span>
				</Gapped>
			</button>
		</React.Fragment>
		);
	}

	renderStudents() {
		const { students } = this.props;
		const { studentIds } = this.state;
		moment.tz.setDefault('Europe/Moscow');
		const grantTime = (grantTime) => moment(grantTime).tz('Asia/Yekaterinburg').format();

		return (
			<div>
				{students.map(item =>
					<div className={styles["student-block"]}
						 key={item.user.id}>
						<Checkbox
							checked={studentIds.has(item.user.id) || false}
							onChange={(_, value) => this.onCheckStudent(item.user.id, _, value)}>
							<Avatar user={item.user} size={styles.small} />
							{ item.user.visible_name } <span className={styles["action-text"]}>
								{ `${ getWordForm('вступила', 'вступил', item.user.gender) }
								${ moment(grantTime(item.adding_time)).fromNow() }` }</span>
						</Checkbox>
					</div>
				)}
			</div>
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
		const studentsToArrayOfIds = students.map(item => item.user.id);

		if (value) {
			this.setState({
				studentIds: new Set(studentsToArrayOfIds),
			});
		} else {
			this.setState({
				studentIds: new Set(),
			});
		}
	};

	onCheckStudent = (id, _, value) => {
		const { studentIds } = this.state;
		const copyStudents = new Set(studentIds);
		console.log(copyStudents);

		if (value) {
			copyStudents.add(id);
		} else {
			copyStudents.delete(id);
		}

		this.setState({
			studentIds: copyStudents,
		});
	};

	onDeleteStudents = () => {
		const students = [...this.state.studentIds];
		this.props.onDeleteStudents(students);
	};
}

GroupStudents.propTypes = {
	students: PropTypes.array,
	group: PropTypes.object,
	onDeleteStudents: PropTypes.func,
};
export default GroupStudents;
