import React from 'react'
import PropTypes from "prop-types";
import styles from './progressBar.less'
import classNames from 'classnames';

const mapRateToStyle = {
	notRated: styles.notRated,
	rate1: styles.rate1,
	rate2: styles.rate2,
	rate3: styles.rate3,
	rate4: styles.rate4,
	rate5: styles.rate5
};

const mapStatusToText = {
	notRated: 'непросмотрено',
	rate1: 'плохо',
	rate5: 'отлично'
};

function ProgressBar({statistics, totalFlashcardsCount}) {
	return (
		<ol className={styles.progressBarContainer}>
			{renderResults()}
		</ol>
	);

	function renderResults() {
		return Object
			.keys(statistics)
			.filter(key => statistics[key] > 0)
			.map(key => convertToBarElement(key, statistics[key], totalFlashcardsCount));
	}

	function convertToBarElement(status, count, cardsCount) {
		const elementWidth = `${count / cardsCount * 100}%`;

		return (
			<span
				key={status}
				className={classNames(styles.progressBarElement, mapRateToStyle[status])}
				style={{width: elementWidth}}>
				{count} {mapStatusToText[status]}
			</span>
		);
	}
}

ProgressBar.propTypes = {
	statistics: PropTypes.shape({
		notRated: PropTypes.number,
		rate1: PropTypes.number,
		rate2: PropTypes.number,
		rate3: PropTypes.number,
		rate4: PropTypes.number,
		rate5: PropTypes.number
	}).isRequired,
	totalFlashcardsCount: PropTypes.number.isRequired
};

export default ProgressBar;
