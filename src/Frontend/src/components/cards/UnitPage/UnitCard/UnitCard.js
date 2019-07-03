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
		let unitStyle = classNames(styles.unitCard, {
			[styles.successColor]: this.props.isCompleted
		});
		return (
			<div className={unitStyle}>
				<div className={styles.unitCardTextContent}>
					<h3 className={styles.unitCardTitle}>
						{this.props.title}
					</h3>
					<span className={styles.unitCardBody}>
						{UnitCard.countCards(this.props.cards.length)}
					</span>
				</div>
				<Button align={'center'} size={'large'}>
					Начать проверку
				</Button>
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