import React, {Component} from 'react';
import PropTypes from "prop-types";
import styles from './unitCard.less';
import Button from "@skbkontur/react-ui/Button";
import classNames from "classnames";
import getCardsPluralForm from "../../../../utils/getCardsPluralForm";

class UnitCard extends Component {
	render() {
		const {unitTitle = "", haveProgress = false, totalFlashcardsCount = 0} = this.props;
		const unitStyle = classNames(styles.unitCard, {
			[styles.successColor]: haveProgress
		});
		const stylesForCardNext = classNames(unitStyle, styles.unitCardNext);
		const stylesForCardLast = classNames(unitStyle, styles.unitCardLast);

		return (
			<div className={styles.unitCardContainer}>
				<div className={unitStyle}>
					<div className={styles.unitCardTextContent}>
						<h3 className={styles.unitCardTitle}>
							{unitTitle}
						</h3>
						<span className={styles.unitCardBody}>
							{getCardsPluralForm(totalFlashcardsCount)}
						</span>
					</div>
					<Button size={'large'} onClick={() => this.handleStartButton()}>
						Начать проверку
					</Button>
				</div>
				{totalFlashcardsCount > 1 &&
				<div
					className={stylesForCardNext}/>}
				{totalFlashcardsCount > 2 &&
				<div
					className={stylesForCardLast}/>}
			</div>
		);
	}

	handleStartButton() {
		console.log(`Starting`);
	}
}

UnitCard.propTypes = {
	unitTitle: PropTypes.string.isRequired,
	totalFlashcardsCount: PropTypes.number.isRequired,
	haveProgress: PropTypes.bool
};

export default UnitCard;
