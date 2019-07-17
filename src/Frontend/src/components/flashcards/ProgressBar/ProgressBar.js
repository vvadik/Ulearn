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
	rate5: styles.rate5,
};

const mapRateToText = {
	notRated: 'непросмотрено',
	rate1: 'плохо',
	rate2: 'удовлетворительно',
	rate3: 'средне',
	rate4: 'хорошо',
	rate5: 'отлично',
};

function ProgressBar({statistics, totalFlashcardsCount}) {
	return (
		<ol className={styles.progressBarContainer}>
			{renderResults()}
		</ol>
	);

	function renderResults() {
		const rates = Object.keys(statistics)
			.filter(key => statistics[key] > 0);
		const lowestOrderIndex = rates['notRated']
			? 1
			: 0;
		const highestOrderIndex = rates.length - 1;

		return rates.map((key, index) => convertToBarElement(
			key,
			statistics[key],
			totalFlashcardsCount,
			index, {
				lowestOrderIndex,
				highestOrderIndex
			}));
	}

	function convertToBarElement(rate, count, totalCardsCount, index, {lowestOrderIndex, highestOrderIndex}) {
		const elementWidth = `${count / totalCardsCount * 100}%`;

		const statusText = (index === lowestOrderIndex || index === highestOrderIndex || rate === 'notRated') && mapRateToText[rate];

		return (
			<span
				key={rate}
				className={classNames(styles.progressBarElement, mapRateToStyle[rate])}
				style={{width: elementWidth}}>
				{count} {statusText}
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
