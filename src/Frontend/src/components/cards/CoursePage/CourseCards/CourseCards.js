import React from 'react';
import PropTypes from "prop-types";
import styles from './courseCards.less';
import classNames from 'classnames';
import getCardsPluralForm from "../../../../utils/getCardsPluralForm";
import LockClosed from '@skbkontur/react-icons/LockClosed';
import Button from "@skbkontur/react-ui/Button";

function CourseCards({flashcardsInfos}) {
	return (
		<div className={styles.cardsContainer}>
			{flashcardsInfos.map(convertToUnitCard)}
			<div className={styles.emptyUnitCard}>
				<span className={styles.emptyUnitCardText}>
					Новые вопросы для самопроверки открываются по мере прохождения курса
				</span>
			</div>
		</div>
	);

	function convertToUnitCard({unitTitle, unlocked, cardsCount, unitId}) {
		const unitCardStyle = classNames(styles.unitCard, {[styles.unitCardLocked]: !unlocked});

		return (
			<div key={unitId} className={unitCardStyle} onClick={() => handleUnitCardClick(unitTitle)}>
				<div>
					<h3 className={styles.unitCardTitle}>
						{unitTitle}
					</h3>
					<div className={styles.unitCardBody}>
						{getCardsPluralForm(cardsCount)}
					</div>
				</div>
				<div className={styles.unitCardButton}>
					{!unlocked &&
					<Button size={'medium'}>
						Открыть модуль
					</Button>}
				</div>
				{!unlocked && <LockClosed className={styles.unitCardLockerIcon} size={22}/>}
			</div>
		);
	}

	function handleUnitCardClick(unitTitle) {
		console.log(`clicked on ${unitTitle}`);
	}
}

CourseCards.propTypes = {
	flashcardsInfos: PropTypes.arrayOf(PropTypes.shape({
		unitTitle: PropTypes.string,
		unlocked: PropTypes.bool,
		cardsCount: PropTypes.number,
		unitId: PropTypes.string
	}))
};

export default CourseCards;
