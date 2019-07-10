import React, {Component} from 'react';
import PropTypes from "prop-types";
import styles from './results.less';
import classNames from 'classnames';
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";

const resultsStyles = [
	classNames(styles.resultsElement, styles.resultsVeryBad),
	classNames(styles.resultsElement, styles.resultsBad),
	classNames(styles.resultsElement, styles.resultsAverage),
	classNames(styles.resultsElement, styles.resultsGood),
	classNames(styles.resultsElement, styles.resultsExcellent)
];

function Results({handleClick}) {
	return (
		<div className={styles.resultsContainer}>
			<p>
				Оцените, на сколько хорошо вы знали ответ?
			</p>
			{resultsStyles.map(convertToResultIcon)}
			{renderFooter()}
		</div>
	);

	function convertToResultIcon(style, index) {
		return (
			<button key={index} className={style} onClick={() => handleClick(index + 1)}>
				<span>
					{index + 1}
				</span>
			</button>
		)
	}

	function renderFooter() {
		return (
			<footer className={styles.footer}>
				<Gapped vertical={false} gap={15}>
					<span>
						плохо
					</span>
					<div className={styles.footerLine}/>
					<span>
						отлично
					</span>
				</Gapped>
			</footer>
		);
	}
}

Results.propTypes = {
	handleClick: PropTypes.func.isRequired
};

export default Results;
