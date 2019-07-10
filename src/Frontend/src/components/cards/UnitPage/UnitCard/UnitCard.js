import React, {Component} from 'react';
import PropTypes from "prop-types";
import styles from './unitCard.less';
import Button from "@skbkontur/react-ui/Button";
import classNames from "classnames";
import getCardsPluralForm from "../../../../utils/getCardsPluralForm";

class UnitCard extends Component {
	render() {
		const {unitTitle = "", haveProgress = false, totalFlashcardsCount = 0} = this.props;
		const unitCardStyle = classNames(styles.unitCard, {
			[styles.successColor]: haveProgress
		});
		const stylesForCardNext = classNames(unitCardStyle, styles.unitCardNext);
		const stylesForCardLast = classNames(unitCardStyle, styles.unitCardLast);

		return (
			<div className={styles.unitCardContainer}>
				<div className={unitCardStyle}>
					<h3 className={styles.unitCardTitle}>
						{unitTitle}
					</h3>
					<p className={styles.unitCardBody}>
						{getCardsPluralForm(totalFlashcardsCount)}
					</p>
					<div className={styles.startButtonContainer}>
						<Button size={'large'} onClick={() => this.handleStartButton()}>
							Начать проверку
						</Button>
					</div>
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
