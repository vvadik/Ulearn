import React, { Component } from "react";
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import Icon from "@skbkontur/react-ui/components/Icon/Icon";
import Kebab from "@skbkontur/react-ui/components/Kebab/Kebab";
import MenuItem from "@skbkontur/react-ui/components/MenuItem/MenuItem";
import getPluralForm from "../../../../utils/getPluralForm";

import './style.less';

class GroupInfo extends Component {

	render() {
		const { group } = this.props;
		if (!group) {
			return null;
		}

		const studentsCount = group.students_count || 0;
		const pluralFormOfStudents = getPluralForm(studentsCount, 'студент', 'студента', 'студентов');
		const teachersList = group.accesses.map(item => item.user.visible_name);
		const owner = group.owner.visible_name || 'Неизвестный';
		const teachers = [owner, ...teachersList];
		const teachersCount = teachers.length;
		const pluralFormOfTeachers = getPluralForm(teachersCount, 'преподаватель', 'преподаватели');
		const groupName = group.name;
		const groupId = group.id;
		const isCodeReviewEnabled = group.is_manual_checking_enabled;
		const isProgressEnabled = group.can_users_see_group_progress;

		return (
			<div className="group">
				<div className="group-content-wrapper">
					<Link to={`groups/${groupId}`}>
						<div className="group-content">
							<header className="group-content-main">
								<h3 className="group-content-main__name">{groupName}</h3>
								<div className="group-content-main__count">
									{studentsCount} {pluralFormOfStudents}
								</div>
								<div className="group-content-main__teachers">
									{`${pluralFormOfTeachers}: ${teachers.join(', ')}`}
								</div>
							</header>
							<div className="group-content-state">
								{this.renderSetting(isProgressEnabled, isProgressEnabled ? 'Ведомость включена': 'Ведомость выключена')}
								{this.renderSetting(isCodeReviewEnabled, isCodeReviewEnabled ? 'Код-ревью включено' : 'Код-ревью выключено')}
							</div>
						</div>
					</Link>
				</div>
				<div className="group-action">
					{this.renderKebab()}
				</div>
			</div>
		)
	}

	renderKebab() {
		const { group } = this.props;

		return (
			<Kebab size="large">
				<MenuItem icon="ArchiveUnpack"
						  onClick={() => this.props.toggleArchived(group, !group.is_archived)}>
					{group.is_archived ? 'Восстановить' : 'Архивировать'}
				</MenuItem>
				<MenuItem icon="Delete"
						  onClick={() => this.props.deleteGroup(group, group.is_archived ? 'archiveGroups' : 'groups')}>
					Удалить
				</MenuItem>
			</Kebab>
		)

	}

	renderSetting(enabled, text) {
		if (enabled) {
			return (
			<div className="group-content-state_on">
				<Icon name="Ok"/>
				{text}
			</div>
			)
		} else {
			return (
			<div className="group-content-state_off">
				<Icon name="Delete"/>
				{text}
			</div>
			)
		}
	}
 }

GroupInfo.propTypes = {
	group: PropTypes.object.isRequired,
	deleteGroup: PropTypes.func,
	toggleArchived: PropTypes.func,
};

export default GroupInfo;