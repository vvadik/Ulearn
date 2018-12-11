import React, {Component} from "react";
import PropTypes from "prop-types";
import moment from "moment";
import 'moment/locale/ru';
import "moment-timezone";
import Checkbox from "@skbkontur/react-ui/components/Checkbox/Checkbox";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import Icon from "@skbkontur/react-icons";
import Avatar from "./Avatar";
import CopyStudentsModal from "./CopyStudentsModal";
import getWordForm from "../../../../utils/getWordForm";

import styles from './style.less';
import Button from "@skbkontur/react-ui/components/Button/Button";

export default class GroupStudents extends Component {
	constructor(props) {
		super(props);

		this.state = {
			studentIds: new Set(),
			modalOpen: false,
		};
	}

	render() {
		const { students, group } = this.props;
		const { studentIds, modalOpen } = this.state;
		const studentsArrayOfIds = students.map(item => item.user.id);
		const grantTime = (grantTime) => moment.tz(grantTime, 'Europe/Moscow').tz('Asia/Yekaterinburg').format();

		return(
			<React.Fragment>
				<div>
					<div className={styles["students-actions"]}>
						<Checkbox
							checked={studentIds.size === studentsArrayOfIds.length || false}
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
								checked={studentIds.has(item.user.id) || false}
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
 		let buttonState = `${styles["students-action"]}`;
		studentIds.size > 0 ? buttonState += ` ${styles["_active"]}` : buttonState;

		return (
		<React.Fragment>
			<Button
				use="link"
				// className={buttonState}
				disabled={studentIds.size === 0}
				onClick={this.onOpenModal}>
				<Gapped gap={3}>
					<Icon name="Copy" />
					<span className={styles["students-action__text"]}>Скопировать в группу...</span>
				</Gapped>
			</Button>
			<Button
				use="link"
				// className={buttonState}
				disabled={studentIds.size === 0}
				onClick={this.onDeleteStudents}
				>
				<Gapped gap={3}>
					<Icon name="Trash" />
					<span className={styles["students-action__text"]}>Исключить из группы</span>
				</Gapped>
			</Button>
		</React.Fragment>
		);
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
		const copyStudents = new Set([...studentIds]);

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