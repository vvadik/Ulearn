import React, {Component} from "react";
import PropTypes from "prop-types";
import Checkbox from "@skbkontur/react-ui/components/Checkbox/Checkbox";

import styles from './groupScores.less';

const mapToServerName = {
	groupScores: 'are_additional_scores_enabled_in_this_group',
	allGroupScores: 'are_additional_scores_enabled_for_all_groups',
	unitScores: 'can_instructor_set_additional_score_in_some_unit',
};

const dictionary = {
	'activity': 'Практика',
	'seminar': 'Семинары',
	'exercise': 'Упражнения',
	'homework': 'Домашние задания',
};

class GroupScores extends Component {

	render() {
		const { score } = this.props;

		return (
			<label className={styles["settings-checkbox"]}>
				{(score.id === 'exercise') ? this.renderExerciseScores() : this.renderOtherScores()}
				<p className={styles["settings-comment"]}>{score.description}</p>
			</label>
		);
	}

	renderOtherScores() {
		const { score } = this.props;
		const id = score.id;
		const changeScores = score.are_additional_scores_enabled_in_this_group || false;

		return (
			<Checkbox
				checked={changeScores}
				onChange={this.onChange}>
				{dictionary[id]}
			</Checkbox>
		)
	}

	renderExerciseScores() {
		const {score} = this.props;
		const checkedScores = score.are_additional_scores_enabled_in_all_group || false;

		return (
			<React.Fragment>
				<Checkbox
					checked={checkedScores}
					disabled>
					{dictionary.exercise}
				</Checkbox>
				<div className={styles["settings-comment"]}>
					<p className={styles["exercise-comment"]}>{mapToServerName.allGroupScores && 'Баллы включены для всех автором курса, и преподаватель ' +
					'не может отдельно включить или выключить эти баллы только в своей группе.'}</p>
					<p className={styles["exercise-comment"]}>{mapToServerName.unitScores &&
					'В курсе нет ни одного модуля, в которых преподаватель мог бы выставлять баллы этого типа.'}</p>
				</div>
			</React.Fragment>
		);
	}

	onChange = (_, value) => {
		const id = this.props.score.id;
		this.props.onChangeScores(id, mapToServerName.groupScores, value);
	};
}

GroupScores.propTypes = {
	score: PropTypes.object,
	onChangeScores: PropTypes.func,
};

export default GroupScores;