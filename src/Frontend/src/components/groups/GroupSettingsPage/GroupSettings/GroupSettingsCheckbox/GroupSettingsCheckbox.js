import React, {Component} from "react";
import PropTypes from "prop-types";
import Checkbox from "@skbkontur/react-ui/components/Checkbox/Checkbox";

import styles from './groupSettingsCheckbox.less';

const mapToServerName = {
	oldSolution: 'is_manual_checking_enabled_for_old_solutions',
	manualChecking: 'is_manual_checking_enabled',
	review: 'default_prohibit_further_review',
	progress: 'can_students_see_group_progress',
};

class GroupSettingsCheckbox extends Component {
	constructor(props) {
		super(props);
		this.bindProgress = this.onChange.bind(this, 'progress');
		this.bindManualChecking = this.onChange.bind(this, 'manualChecking');
		this.bindOldSolution = this.onChange.bind(this, 'oldSolution');
		this.bindReview = this.onChange.bind(this, 'review');
	}

	render () {
		const { group } = this.props;
		const manualChecking = group.is_manual_checking_enabled || false;
		const progress = group.can_students_see_group_progress || false;

		return (
			<React.Fragment>
				<label className={styles["settings-checkbox"]}>
					{this.renderSettings(progress, "Открыть ведомость курса студентам", this.bindProgress)}
				</label>
				<label className={styles["settings-checkbox"]}>
				{ this.renderSettings(manualChecking,
					"Включить код-ревью и ручную проверку тестов для участников группы",
					this.bindManualChecking) }
				</label>
				{manualChecking && this.renderReviewSettings()}
			</React.Fragment>
		)
	}

	renderReviewSettings() {
		const { group } = this.props;
		const oldSolution = group.is_manual_checking_enabled_for_old_solutions || false;
		const review = group.default_prohibit_further_review || false;

		return (
			<React.Fragment>
				<label className={styles["settings-checkbox"]}>
					{ this.renderSettings(oldSolution,
						"Отправить на код-ревью и ручную проверку тестов старые решения участников",
						this.bindOldSolution) }
					<p className={styles["settings-comment"]}>Если эта опция выключена, то при вступлении
						студента в группу его старые решения не будут отправлены на код-ревью</p>
				</label>
				<label className={styles["settings-checkbox"]}>
					{this.renderSettings(review, "По умолчанию запрещать второе прохождение код-ревью",
						this.bindReview)}
					<p className={styles["settings-comment"]}>В каждом код-ревью вы сможете выбирать,
						разрешить ли студенту второй раз отправить свой код на проверку.
						Эта опция задаёт лишь значение по умолчанию</p>
				</label>
			</React.Fragment>
		)
	};

	renderSettings(checked, text, callback) {
		return (
			<Checkbox checked={checked} onChange={callback}>{text}</Checkbox>
		)
	};

	onChange = (field, _, value) => {
		const { onChangeSettings } = this.props;
		onChangeSettings(mapToServerName[field], value);
	};
}

GroupSettingsCheckbox.propTypes = {
	group: PropTypes.object,
	onChangeSettings: PropTypes.func,
};

export default GroupSettingsCheckbox;

