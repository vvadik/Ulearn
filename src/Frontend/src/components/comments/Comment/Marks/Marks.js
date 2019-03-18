import React from "react";
import PropTypes from "prop-types";
import Icon from "@skbkontur/react-icons";

import styles from "./Marks.less";

export default function Marks({isApproved, isCorrectAnswer, isPinnedToTop, canViewStudentsGroup}) {
	return (
		<>
			{!isApproved && <HiddenMark />}
			{isCorrectAnswer && <CorrectAnswerMark />}
			{isPinnedToTop && <PinnedToTopMark />}
			{canViewStudentsGroup && <GroupMark />}
		</>
	)
};

const HiddenMark = () => (
	<div className={`${styles.mark} ${styles.approvedComment}`}>Скрытый</div>
);

const CorrectAnswerMark = () => (
	<div className={`${styles.mark} ${styles.correctAnswer}`}>
		<Icon name='Star' size={15} />
		<span className={styles.text}>Правильный ответ</span>
	</div>
);

const PinnedToTopMark = () => (
	<div className={`${styles.mark} ${styles.pinnedToTop}`}>
		<Icon name='Pin' size={15} />
		<span className={styles.text}>Закреплено</span>
	</div>
);

const GroupMark = () => (
	<div className={`${styles.mark} ${styles.group}`}>
		<span className={styles.text}>группа</span>
	</div>
);

Marks.propTypes = {
	canViewStudentsGroup: PropTypes.bool,
	isApproved: PropTypes.bool,
	isCorrectAnswer: PropTypes.bool,
	isPinnedToTop: PropTypes.bool,
};
