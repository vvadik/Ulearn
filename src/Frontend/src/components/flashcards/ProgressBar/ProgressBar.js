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

		const ratesWithText = [
			'notRated'
		];
		//TODO ROZENTOR implement better algorithm for selecting rates with text
		if (rates[0] === 'notRated') {
			ratesWithText.push(rates[1]);
		} else {
			ratesWithText.push(rates[0]);
		}

		if (rates[rates.length - 1] === 'notRated') {
			ratesWithText.push(rates[rates.length - 2]);
		} else {
			ratesWithText.push(rates[rates.length - 1]);
		}

		return rates.map(rate => convertToBarElement(
			rate,
			`${statistics[rate] / totalFlashcardsCount * 100}%`,
			`${statistics[rate]} ${ratesWithText.some(r => r === rate) ? mapRateToText[rate] : ''}`,
		));
	}

	function convertToBarElement(rate, elementWidth, text) {
		return (
			<span
				key={rate}
				className={classNames(styles.progressBarElement, mapRateToStyle[rate])}
				style={{width: elementWidth}}>
				{text}
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
