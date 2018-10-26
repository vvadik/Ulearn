import {Component} from "react";
import Input from "@skbkontur/react-ui/components/Input/Input";
import Checkbox from "@skbkontur/react-ui/components/Checkbox/Checkbox";
import React from "react";
import PropTypes from "prop-types";

const mapToServerName = {
	oldSolution: 'is_manual_checking_enabled_for_old_solutions',
	manualChecking: 'is_manual_checking_enabled',
	review: 'default_prohibit_further_review',
	progress: 'can_students_see_group_progress',
	name: 'name'
};

class GroupSettings extends Component {
	constructor(props) {
		super(props);
		this.bindProgress = this.onChange.bind(this, 'progress');
		this.bindManualChecking = this.onChange.bind(this, 'manualChecking');
		this.bindOldSolution = this.onChange.bind(this, 'oldSolution');
		this.bindReview = this.onChange.bind(this, 'review');
	}

	render() {
		const { group } = this.props;
		const oldSolution = group.is_manual_checking_enabled_for_old_solutions || false;
		const manualChecking = group.is_manual_checking_enabled || false;
		const review = group.default_prohibit_further_review || false;
		const progress = group.can_students_see_group_progress || false;

		return (
			<div className="group-settings-wrapper">
				<div className="group-name-field">
					<div className="group-name-label">
						<label htmlFor="groupName">Название группы</label>
					</div>
					<Input
						id="groupName"
						type="text"
						size="small"
						width="80%"
						placeholder="Здесь можно изменить название группы"
						onChange={this.onChangeInput}
					/>
				</div>
				<div className="check-block">
					<h3>Код-ревью и проверка тестов</h3>
					{ this.renderCheckbox(progress, "Открыть ведомость курса студентам", this.bindProgress) }
					{ this.renderCheckbox(manualChecking,
						"Включить код-ревью и ручную проверку тестов для участников группы",
						this.bindManualChecking) }
					{ this.renderCheckbox(oldSolution,
						"Отправить на код-ревью и ручную проверку тестов старые решения участников",
						this.bindOldSolution) }
					{this.renderCheckbox(review, "По умолчанию запрещать второе прохождение код-ревью",
					this.bindReview)}
				</div>
				<div className="points-block">
					<h3>Баллы</h3>
				</div>
			</div>
		)
	}

	renderCheckbox(checked, text, callback) {
		return (
			<Checkbox checked={checked} onChange={callback}>{text}</Checkbox>
		)
	}

	onChange = (field, _, value) => {
		this.props.onChangeSettings(mapToServerName[field], value);
	};

	onChangeInput = (_, value) => {
		this.props.onChangeSettings('name', value);
	};
}

GroupSettings.propTypes = {
	group: PropTypes.object,
	onChangeSettings: PropTypes.func
};

export default GroupSettings;