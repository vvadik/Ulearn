import React from 'react'
import PropTypes from "prop-types";
import styles from './progressBar.less'
import classNames from 'classnames';

const mapStatusToStyle = {
	unseen: styles.statusUnseen,
	1: styles.statusVeryBad,
	2: styles.statusBad,
	3: styles.statusAverage,
	4: styles.statusGood,
	5: styles.statusExcellent,
};

const mapStatusToText = {
	unseen: 'непросмотрено',
	1: 'плохо',
	5: 'отлично'
};

function ProgressBar({byScore, total, className}) {
	return (
		<div className={classNames(styles.progressBarContainer, className)}>
			{renderResults()}
		</div>
	);

	function renderResults() {
		return Object
			.keys(byScore)
			.filter(key => byScore[key] > 0)
			.map(key => convertToBarElement(key, byScore[key], total));
	}

	function convertToBarElement(status, count, cardsCount) {
		const elementWidth = `${count / cardsCount * 100}%`;

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

ProgressBar.propTypes = {
	byScore: PropTypes.shape({
		unseen: PropTypes.number,
		1: PropTypes.number,
		2: PropTypes.number,
		3: PropTypes.number,
		4: PropTypes.number,
		5: PropTypes.number
	}).isRequired,
	total: PropTypes.number.isRequired
};

export default ProgressBar;
