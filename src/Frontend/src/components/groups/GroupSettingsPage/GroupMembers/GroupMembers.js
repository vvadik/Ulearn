import {Component} from "react";
import Input from "@skbkontur/react-ui/components/Input/Input";
import React from "react";
import PropTypes from "prop-types";

import './style.less';

class GroupMembers extends Component {

	render() {
		const { group } = this.props;
		const owner = group.owner;

		return (
			<div className="group-settings-wrapper">
				<div className="teachers-block">
					<h3>Преподаватели</h3>
					<p>
						Преподаватели могут видеть список участников группы, проводить код-ревью
						и проверку тестов, выставлять баллы и смотреть ведомость.
					</p>
					<div className="teacher-block">
						<img alt="фото" src={owner.avatar_url} />
						<div className="teacher-block-name">
							<div>{owner.visible_name}</div>
							<span>Владелец</span>
						</div>
					</div>
					{this.renderTeachers(group)}
					<Input size="small" width="100%" placeholder="Начните вводить имя, фамилию или логин преподавателя "/>
				</div>
				<div className="students-block">
					<h3>Студенты</h3>
					<div className="students-block-invite">
						<span>Отправьте своим студентам ссылку для вступления в группу:</span>
						<a>https://ulearn.me/Account/JoinGroup?hash={this.props.group.invite_hash}</a>
					</div>
				</div>
			</div>
		)
	}

	renderTeachers(group) {
		return( group.accesses.map(item => {
			const teacher = item.user;
			return (
				<div className="teacher-block">
					<img alt="фото" src={teacher.avatar_url} />
					<div className="teacher-block-name">
						<div>{teacher.visible_name}</div>
						<span>Полный доступ предоставлен</span>
					</div>
				</div>
			);
		})
	)};
}

GroupMembers.propTypes = {
	group: PropTypes.object
};

export default GroupMembers;