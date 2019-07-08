import React, {Component} from 'react';
import PropTypes from "prop-types";
import styles from './results.less';
import classNames from 'classnames';
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";

const resultsStyles = [
	styles.resultsVeryBad,
	styles.resultsBad,
	styles.resultsAverage,
	styles.resultsGood,
	styles.resultsExcellent
];

function Results({handleClick}) {
	return (
		<div className={styles.flashcardContainer}>
			<p>
				Оцените, на сколько хорошо вы знали ответ?
			</p>
			{renderResultsIcons()}
			{renderFooter()}
		</div>
	);

	function renderResultsIcons() {
		return (
			<Gapped vertical={false} gap={25}>
				{resultsStyles.map(convertToResultIcon)}
			</Gapped>
		);
	}

	function convertToResultIcon(style, index) {
		const buttonStyle = classNames(styles.resultsElement, style);

		return (
			<button key={index} className={buttonStyle} onClick={() => handleClick(index + 1)}>
				<span>
					{index + 1}
				</span>
			</button>
		)
	}

	function renderFooter() {
		return (
			<div className={styles.footer}>
				<Gapped vertical={false} gap={15}>
					<span>
						плохо
					</span>
					<div className={styles.footerLine}/>
					<span>
						отлично
					</span>
				</Gapped>
			</div>
		);
	}
}

Results.propTypes = {
	handleClick: PropTypes.func.isRequired
};

export default Results;
