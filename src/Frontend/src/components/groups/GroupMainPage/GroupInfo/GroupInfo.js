import React, { Component } from "react";
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import Icon from "@skbkontur/react-icons";
import Kebab from "@skbkontur/react-ui/components/Kebab/Kebab";
import MenuItem from "@skbkontur/react-ui/components/MenuItem/MenuItem";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import getPluralForm from "../../../../utils/getPluralForm";

import styles from "./style.less";

class GroupInfo extends Component {

	render() {
		const { group } = this.props;

		if (!group) {
			return null;
		}

		const studentsCount = group.students_count || 0;
		const pluralFormOfStudents = getPluralForm(studentsCount, 'студент', 'студента', 'студентов');
		const isCodeReviewEnabled = group.is_manual_checking_enabled;
		const isProgressEnabled = group.can_users_see_group_progress;

		return (
			<div className={styles.wrapper}>
				<div className={styles["content-wrapper"]}>
					<Link className={styles["link-to-group-page"]} to={`/${this.props.courseId}/groups/${group.id}`} />
					<div className={styles["content-block"]}>
						<header className={styles.content}>
							<h3 className={styles["group-name"]}>{group.name}</h3>
							<div className={styles["students-count"]}>
								{studentsCount} {pluralFormOfStudents}
							</div>
							{this.renderTeachers()}
						</header>
						<div className={styles["group-settings"]}>
							{this.renderSetting(isProgressEnabled, isProgressEnabled ?
								'Ведомость включена': 'Ведомость выключена')}
							{this.renderSetting(isCodeReviewEnabled, isCodeReviewEnabled ?
								'Код-ревью включено' : 'Код-ревью выключено')}
						</div>
					</div>
				</div>
				{this.renderActions()}
			</div>
		)
	}

	renderTeachers() {
		const { group } = this.props;
		const teachersList = group.accesses.map(item => item.user.visible_name);
		const owner = group.owner.visible_name || 'Неизвестный';
		const teachers = [owner, ...teachersList];
		const teachersCount = teachers.length;
		const pluralFormOfTeachers = getPluralForm(teachersCount, 'преподаватель', 'преподаватели');

		return (
			<div className={styles["teachers-list"]}>
				{`${pluralFormOfTeachers}: ${teachers.join(', ')}`}
			</div>
		)
	}

	renderSetting(enabled, text) {
		return (
			<div className={enabled ? styles["settings-on"] : styles["settings-off"]}>
				<Gapped gap={5}>
					{ enabled ? <Icon name="Ok"/> : <Icon name="Delete"/> }
					{text}
				</Gapped>
			</div>
		)
	}

	renderActions() {
		const { group } = this.props;

		return (
			<div className={styles["group-action"]}>
				<Kebab size="large">
					<MenuItem onClick={() => this.props.toggleArchived(group, !group.is_archived)}>
						<Gapped gap={5}>
							<Icon name="ArchiveUnpack" />
							{group.is_archived ? 'Восстановить' : 'Архивировать'}
						</Gapped>
					</MenuItem>
					<MenuItem onClick={() => this.props.deleteGroup(group, group.is_archived ?
						'archiveGroups' : 'groups')}>
						<Gapped gap={5}>
							<Icon name="Delete" />
							Удалить
						</Gapped>
					</MenuItem>
				</Kebab>
			</div>
		)
	}
}

GroupInfo.propTypes = {
	courseId: PropTypes.string.isRequired,
	group: PropTypes.object.isRequired,
	deleteGroup: PropTypes.func,
	toggleArchived: PropTypes.func,
};

export default GroupInfo;