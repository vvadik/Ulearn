import { Component } from "react";
import PropTypes from 'prop-types';
import { getPluralForm } from "../../../utils/getPluralForm";
import Icon from "@skbkontur/react-ui/components/Icon/Icon";
import Kebab from "@skbkontur/react-ui/components/Kebab/Kebab";
import MenuItem from "@skbkontur/react-ui/components/MenuItem/MenuItem";
import React from "react";

class GroupInfo extends Component {

	render() {
		const { group } = this.props;
		if (!group) {
			return null;
		}

		const studentsCount = group.students_count || 0;
		const pluralFormOfStudents = getPluralForm(studentsCount, 'студент', 'студента', 'студентов');
		const owner = group.owner.visible_name || 'Неизвестный';
		const groupName = group.name;
		const teachersList = group.accesses.map(item => item.user.visible_name);
		const teachers = [owner, ...teachersList];

		return (
			<div className ="group">
				<div className="group-content">
					<h3 className="group-content__name">{ groupName }</h3>
					<div className="group-content__count">
						{studentsCount} {pluralFormOfStudents}
					</div>
					<div className="group-content__teachers">
						Преподаватели: { teachers.join(', ') }
					</div>
					<div className="group-content__state">
						{ this.renderSetting(group.can_users_see_group_progress, true) }
						{ this.renderSetting(group.is_manual_checking_enabled, false) }
					</div>
				</div>
				<div className="group-action">
					<Kebab size="large">
						<MenuItem icon="ArchiveUnpack">Архивировать</MenuItem>
						<MenuItem icon="Delete">Удалить</MenuItem>
					</Kebab>
				</div>
			</div>
		)
	}

	renderSetting(enabled, isProgress) {
		if (enabled) {
			return (
			<div className="group-content__state_on">
				<Icon name="Ok"/>
				{isProgress ? 'Ведомость включена' : 'Код-ревью включено'}
			</div>
			)
		} else {
			return (
			<div className="group-content__state_off">
				<Icon name="Delete"/>
				{isProgress ? 'Ведомость выключена' : 'Код-ревью выключено'}
			</div>
			)
		}
	}
}

GroupInfo.propTypes = {
	group: PropTypes.object.isRequired
};

export default GroupInfo;