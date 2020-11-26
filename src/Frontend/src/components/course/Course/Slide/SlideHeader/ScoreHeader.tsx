import React from "react";
import { connect, ConnectedProps } from "react-redux";

import { RootState } from "src/models/reduxState";

import texts from "./SlideHeader.texts";
import Course from '../../Course.js';

import styles from "../SlideHeader/SlideHeader.less";
import { ShortSlideInfo } from "src/models/slide";
import { SubmissionColor } from "../Blocks/Exercise/ExerciseUtils";


interface ScoreHeaderProps {
	courseId: string;
	slideId: string;
}

const mapState = (state: RootState, ownProps: ScoreHeaderProps) => {
	const { userProgress, courses, slides } = state;
	const { submissionsByCourses, } = slides;
	const { courseId, slideId } = ownProps;

	const slideProgress = userProgress.progress[courseId]?.[slideId];
	const courseInfo = courses.fullCoursesInfo[courseId];
	const slideInfo: ShortSlideInfo = Course.getSlideInfoById(slideId, courseInfo).current;
	const submissions = submissionsByCourses[courseId]?.[slideId];
	const hasReviewedSubmissions = submissions
		? Object.values(submissionsByCourses[courseId][slideId]).some(s => s.manualCheckingPassed)
		: false;

	return {
		score: slideProgress?.score ?? 0,
		isSkipped: slideProgress?.isSkipped ?? false,
		waitingForManualChecking: slideProgress?.waitingForManualChecking ?? false,
		prohibitFurtherManualChecking: slideProgress?.prohibitFurtherManualChecking ?? false,
		maxScore: slideInfo.maxScore,
		hasReviewedSubmissions: hasReviewedSubmissions,
	};
};
const connector = connect(mapState);
type PropsFromRedux = ConnectedProps<typeof connector>;


const ScoreHeaderInternal = (props: PropsFromRedux & ScoreHeaderProps) => {
	const { score, maxScore, isSkipped, waitingForManualChecking, prohibitFurtherManualChecking, hasReviewedSubmissions } = props;
	if(score === null || maxScore === null) {
		return null;
	}

	const isMaxScore = score === maxScore;
	let color: SubmissionColor = SubmissionColor.NeedImprovements;
	let message: string | null = null;
	if(!isMaxScore) {
		if(isSkipped) {
			color = SubmissionColor.MaxResult;
			message = texts.skippedHeaderText;
		} else if(waitingForManualChecking) {
			message = texts.pendingReview;
		} else if(prohibitFurtherManualChecking) {
			color = SubmissionColor.MaxResult;
			message = texts.prohibitFurtherReview;
		} else if(hasReviewedSubmissions) {
			message = texts.reviewWaitForCorrection;
		}
	}

	const messageStyle = color === SubmissionColor.MaxResult ? styles.headerMaxResultText : styles.headerNeedImprovementsText;
	return (
		<div className={ styles.header }>
			<span className={ styles.headerText }>
				{ texts.getSlideScore(score, maxScore, !isSkipped) }
			</span>
			{ message && <span className={ messageStyle }>{ message }</span> }
		</div>
	);
};


type ScoreHeaderPropsFromRedux = Omit<PropsFromRedux, "dispatch">;
const ScoreHeader = connector(ScoreHeaderInternal);
export { ScoreHeader, ScoreHeaderProps, ScoreHeaderPropsFromRedux };
