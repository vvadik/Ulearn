import React from "react";
import { connect, ConnectedProps } from "react-redux";

import { RootState } from "src/models/reduxState";

import texts from "./SlideHeader.texts";
import Course from '../../Course.js';

import styles from "../Blocks/BlocksWrapper.less";
import { ShortSlideInfo } from "src/models/slide";


interface ScoreHeaderProps {
	courseId: string;
	slideId: string;
}

const mapState = (state: RootState, ownProps: ScoreHeaderProps) => {
	const { userProgress, courses } = state;
	const { courseId, slideId } = ownProps;
	const slideProgress = userProgress.progress[courseId]?.[slideId];
	const courseInfo = courses.fullCoursesInfo[courseId];
	const slideInfo: ShortSlideInfo = Course.getSlideInfoById(slideId, courseInfo).current;
	return {
		score: slideProgress?.score ?? 0,
		isSkipped: slideProgress?.isSkipped ?? false,
		waitingForManualChecking: slideProgress?.waitingForManualChecking ?? false,
		maxScore: slideInfo.maxScore,
	};
};
const connector = connect(mapState);
type PropsFromRedux = ConnectedProps<typeof connector>;


const ScoreHeaderInternal = (props: PropsFromRedux & ScoreHeaderProps) => {
	const { score, maxScore, isSkipped, waitingForManualChecking } = props;
	if(score === null || maxScore === null) {
		return null;
	}
	return (
		<div className={ styles.header }>
			<span className={ styles.headerText }>
				{ texts.getSlideScore(score, maxScore, !isSkipped) }
			</span>
			{ isSkipped &&
			<span className={ styles.headerSkippedText }>{ texts.skippedHeaderText }</span>
			}
		</div>
	);
};
const ScoreHeader = connector(ScoreHeaderInternal)

export { ScoreHeader, ScoreHeaderProps }
