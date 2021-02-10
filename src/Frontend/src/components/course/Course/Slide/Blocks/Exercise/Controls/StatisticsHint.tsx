import React from "react";
import classNames from "classnames";

import { Tooltip, TooltipTrigger } from "ui";
import ShowControlsTextContext from "./ShowControlsTextContext";

import IControlWithText from "./IControlWithText";
import { AttemptsStatistics } from "src/models/exercise";

import styles from './Controls.less';

import texts from "../Exercise.texts";
import { Statistic } from "@skbkontur/react-icons";


export interface Props extends IControlWithText {
	attemptsStatistics: AttemptsStatistics,
	tooltipTrigger?: TooltipTrigger,
}

function StatisticsHint({
	attemptsStatistics,
	tooltipTrigger = "hover&focus",
	showControlsText
}: Props): React.ReactElement {
	const {
		attemptedUsersCount,
		usersWithRightAnswerCount,
		lastSuccessAttemptDate,
	} = attemptsStatistics;
	const statisticsClassName = classNames(styles.exerciseControls, styles.statistics);

	return (
		<span className={ statisticsClassName }>
			<Tooltip
				pos={ "bottom right" }
				closeButton={ false }
				trigger={ tooltipTrigger }
				render={ renderTooltipContent }>
				<ShowControlsTextContext.Consumer>
					{
						(showControlsTextContext) => (showControlsTextContext || showControlsText)
							? texts.controls.statistics.buildShortText(usersWithRightAnswerCount)
							: <span className={ styles.exerciseControlsIcon }>
							<Statistic/>
						</span>
					}
				</ShowControlsTextContext.Consumer>
			</Tooltip>
		</span>
	);

	function renderTooltipContent() {
		return (
			<span>
				{ texts.controls.statistics.buildStatistics(attemptedUsersCount,
					usersWithRightAnswerCount, lastSuccessAttemptDate) }
			</span>);
	}
}

export default StatisticsHint;
