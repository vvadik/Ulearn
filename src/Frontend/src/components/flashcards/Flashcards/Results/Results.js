import React from 'react';
import PropTypes from "prop-types";

import classNames from 'classnames';

import styles from './results.less';

const resultsStyles = [
	classNames(styles.resultsElement, styles.resultsVeryBad),
	classNames(styles.resultsElement, styles.resultsBad),
	classNames(styles.resultsElement, styles.resultsAverage),
	classNames(styles.resultsElement, styles.resultsGood),
	classNames(styles.resultsElement, styles.resultsExcellent)
];

function Results({ handleClick }) {
	return (
		<div className={ styles.resultsContainer }>
			<p className={ styles.headerText }>
				Оцените, на сколько хорошо вы знали ответ?
			</p>
			{ resultsStyles.map(convertToResultIcon) }
			{ renderFooter() }
		</div>
	);

	function convertToResultIcon(style, index) {
		return (
			<button key={ index } className={ style } onClick={ () => handleClick(index + 1) }>
				<span>
					{ index + 1 }
				</span>
			</button>
		)
	}

	function renderFooter() {
		return (
			<div className={ styles.footer }>
				плохо
				<hr className={ styles.footerLine }/>
				отлично
			</div>
		);
	}
}

Results.propTypes = {
	handleClick: PropTypes.func,
};

export default Results;
