import React, {Component} from 'react'
import PropTypes from "prop-types";
import styles from './unitProgressBar.less'

const classNames = require('classnames');

const mapStatusToStyle = Object.freeze({
	0: styles.statusUnseen,
	1: styles.statusVeryBad,
	2: styles.statusBad,
	3: styles.statusAverage,
	4: styles.statusGood,
	5: styles.statusExcellent,
});

const mapStatusToText = Object.freeze({
	0: 'непросмотрено',
	1: 'плохо',
	5: 'отлично'
});

class UnitProgressBar extends Component {
	render() {
		return (
			<div>
				<span className={styles.progressBarTitle}>
					Результаты последнего прохождения
				</span>
				<div className={styles.progressBarContainer}>
					{this.renderResults()}
				</div>
			</div>
		);
	}

	renderResults() {
		const {byScore} = this.props;
		const cardsCount = byScore.reduce((accumulator, currentValue) => accumulator + currentValue);

		return Object
			.keys(byScore)
			.filter(key => byScore[key] > 0)
			.map(key => UnitProgressBar.convertToBarElement(key, byScore[key], cardsCount));
	}

	static convertToBarElement(status, count, cardsCount) {
		const elementWidth = `${100 * count / cardsCount}%`;

		return (
			<span
				key={status}
				className={classNames(styles.progressBarElement, mapStatusToStyle[status])}
				style={{width: elementWidth}}>
				{count} {mapStatusToText[status]}
			</span>
		);
	}

}

UnitProgressBar.propTypes = {
	byScore: PropTypes.array.isRequired
};

export default UnitProgressBar;