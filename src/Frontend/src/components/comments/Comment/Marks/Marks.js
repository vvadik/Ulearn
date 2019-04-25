import React from "react";
import PropTypes from "prop-types";
import { comment } from "../../commonPropTypes";
import Icon from "@skbkontur/react-icons";

import styles from "./Marks.less";

export default function Marks({comment, canViewStudentsGroup}) {
	return (
		<>
			{!comment.isApproved && <HiddenMark />}
			{comment.isCorrectAnswer && <CorrectAnswerMark />}
			{comment.isPinnedToTop && <PinnedToTopMark />}
			{canViewStudentsGroup && <GroupMark />}
		</>
	)
};

const HiddenMark = () => (
	<div className={`${styles.mark} ${styles.approvedComment}`}>
		<Icon name="EyeClosed" size={15} />
		<span className={`${styles.text} ${styles.visibleOnDesktopAndTablet}`}>
			Скрыт
		</span>
	</div>
);

const CorrectAnswerMark = () => (
	<div className={`${styles.mark} ${styles.correctAnswer}`}>
		<Icon name="Ok" size={15} />
		<span className={`${styles.text} ${styles.visibleOnDesktopAndTablet}`}>
			Правильный&nbsp;ответ
		</span>
	</div>
);

const PinnedToTopMark = () => (
	<div className={`${styles.mark} ${styles.pinnedToTop}`}>
		<Icon name="Pin" size={15} />
		<span className={`${styles.text} ${styles.visibleOnDesktopAndTablet}`}>
			Закреплен
		</span>
	</div>
);

const GroupMark = () => (
	<div className={`${styles.mark} ${styles.group}`}>
		<span className={styles.text}>группа</span>
	</div>
);

Marks.propTypes = {
	comment: comment.isRequired,
	canViewStudentsGroup: PropTypes.func,
};
