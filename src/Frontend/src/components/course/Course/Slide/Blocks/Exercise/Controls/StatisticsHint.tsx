import React from "react";
import classNames from "classnames";

import { Tooltip } from "ui";

import styles from './Controls.less';

import texts from "../Exercise.texts";

interface Props {
	attemptsStatistics: {
		attemptedUsersCount: number,
		usersWithRightAnswerCount: number,
		lastSuccessAttemptDate?: string,
	}
}

function StatisticsHint({ attemptsStatistics }: Props): React.ReactElement {
	const {
		attemptedUsersCount,
		usersWithRightAnswerCount,
		lastSuccessAttemptDate,
	} = attemptsStatistics;
	const statisticsClassName = classNames(styles.exerciseControls, styles.statistics);

	return (
		<span className={ statisticsClassName }>
			<Tooltip pos={ "bottom right" } trigger={ "hover&focus" } render={
				() =>
					<span>
						{ texts.controls.statistics.buildStatistics(attemptedUsersCount,
							usersWithRightAnswerCount, lastSuccessAttemptDate) }
					</span>
			}>
				{ texts.controls.statistics.buildShortText(usersWithRightAnswerCount) }
			</Tooltip>
		</span>
	);
}

export default StatisticsHint;
