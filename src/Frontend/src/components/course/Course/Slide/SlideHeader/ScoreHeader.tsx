import React, { useState } from "react";
import { connect, ConnectedProps } from "react-redux";
import classNames from 'classnames';
import { constructPathToStudentSubmissions } from "src/consts/routes";
import { isInstructor } from "src/utils/courseRoles";
import { ShortSlideInfo, SlideType } from "src/models/slide";

import { RootState } from "src/models/reduxState";

import Course from '../../Course.js';
import DownloadedHtmlContent from 'src/components/common/DownloadedHtmlContent.js';
import ShowAfterDelay from "src/components/ShowAfterDelay/ShowAfterDelay";
import { Modal } from "@skbkontur/react-ui";

import texts from "./SlideHeader.texts";
import styles from "../SlideHeader/SlideHeader.less";


const ScoreHeaderInternal = (props: PropsFromRedux & ScoreHeaderProps) => {
	const [isModalShowed, showModal] = useState(false);

	const {
		score,
		maxScore,
		isSkipped,
		waitingForManualChecking,
		prohibitFurtherManualChecking,
		hasReviewedSubmissions,
		courseId,
		slideId,
		showStudentSubmissions,
	} = props;
	if(score === null || maxScore === null) {
		return null;
	}

	const isMaxScore = score === maxScore;
	let message: string | null = null;
	if(!isMaxScore) {
		if(isSkipped) {
			message = texts.skippedHeaderText;
		} else if(waitingForManualChecking) {
			message = texts.pendingReview;
		} else if(prohibitFurtherManualChecking) {
			message = texts.prohibitFurtherReview;
		} else if(hasReviewedSubmissions) {
			message = texts.reviewWaitForCorrection;
		}
	}

	const maxModalWidth = window.innerWidth - 40;
	const modalWidth: undefined | number = maxModalWidth > 880 ? 880 : maxModalWidth; //TODO пока что это мок, в будущем width будет другой
	return (
		<div className={ styles.header }>
			<span className={ classNames(styles.headerText, styles.scoreTextWeight, styles.scoreTextColor) }>
				{ texts.getSlideScore(score, maxScore, !isSkipped) }
			</span>
			{ message && <span className={ styles.headerStatusText }>{ message }</span> }
			{ showStudentSubmissions && <a onClick={ () => showModal(true) } className={ styles.headerLinkText }>
				{ texts.showAcceptedSolutionsText }
			</a> }
			{ isModalShowed &&
			<ShowAfterDelay>
				<Modal width={ modalWidth } onClose={ () => showModal(false) }>
					<Modal.Header>
						<h2>
							{ texts.showAcceptedSolutionsHeaderText }
						</h2>
					</Modal.Header>
					<Modal.Body>
						<DownloadedHtmlContent url={ constructPathToStudentSubmissions(courseId, slideId) }/>
					</Modal.Body>
				</Modal>
			</ShowAfterDelay> }
		</div>
	);
};

interface ScoreHeaderProps {
	courseId: string;
	slideId: string;
}

const mapState = (state: RootState, ownProps: ScoreHeaderProps) => {
	const { userProgress, courses, slides, account, } = state;
	const { submissionsByCourses, } = slides;
	const { courseId, slideId } = ownProps;

	const slideProgress = userProgress.progress[courseId]?.[slideId];
	const courseInfo = courses.fullCoursesInfo[courseId];
	const slideInfo: ShortSlideInfo = Course.getSlideInfoById(slideId, courseInfo).current;
	const submissions = submissionsByCourses[courseId]?.[slideId];
	const hasReviewedSubmissions = submissions
		? Object.values(submissionsByCourses[courseId][slideId]).some(s => s.manualCheckingPassed)
		: false;
	const instructor = isInstructor(
		{ isSystemAdministrator: account.isSystemAdministrator, courseRole: account.roleByCourse[courseId] });

	return {
		courseId,
		slideId,
		score: slideProgress?.score ?? 0,
		isSkipped: slideProgress?.isSkipped ?? false,
		waitingForManualChecking: slideProgress?.waitingForManualChecking ?? false,
		prohibitFurtherManualChecking: slideProgress?.prohibitFurtherManualChecking ?? false,
		maxScore: slideInfo.maxScore,
		hasReviewedSubmissions: hasReviewedSubmissions,
		showStudentSubmissions: slideInfo.type === SlideType.Exercise && instructor,
	};
};
const connector = connect(mapState);
type PropsFromRedux = ConnectedProps<typeof connector>;


type ScoreHeaderPropsFromRedux = Omit<PropsFromRedux, "dispatch">;
const ScoreHeader = connector(ScoreHeaderInternal);
export { ScoreHeader, ScoreHeaderProps, ScoreHeaderPropsFromRedux };
