import React, {Component} from 'react'
import PropTypes from "prop-types";
import getPluralForm from "../../../../utils/getPluralForm";
import styles from './unitCard.less'
import Button from "@skbkontur/react-ui/Button";

const classNames = require('classnames');

class UnitCard extends Component {
	render() {
		const {unitTitle, haveProgress, total} = this.props;
		const title = unitTitle !== undefined
			? unitTitle
			: "";
		const unitStyle = classNames(styles.unitCard, {
			[styles.successColor]: haveProgress
		});

		return (
			<div className={styles.unitCardContainer}>
				<div className={unitStyle}>
					<div className={styles.unitCardTextContent}>
						<h3 className={styles.unitCardTitle}>
							{title}
						</h3>
						<span className={styles.unitCardBody}>
							{UnitCard.countCards(total)}
						</span>
					</div>
					<Button align={'center'} size={'large'}>
						Начать проверку
					</Button>
				</div>
				{total > 1 &&
				<div
					className={classNames(unitStyle, styles.unitCardNext, {[styles.successNextColor]: haveProgress})}/>}
				{total > 2 &&
				<div
					className={classNames(unitStyle, styles.unitCardLast, {[styles.successLastColor]: haveProgress})}/>}
			</div>
		);
	}

	static countCards(count) {
		if (count === undefined) {
			count = 0;
		}
		return (
			`${count} ${getPluralForm(count, 'карточка', 'карточки', 'карточек')}`
		);
	}
}

UnitCard.propTypes = {
	unitTitle: PropTypes.string.isRequired,
	total: PropTypes.number.isRequired,
	haveProgress: PropTypes.bool.isRequired
};

export default UnitCard;