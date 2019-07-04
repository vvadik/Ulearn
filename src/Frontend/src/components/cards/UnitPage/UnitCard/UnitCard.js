import React, {Component} from 'react';
import PropTypes from "prop-types";
import getPluralForm from "../../../../utils/getPluralForm";
import styles from './unitCard.less';
import Button from "@skbkontur/react-ui/Button";
import classNames from "classnames";

class UnitCard extends Component {
	render() {
		const {unitTitle = "", haveProgress = false, total = 0} = this.props;
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
							{UnitCard.countCards(total)}
						</span>
					</div>
					<Button size={'large'}>
						Начать проверку
					</Button>
				</div>
				{total > 1 &&
				<div
					className={stylesForCardNext}/>}
				{total > 2 &&
				<div
					className={stylesForCardLast}/>}
			</div>
		);
	}

	static countCards(count = 0) {
		return (
			`${count} ${getPluralForm(count, 'карточка', 'карточки', 'карточек')}`
		);
	}
}

UnitCard.propTypes = {
	unitTitle: PropTypes.string.isRequired,
	total: PropTypes.number.isRequired,
	haveProgress: PropTypes.bool
};

export default UnitCard;
