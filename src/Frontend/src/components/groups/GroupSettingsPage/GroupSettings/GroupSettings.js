import React, {Component} from "react";
import PropTypes from "prop-types";
// import api from "../../../../api";
import Input from "@skbkontur/react-ui/components/Input/Input";
import Checkbox from "@skbkontur/react-ui/components/Checkbox/Checkbox";
import GroupScores from "./GroupScores";

import './style.less';

const mapToServerName = {
	oldSolution: 'is_manual_checking_enabled_for_old_solutions',
	manualChecking: 'is_manual_checking_enabled',
	review: 'default_prohibit_further_review',
	progress: 'can_students_see_group_progress',
	name: 'name',
};

class GroupSettings extends Component {
	constructor(props) {
		super(props);
		this.bindProgress = this.onChange.bind(this, 'progress');
		this.bindManualChecking = this.onChange.bind(this, 'manualChecking');
		this.bindOldSolution = this.onChange.bind(this, 'oldSolution');
		this.bindReview = this.onChange.bind(this, 'review');

		this.state = {
			name: '',
		}
	}

	render() {
		const { group, scores } = this.props;
		const oldSolution = group.is_manual_checking_enabled_for_old_solutions || false;
		const manualChecking = group.is_manual_checking_enabled || false;
		const review = group.default_prohibit_further_review || false;
		const progress = group.can_students_see_group_progress || false;

		return (
			<div className="group-settings-wrapper">
				<form>
					<div className="group-name-field">
						<div className="group-name-label">
							<label htmlFor="groupName">
								<h4>Название группы</h4>
							</label>
						</div>
						<Input
							id="groupName"
							type="text"
							size="small"
							value={group.name}
							placeholder="Здесь можно изменить название группы"
							onChange={this.onChangeInput}
						/>
					</div>
					<div className="check-block points-block">
						<h4>Код-ревью и проверка тестов</h4>
						{ this.renderSettings(progress, "Открыть ведомость курса студентам", this.bindProgress) }
						{ this.renderSettings(manualChecking,
							"Включить код-ревью и ручную проверку тестов для участников группы",
							this.bindManualChecking) }
						{ this.renderSettings(oldSolution,
							"Отправить на код-ревью и ручную проверку тестов старые решения участников",
							this.bindOldSolution) }
							<p className="points-block-comment">Если эта опция выключена, то при вступлении
								студента в группу его старые решения не будут отправлены на код-ревью</p>
						{this.renderSettings(review, "По умолчанию запрещать второе прохождение код-ревью",
						this.bindReview)}
						<p className="points-block-comment">В каждом код-ревью вы сможете выбирать,
							разрешить ли студенту второй раз отправить свой код на проверку.
							Эта опция задаёт лишь значение по умолчанию</p>
					</div>
					<div className="points-block">
						<h4>Баллы</h4>
						<p>Преподаватели могут выставлять студентам группы следующие баллы:</p>
						{scores && scores.map(score =>
							<GroupScores
								key={score.id}
								score={score}
								onChangeScores={this.props.onChangeScores}
							/>
						)}
					</div>
				</form>
			</div>
		)
	}

	renderSettings(checked, text, callback) {
		return (
			<Checkbox checked={checked} onChange={callback}>{text}</Checkbox>
		)
	}

	onChange = (field, _, value) => {
		this.props.onChangeSettings(mapToServerName[field], value);
	};

	onChangeInput = (_, value) => {
		this.setState({
			name: value,
		});
	};
}

GroupSettings.propTypes = {
	group: PropTypes.object,
	onChangeSettings: PropTypes.func
};

export default GroupSettings;

