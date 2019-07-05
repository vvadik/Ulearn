import React, {Component} from 'react';
import PropTypes from "prop-types";
import styles from './courseCards.less';
import classNames from 'classnames';
import getCardsPluralForm from "../../../../utils/getCardsPluralForm";
import LockClosed from '@skbkontur/react-icons/LockClosed';
import Button from "@skbkontur/react-ui/Button";

class CourseCards extends Component {
	render() {
		const {cardsByUnits} = this.props;

		return (
			<div className={styles.cardsContainer}>
				{cardsByUnits.map(CourseCards.convertToUnitCard)}
				<div className={styles.emptyUnitCard}>
					<span className={styles.emptyUnitCardText}>
						Новые вопросы для самопроверки открываются по мере прохождения курса
					</span>
				</div>
			</div>
		)
	}

	static convertToUnitCard({unitTitle, unlocked, cardsCount, unitId}) {
		const unitCardStyle = classNames(styles.unitCard, {[styles.unitCardLocked]: !unlocked});

		return (
			<div key={unitId} className={unitCardStyle}>
				<div>
					<h3 className={styles.unitCardTitle}>
						{unitTitle}
					</h3>
					<div className={styles.unitCardBody}>
						{getCardsPluralForm(cardsCount)}
					</div>
				</div>
				<div className={styles.unitCardButton}>
					<Button size={'medium'}>
						{unlocked ? 'Начать' : 'Открыть модуль'}
					</Button>
				</div>
				{!unlocked && <LockClosed className={styles.unitCardLockerIcon} size={22}/>}
			</div>
		);
	}
}

CourseCards.propTypes = {
	cardsByUnits: PropTypes.arrayOf(PropTypes.shape({
		unitTitle: PropTypes.string,
		unlocked: PropTypes.bool,
		cardsCount: PropTypes.number,
		unitId: PropTypes.string
	}))
};

export default CourseCards;
