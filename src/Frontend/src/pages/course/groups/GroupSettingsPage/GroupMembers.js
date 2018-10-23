import {Component} from "react";
import Input from "@skbkontur/react-ui/components/Input/Input";
import React from "react";

class GroupMembers extends Component {
	constructor(props) {
		super(props);
	}

	render() {
		return (
			<div>
				<div className="teachers-block">
					<h3>Преподаватели</h3>
					<p>
						Преподаватели могут видеть список участников группы, проводить код-ревью
						и проверку тестов, выставлять баллы и смотреть ведомость.
					</p>
					<Input size="small" width="100%" placeholder="Начните вводить имя, фамилию или логин преподавателя "/>
				</div>
				<div className="students-block">
					<h3>Студенты</h3>
					<p>Отправьте своим студентам ссылку для вступления в группу:</p>
				</div>
			</div>
		)
	}
}

GroupMembers.propTypes = {};

export default GroupMembers;