import React, {Component} from "react";
import PropTypes from "prop-types";
import Checkbox from "@skbkontur/react-ui/components/Checkbox/Checkbox";

import './style.less';

const mapToServerName = {
	groupScores: 'are_additional_scores_enabled_in_this_group',
	allGroupScores: 'are_additional_scores_enabled_for_all_groups',
	unitScores: 'can_instructor_set_additional_score_in_some_unit',
};

class GroupScores extends Component {

	render() {
		const { score } = this.props;
		const changeScores = score.are_additional_scores_enabled_in_this_group || false;

		return(
			<label>
				<Checkbox
					checked={changeScores}
					onChange={this.onChange}
					disabled={score.id === 'exercise'}>
					{this.renderText()}
				</Checkbox>
				<p className="points-block-comment">{ (score.id === 'exercise') ? this.renderExerciseScore() : score.description }</p>
			</label>
		);
	}

	renderText() {
		const { score } = this.props;
		switch (score.id) {
			case 'activity':
				return 'Практика';
			case 'seminar':
				return 'Семинары';
			case 'exercise':
				return 'Упражнения';
			case 'homework':
				return 'Домашние задания';
			default:
				return null;
		}
	}

	renderExerciseScore() {
		if (mapToServerName.allGroupScores) {
			return 'Баллы включены для всех автором курса, и преподатель ' +
				'не может отдельно включить или выключить эти баллы только в своей группе';
		} else if (mapToServerName.unitScores) {
			return 'В курсе нет ни одного модуля, в которых преподаватель мог бы выставлять баллы этого типа';
		}
	}

	onChange = (_, value) => {
		const key = this.props.score.id;
		this.props.onChangeScores(key, mapToServerName.groupScores, value);
	};
}

export default GroupScores;