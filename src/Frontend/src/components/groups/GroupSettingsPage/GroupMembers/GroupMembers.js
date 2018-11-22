import React, {Component} from "react";
import PropTypes from "prop-types";
import Icon from "@skbkontur/react-icons";
import moment from "moment";
import 'moment/locale/ru';
import Input from "@skbkontur/react-ui/components/Input/Input";
import Toggle from "@skbkontur/react-ui/components/Toggle/Toggle";
import Kebab from "@skbkontur/react-ui/components/Kebab/Kebab";
import MenuItem from "@skbkontur/react-ui/components/MenuItem/MenuItem";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import Avatar from "./Avatar";

import './style.less';

class GroupMembers extends Component {

	render() {
		const { group, accesses } = this.props;
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
							<span className="teacher-status">Владелец</span>
						</div>
					</div>
					{(accesses.length > 0) && this.renderTeachers()}
					<label className="teacher-block-search">
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
							value={`https://ulearn.me/Account/JoinGroup?hash=${group.invite_hash}`}
							readOnly
							selectAllOnFocus
						/>
						}
						<div className="toggle-invite">
							<label className="toggle-label">
								<Toggle
									checked={inviteLink}
									onChange={this.toggleHash}
									color="default">
								</Toggle>
								Ссылка для вступления в группу {inviteLink ? 'включена' : 'выключена'}
							</label>
						</div>
					</div>
				</div>
			</div>
		)
	}

	renderTeachers() {
		const { accesses } = this.props;

		return (accesses.map(item =>
			<React.Fragment
				key={item.user.id}>
				<div className="teacher-block">
					<Avatar
						user={item.user}/>
					<div className="teacher-block-name">
						<div>{item.user.visible_name}</div>
						<span className="teacher-status">
							Полный доступ предоставил(а) {item.granted_by.visible_name} {moment().startOf(item.grant_time).fromNow()}
						</span>
					</div>
					<div className="group-action">
						{this.renderKebab(item.user)}
					</div>
				</div>
			</React.Fragment>
			)
		)
	};

	renderKebab(user) {
		return (
			<Kebab size="large">
				<MenuItem onClick={() => this.props.onChangeOwner(user)}>
					<Gapped gap={5}>
						<Icon name="User" />
						Сделать владельцем
					</Gapped>
				</MenuItem>
				<MenuItem onClick={() => this.props.onRemoveTeacher(user)}>
					<Gapped gap={5}>
					<Icon name="Delete" />
					Забрать доступ
					</Gapped>
				</MenuItem>
			</Kebab>
		)
	}

	toggleHash = (value) => {
		this.props.onChangeSettings('is_invite_link_enabled', value);
	};
}

GroupMembers.propTypes = {
	group: PropTypes.object,
	accesses: PropTypes.array,
	onChangeSettings: PropTypes.func,
	onChangeOwner: PropTypes.func,
	onRemoveTeacher: PropTypes.func,
};

export default GroupMembers;