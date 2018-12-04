import React, {Component} from "react";
import PropTypes from "prop-types";
import Input from "@skbkontur/react-ui/components/Input/Input";
import GroupScores from "./GroupScores";
import GroupSettingsCheckbox from "./GroupSettingsCheckbox";
import Loader from "@skbkontur/react-ui/components/Loader/Loader";

import styles from './style.less';

class GroupSettings extends Component {
	render() {
		const { group, scores, onChangeSettings, error, loading } = this.props;

		return (
			<Loader type="big" active={loading}>
				<div className={styles["group-settings-wrapper"]}>
					<div className={styles["group-name-field"]}>
						<div className={styles["group-name-label"]}>
							<label>
								<h4>Название группы</h4>
							</label>
						</div>
						<Input
							type="text"
							required
							size="small"
							error={error}
							value={this.inputValue}
							placeholder="Здесь вы можете изменить название группы"
							onChange={this.onChangeName} />
					</div>
					<div className={`${styles["check-block"]} ${styles["points-block"]}`}>
						<h4>Код-ревью и проверка тестов</h4>
						<GroupSettingsCheckbox
							group={group}
							onChangeSettings={onChangeSettings}/>
					</div>
					<div className={styles["points-block"]}>
						<h4>Баллы</h4>
						<p>Преподаватели могут выставлять студентам группы следующие баллы:</p>
						{scores && scores.map(score =>
							<GroupScores
								key={score.id}
								score={score}
								onChangeScores={this.props.onChangeScores} />
						)}
					</div>
				</div>
			</Loader>
		)
	}

	get inputValue() {
		const { group, name } = this.props;

		return (name !== undefined ? name : group.name) || '';
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