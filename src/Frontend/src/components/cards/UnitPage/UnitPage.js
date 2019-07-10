import React from 'react'
import PropTypes from "prop-types";
import UnitCard from "./UnitCard/UnitCard";
import Guides from "../Guides/Guides";
import styles from './unitPage.less'
import Gapped from "@skbkontur/react-ui/Gapped";
import ProgressBar from "../ProgressBar/ProgressBar";
import ShortQuestions from "./ShortQuestions/ShortQuestions";

function UnitPage({unitTitle,statistics,totalFlashcardsCount, guides, questionsWithAnswers}) {
	const haveProgress = statistics.notRated !== totalFlashcardsCount;

	return (
		<Gapped gap={8} vertical={true}>
			<h3 className={styles.title}>
				Вопросы для самопроверки
			</h3>
			<UnitCard haveProgress={haveProgress} totalFlashcardsCount={totalFlashcardsCount} unitTitle={unitTitle}/>
			{renderFooter()}
		</Gapped>
	);

	function renderFooter() {
		if (haveProgress) {
			return (
				<div>
					<p className={styles.progressBarTitle}>
						Результаты последнего прохождения
					</p>
					<ProgressBar statistics={statistics} totalFlashcardsCount={totalFlashcardsCount}/>
					<ShortQuestions className={styles.shortQuestions} questionsWithAnswers={questionsWithAnswers}/>
				</div>
			);
		} else {
			return <Guides guides={guides}/>;
		}
	}
}

UnitPage.propTypes = {
	unitTitle: PropTypes.string.isRequired,
	guides: PropTypes.arrayOf(PropTypes.string).isRequired,
	questionsWithAnswers: PropTypes.arrayOf(PropTypes.shape({
		question: PropTypes.string,
		answer: PropTypes.string
	})).isRequired,
	totalFlashcardsCount: PropTypes.number.isRequired,
	statistics: PropTypes.shape({
		notRated: PropTypes.number,
		rate1: PropTypes.number,
		rate2: PropTypes.number,
		rate3: PropTypes.number,
		rate4: PropTypes.number,
		rate5: PropTypes.number
	}).isRequired
};

export default UnitPage;
