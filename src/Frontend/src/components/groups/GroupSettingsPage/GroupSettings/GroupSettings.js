import React, {Component} from "react";
import PropTypes from "prop-types";
import Input from "@skbkontur/react-ui/components/Input/Input";
import GroupScores from "./GroupScores";
import GroupSettingsCheckbox from "./GroupSettingsCheckbox";

import './style.less';

class GroupSettings extends Component {

	render() {
		const { group, scores, onChangeSettings } = this.props;

		return (
			<div className="group-settings-wrapper">
				<form>
					<div className="group-name-field">
						<div className="group-name-label">
							<label>
								<h4>Название группы</h4>
							</label>
						</div>
						<Input
							type="text"
							size="small"
							// value={group.name}
							placeholder="Здесь вы можете изменить название группы"
							onChange={this.onChangeName}
						/>
					</div>
					<div className="check-block points-block">
						<h4>Код-ревью и проверка тестов</h4>
						<GroupSettingsCheckbox
							group={group}
							onChangeSettings={onChangeSettings}/>
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

	onChangeName = (_, value) => {
		this.props.onChangeName(value);
	};
}

GroupSettings.propTypes = {
	group: PropTypes.object,
	scores: PropTypes.array,
	onChangeSettings: PropTypes.func,
	onChangeName: PropTypes.func,
	onChangeScores: PropTypes.func,
};

export default GroupSettings;