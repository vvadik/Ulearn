import React from 'react'
import PropTypes from "prop-types";
import CourseCards from "./CourseCards/CourseCards";
import Guides from "../Guides/Guides";
import styles from './coursePage.less'
import Gapped from "@skbkontur/react-ui/Gapped";
import Button from "@skbkontur/react-ui/Button";

function CoursePage({cardsByUnits, guides}) {
	return (
		<Gapped gap={15} vertical={true}>
			<div className={styles.textContainer}>
				<h2 className={styles.title}>
					Флеш-карты для самопроверки
				</h2>
				<span className={styles.description}>
					Помогут лучше запомнить материал курса и подготовиться к экзаменам
				</span>
				<div className={styles.launchAllButtonContainer}>
					<Button use="primary" size='large'>
						Проверить себя
					</Button>
				</div>
			</div>
			<CourseCards cardsByUnits={cardsByUnits}/>
			<Guides guides={guides}/>
		</Gapped>
	);
}

CoursePage.propTypes = {
	cardsByUnits: PropTypes.arrayOf(PropTypes.shape({
		unitTitle: PropTypes.string,
		unlocked: PropTypes.bool,
		cardsCount: PropTypes.number,
		unitId: PropTypes.string
	})),
	guides: PropTypes.arrayOf(PropTypes.string).isRequired,
};

export default CoursePage;
