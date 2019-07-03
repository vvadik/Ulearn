import React, {Component} from 'react'
import PropTypes from "prop-types";
import getPluralForm from "../../../../utils/getPluralForm";
import styles from './unitCard.less'
import Button from "@skbkontur/react-ui/Button";

const classNames = require('classnames');

class UnitCard extends Component {
	static countCards(count) {
		return (
			`${count} ${getPluralForm(count, 'карточка', 'карточки', 'карточек')}`
		);
	}

	render() {
		const {cards, title, isCompleted} = this.props;
		const unitStyle = classNames(styles.unitCard, {
			[styles.successColor]: isCompleted
		});
		const cardsCount = cards.length;
		return (
			<div className={styles.unitCardContainer}>
				<div className={unitStyle}>
					<div className={styles.unitCardTextContent}>
						<h3 className={styles.unitCardTitle}>
							{title}
						</h3>
						<span className={styles.unitCardBody}>
							{UnitCard.countCards(cardsCount)}
						</span>
					</div>
					<Button align={'center'} size={'large'}>
						Начать проверку
					</Button>
				</div>
				{cardsCount > 1 &&
				<div className={classNames(unitStyle, styles.unitCardNext, {[styles.successNextColor]: isCompleted})}/>}
				{cardsCount > 2 &&
				<div className={classNames(unitStyle, styles.unitCardLast, {[styles.successLastColor]: isCompleted})}/>}
			</div>
		);
	}
}

UnitCard.propTypes = {
	title: PropTypes.string,
	cards: PropTypes.array,
	isCompleted: PropTypes.bool
};

export default UnitCard;