import React, {Component} from "react";
import PropTypes from "prop-types";
import Input from "@skbkontur/react-ui/components/Input/Input";
import Loader from "@skbkontur/react-ui/components/Loader/Loader";
import GroupScores from "./GroupScores/GroupScores";
import GroupSettingsCheckbox from "./GroupSettingsCheckbox/GroupSettingsCheckbox";

import styles from './style.less';

class GroupSettings extends Component {
	render() {
		const { group, scores, onChangeSettings, error, loading } = this.props;

		return (
			<Loader type="big" active={loading}>
				<div className={styles.wrapper}>
					<div className={styles["changeName-block"]}>
						<div className={styles["changeName-header-block"]}>
							<label>
								<h4 className={styles["changeName-header"]}>Название группы</h4>
							</label>
						</div>
						<div className={styles["changeName-search"]}>
							<Input
								type="text"
								required
								size="small"
								error={error}
								value={this.inputValue}
								placeholder="Здесь вы можете изменить название группы"
								onChange={this.onChangeName} />
						</div>
					</div>
					<div className={`${styles["checkbox-block"]} ${styles.settings}`}>
						<h4 className={styles["settings-header"]}>Код-ревью и проверка тестов</h4>
						<GroupSettingsCheckbox
							group={group}
							onChangeSettings={onChangeSettings}/>
					</div>
					<div className={styles.settings}>
						<h4 className={styles["settings-header"]}>Баллы</h4>
						<p className={styles["settings-text"]}>Преподаватели могут выставлять студентам группы следующие баллы:</p>
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
		const { name } = this.props;

		return name || '';
	}

	onChangeName = (_, value) => {
		this.props.onChangeName(value);
	};
}

GroupSettings.propTypes = {
	name: PropTypes.string,
	error: PropTypes.bool,
	loading: PropTypes.bool,
	group: PropTypes.object,
	scores: PropTypes.array,
	onChangeSettings: PropTypes.func,
	onChangeName: PropTypes.func,
	onChangeScores: PropTypes.func,
};

export default GroupSettings;