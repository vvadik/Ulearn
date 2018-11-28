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
		const {score} = this.props;

		return (
			<label>
				{(score.id === 'exercise') ? this.renderExerciseScore() : this.renderOtherScores()}
				<p className="points-block-comment">{score.description}</p>
			</label>
		);
	}

	renderOtherScores() {
		const {score} = this.props;
		const changeScores = score.are_additional_scores_enabled_in_this_group || false;

		return (
			<Checkbox
				checked={changeScores}
				onChange={this.onChange}>
				{this.renderText()}
			</Checkbox>
		)
	}

	renderExerciseScore() {
		const {score} = this.props;
		const checkedScores = score.are_additional_scores_enabled_in_all_group || false;

		return (
			<React.Fragment>
				<Checkbox
					checked={checkedScores}
					disabled>
					{this.renderText()}
				</Checkbox>
				<div className="points-block-comment exercise-comment">
					<p>{mapToServerName.allGroupScores && 'Баллы включены для всех автором курса, и преподаватель ' +
					'не может отдельно включить или выключить эти баллы только в своей группе.'}</p>
					<p>{mapToServerName.unitScores &&
					'В курсе нет ни одного модуля, в которых преподаватель мог бы выставлять баллы этого типа.'}</p>
				</div>
			</React.Fragment>
		);
	}

	renderText() {
		const {score} = this.props;
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