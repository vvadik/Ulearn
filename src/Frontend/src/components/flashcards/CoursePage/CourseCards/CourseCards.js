import React from 'react';
import PropTypes from "prop-types";

import LockClosed from '@skbkontur/react-icons/LockClosed';
import Button from "@skbkontur/react-ui/Button";
import { Link } from 'react-router-dom';

import styles from './courseCards.less';
import classNames from 'classnames';
import getCardsPluralForm from "../../../../utils/getCardsPluralForm";


const emptyUnitCardStyle = classNames(styles.unitCard, styles.emptyUnitCard);

function CourseCards({ infoByUnits, courseId }) {
	return (
		<div className={ styles.cardsContainer }>
			{ infoByUnits.map(convertToUnitCard) }
			<div className={ emptyUnitCardStyle }>
				Новые вопросы для самопроверки открываются по мере прохождения курса
			</div>
		</div>
	);

	function convertToUnitCard({ unitTitle, unlocked, cardsCount, unitId, flashcardsSlideSlug }) {
		const unitCardStyle = classNames(styles.unitCard, { [styles.unitCardLocked]: !unlocked });

		const url = `/course/${ courseId }/${ flashcardsSlideSlug }/`;

		return (
			<Link key={ unitId } className={ unitCardStyle }
				  to={ url }>
				<div>
					<h3 className={ styles.unitCardTitle }>
						{ unitTitle }
					</h3>
					<p className={ styles.unitCardBody }>
						{ getCardsPluralForm(cardsCount) }
					</p>
				</div>
				<div className={ styles.unitCardButton }>
					{ !unlocked &&
					<Button size={ 'medium' }>
						Открыть модуль
					</Button> }
				</div>
				{ !unlocked && <LockClosed className={ styles.unitCardLockerIcon } size={ 22 }/> }
			</Link>
		);
	}
}

CourseCards.propTypes = {
	courseId: PropTypes.string.isRequired,
	infoByUnits: PropTypes.arrayOf(PropTypes.shape({
		unitTitle: PropTypes.string,
		unlocked: PropTypes.bool,
		cardsCount: PropTypes.number,
		unitId: PropTypes.string,
		flashcardsSlideSlug: PropTypes.string,
	})),
};

export default CourseCards;
