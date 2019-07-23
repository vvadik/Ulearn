import React from 'react'
import PropTypes from "prop-types";

import classNames from 'classnames';
import { rateTypes } from '../../../consts/rateTypes';

import styles from './progressBar.less'

const mapRateToStyle = {
	[rateTypes.notRated]: styles.notRated,
	[rateTypes.rate1]: styles.rate1,
	[rateTypes.rate2]: styles.rate2,
	[rateTypes.rate3]: styles.rate3,
	[rateTypes.rate4]: styles.rate4,
	[rateTypes.rate5]: styles.rate5,
};

const mapRateToText = {
	[rateTypes.notRated]: 'непросмотрено',
	[rateTypes.rate1]: 'плохо',
	[rateTypes.rate2]: 'удовлетворительно',
	[rateTypes.rate3]: 'средне',
	[rateTypes.rate4]: 'хорошо',
	[rateTypes.rate5]: 'отлично',
};

function ProgressBar({ statistics, totalFlashcardsCount }) {
	return (
		<ol className={ styles.progressBarContainer }>
			{ renderResults() }
		</ol>
	);

	function renderResults() {
		const rates = Object.keys(statistics)
			.filter(rateType => statistics[rateType] > 0);

		const ratesWithText = [rateTypes.notRated];
		//TODO ROZENTOR implement better algorithm for selecting rates with text
		if (rates[0] === rateTypes.notRated) {
			ratesWithText.push(rates[1]);
		} else {
			ratesWithText.push(rates[0]);
		}

		if (rates[rates.length - 1] === rateTypes.notRated) {
			ratesWithText.push(rates[rates.length - 2]);
		} else {
			ratesWithText.push(rates[rates.length - 1]);
		}

		return rates.map(rate => convertToBarElement(
			rate,
			`${ statistics[rate] / totalFlashcardsCount * 100 }%`,
			`${ statistics[rate] } ${ ratesWithText.some(r => r === rate) ? mapRateToText[rate] : '' }`,
		));
	}

	function convertToBarElement(rate, elementWidth, text) {
		return (
			<span
				key={ rate }
				className={ classNames(styles.progressBarElement, mapRateToStyle[rate]) }
				style={ { width: elementWidth } }>
				{ text }
			</span>
		);
	}
}

ProgressBar.propTypes = {
	statistics: PropTypes.shape({
		[rateTypes.notRated]: PropTypes.number,
		[rateTypes.rate1]: PropTypes.number,
		[rateTypes.rate2]: PropTypes.number,
		[rateTypes.rate3]: PropTypes.number,
		[rateTypes.rate4]: PropTypes.number,
		[rateTypes.rate5]: PropTypes.number,
	}),
	totalFlashcardsCount: PropTypes.number,
};

export default ProgressBar;
