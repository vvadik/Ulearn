import React from "react";
import PropTypes from "prop-types";
import Icon from "@skbkontur/react-icons";

import styles from "./Marks.less";
import { NotMobile } from "../../../../utils/responsive";

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
	<div className={`${styles.mark} ${styles.approvedComment}`}>
		<Icon name='EyeClosed' size={15} />
		<NotMobile><span className={styles.text}>Скрытый</span></NotMobile>
	</div>
);

const CorrectAnswerMark = () => (
	<div className={`${styles.mark} ${styles.correctAnswer}`}>
		<Icon name='Ok' size={15} />
		<NotMobile><span className={styles.text}>Правильный&nbsp;ответ</span></NotMobile>
	</div>
);

const PinnedToTopMark = () => (
	<div className={`${styles.mark} ${styles.pinnedToTop}`}>
		<Icon name='Pin' size={15} />
		<NotMobile><span className={styles.text}>Закреплено</span></NotMobile>
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
