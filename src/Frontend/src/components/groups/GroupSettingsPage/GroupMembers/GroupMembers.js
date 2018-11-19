import React, {Component} from "react";
import PropTypes from "prop-types";
import Input from "@skbkontur/react-ui/components/Input/Input";
import Toggle from "@skbkontur/react-ui/components/Toggle/Toggle";
import Avatar from "./Avatar";

import './style.less';

class GroupMembers extends Component {

	render() {
		const { group } = this.props;
		console.log(group);
		const owner = group.owner;
		const inviteLink = group.is_invite_link_enabled || false;

		return (
			<div className="group-settings-wrapper">
				<div className="teachers-block">
					<h4>Преподаватели</h4>
					<p>
						Преподаватели могут видеть список участников группы, проводить код-ревью
						и проверку тестов, выставлять баллы и смотреть ведомость.
					</p>
					<div className="teacher-block">
						<Avatar user={owner} />
						<div className="teacher-block-name">
							<div>{owner.visible_name}</div>
							<span>Владелец</span>
						</div>
					</div>
					{(group.accesses.length > 0) && this.renderTeachers(group)}
					<label>
						<p>Добавить преподавателя:</p>
						<Input size="small" width="100%" placeholder="Начните вводить имя, фамилию или логин преподавателя "/>
					</label>
				</div>
				<div className="students-block">
					<h4>Студенты</h4>
					<div className="students-block-invite">
						<p>Отправьте своим студентам ссылку для вступления в группу:</p>
						{inviteLink &&
						<Input
							type="text"
							value={`https://ulearn.me/Account/JoinGroup?hash=${this.props.group.invite_hash}`}
							readOnly
							// selectAllOnFocus={true}
						/>
						}
						<div className="toggle-invite">
							<Toggle
								checked={inviteLink}
								onChange={this.toggleHash}
								color="default">
							</Toggle>
							Ссылка для вступления в группу {inviteLink ? 'включена' : 'выключена'}
						</div>
					</div>
				</div>
			</div>
		)
	}

	renderTeachers(group) {

		return (group.accesses.map(item =>
			<React.Fragment
				key={item.user.id}>
				<div className="teacher-block">
					<Avatar
						user={item.user}/>
					<div className="teacher-block-name">
						<div>{item.user.visible_name}</div>
						<span>Полный доступ предоставлен</span>
					</div>
				</div>
			</React.Fragment>
			)
		)
	};

	toggleHash = (value) => {
		this.props.onChangeSettings('is_invite_link_enabled', value);
	};
}

GroupMembers.propTypes = {
	group: PropTypes.object
};

export default GroupMembers;