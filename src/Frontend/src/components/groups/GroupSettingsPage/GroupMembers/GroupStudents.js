import React, {Component} from "react";
import PropTypes from "prop-types";
import moment from "moment";
import 'moment/locale/ru';
import "moment-timezone";
import Checkbox from "@skbkontur/react-ui/components/Checkbox/Checkbox";
import Kebab from "@skbkontur/react-ui/components/Kebab/Kebab";
import MenuItem from "@skbkontur/react-ui/components/MenuItem/MenuItem";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import Select from "@skbkontur/react-ui/components/Select/Select";
import Icon from "@skbkontur/react-icons";
import Avatar from "./Avatar";
import getWordForm from "../../../../utils/getWordForm";

import styles from './style.less';
import CopyGroupModal from "../../GroupMainPage/CopyGroupModal/CopyGroupModal";

export default class GroupStudents extends Component {

	state = {
		checked: false,
		modalOpen: false,
		course: '',
		value: '',
		studentsId: [],
	};

	render() {
		const { students } = this.props;
		const grantTime = (grantTime) => moment.tz(grantTime, 'Europe/Moscow').tz('Asia/Yekaterinburg').format();

		return(
			<React.Fragment>
				<div>
					<div className={styles["students-actions"]}>
						<Checkbox
							checked={this.state.checked}
							onChange={(_, value) => this.setState({checked: value })}>
							<span>Выбрать всех</span>
						</Checkbox>
						{this.renderStudentActions()}
					</div>
					<div>
						{students.map(item =>
						<div className={styles["student-block"]}
							key={item.user.id}>
							<Checkbox
								checked={this.state.checked}
								onChange={this.onCheckStudent}>
								<Avatar user={item.user} size={styles["_small"]}/>
								{ item.user.visible_name } {' '}
								<span className={styles["students-action__text"]}>{ getWordForm('вступила', 'вступил', item.user.gender) }
									{' '} { moment(grantTime(item.adding_time)).fromNow() }</span>
							</Checkbox>
							<Kebab size="large" >
								<MenuItem onClick={() => this.props.onDeleteStudent(item.user.id)}>
									<Gapped gap={5}>
									<Icon name="Delete" />
									Удалить
									</Gapped>
								</MenuItem>
							</Kebab>
						</div>
					)}
					</div>
				</div>
				{ this.state.modalOpen && <CopyGroupModal onClose={this.onCloseModal}/> }
			</React.Fragment>
		)
	}

	renderStudentActions() {
		let buttonState = `${styles["students-action"]}`;
		if (this.state.checked) {
			buttonState = `${styles["students-action"]} ${styles["_active"]}`
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
				onClick={() => this.props.onDeleteStudents}
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

	onCheckStudent = (_, value, item) => {
		const { studentsId, checked } = this.state;
		this.setState({
			checked: value,
		});
		if(checked) {
			this.setState({
				studentsId: [...studentsId, item.user.id],
			});
		}
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