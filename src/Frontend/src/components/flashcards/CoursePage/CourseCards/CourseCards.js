import React from 'react';
import PropTypes from "prop-types";
import styles from './courseCards.less';
import classNames from 'classnames';
import getCardsPluralForm from "../../../../utils/getCardsPluralForm";
import LockClosed from '@skbkontur/react-icons/LockClosed';
import Button from "@skbkontur/react-ui/Button";
import {Link} from 'react-router-dom';

function CourseCards({flashcardsInfo, courseId, unitsInfo}) {
	return (
		<div className={styles.cardsContainer}>
			{flashcardsInfo.map(convertToUnitCard)}
			<div className={styles.emptyUnitCard}>
				Новые вопросы для самопроверки открываются по мере прохождения курса
			</div>
		</div>
	);

	function convertToUnitCard({unitTitle, unlocked, cardsCount, unitId}) {
		const unitCardStyle = classNames(styles.unitCard, {[styles.unitCardLocked]: !unlocked});
		const slideUrl = unitsInfo
			.find(unit => unit.id === unitId)
			.slides[0]
			.id;

		const url = unlocked
			? `/Course/${courseId}/flashcards/${unitId}`
			: `/Course/${courseId}/${slideUrl}/`;

		return (
			<Link key={unitId} className={unitCardStyle}
				  to={url}>
				<div>
					<h3 className={styles.unitCardTitle}>
						{unitTitle}
					</h3>
					<p className={styles.unitCardBody}>
						{getCardsPluralForm(cardsCount)}
					</p>
				</div>
				<div className={styles.unitCardButton}>
					{!unlocked &&
					<Button size={'medium'}>
						Открыть модуль
					</Button>}
				</div>
				{!unlocked && <LockClosed className={styles.unitCardLockerIcon} size={22}/>}
			</Link>
		);
	}
}

CourseCards.propTypes = {
	flashcardsInfo: PropTypes.arrayOf(PropTypes.shape({
		unitTitle: PropTypes.string,
		unlocked: PropTypes.bool,
		cardsCount: PropTypes.number,
		unitId: PropTypes.string
	})),
	courseId: PropTypes.string.isRequired,
	unitsInfo: PropTypes.arrayOf(PropTypes.shape({
		id: PropTypes.string,
		slides: PropTypes.array,
		title: PropTypes.string,
	})),
};

export default CourseCards;
