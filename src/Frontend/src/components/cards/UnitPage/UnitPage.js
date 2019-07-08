import React from 'react'
import PropTypes from "prop-types";
import UnitCard from "./UnitCard/UnitCard";
import Guides from "../Guides/Guides";
import styles from './unitPage.less'
import Gapped from "@skbkontur/react-ui/Gapped";
import UnitProgressBar from "./UnitProgressBar/UnitProgressBar";
import ShortQuestions from "./ShortQuestions/ShortQuestions";

function UnitPage({unitTitle, byScore, total, guides, questionsWithAnswers}) {
	const haveProgress = byScore.unseen !== total;

	return (
		<Gapped gap={8} vertical={true}>
			<h3 className={styles.title}>
				Вопросы для самопроверки
			</h3>
			<UnitCard haveProgress={haveProgress} total={total} unitTitle={unitTitle}/>
			{renderFooter()}
		</Gapped>
	);

	function renderFooter() {
		if (haveProgress) {
			return (
				<div>
					<UnitProgressBar className={styles.progressBar} byScore={byScore} total={total}/>
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
	total: PropTypes.number.isRequired,
	byScore: PropTypes.shape({
		unseen: PropTypes.number,
		1: PropTypes.number,
		2: PropTypes.number,
		3: PropTypes.number,
		4: PropTypes.number,
		5: PropTypes.number
	}).isRequired
};

export default UnitPage;
