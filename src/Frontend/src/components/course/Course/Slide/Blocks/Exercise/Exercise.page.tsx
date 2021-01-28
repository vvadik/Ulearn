import { connect } from "react-redux";
import { Dispatch } from "redux";

import Exercise from './Exercise';

import { AutomaticExerciseCheckingResult as CheckingResult } from "src/models/exercise";
import { RootState } from "src/models/reduxState";

import { userProgressUpdateAction } from "src/actions/userProgress";
import { sendCode, addReviewComment, deleteReviewComment, } from "src/actions/course.js";
import { Language } from "src/consts/languages";
import { MatchParams } from "src/consts/router";

const mapStateToProps = (state: RootState, { courseId, slideId, }: MatchParams) => {
	const { slides, account, userProgress } = state;
	const { submissionsByCourses, submissionError, lastCheckingResponse, } = slides;
	const slideProgress = userProgress?.progress[courseId]?.[slideId] || {};

	const submissions = Object.values(submissionsByCourses[courseId][slideId])
		.filter((s, i, arr) =>
			(i === arr.length - 1)
			|| (!s.automaticChecking || s.automaticChecking.result === CheckingResult.RightAnswer));

	//newer is first
	submissions.sort((s1, s2) => (new Date(s2.timestamp).getTime() - new Date(s1.timestamp).getTime()));

	return {
		isAuthenticated: account.isAuthenticated,
		submissions,
		submissionError,
		lastCheckingResponse: !(lastCheckingResponse && lastCheckingResponse.courseId === courseId && lastCheckingResponse.slideId === slideId) ? null : lastCheckingResponse,
		author: account,
		slideProgress
	};
};

const mapDispatchToProps = (dispatch: Dispatch) => ({
	sendCode: (courseId: string, slideId: string, code: string, language: Language
	) => dispatch(sendCode(courseId, slideId, code, language)),

	addReviewComment: (courseId: string, slideId: string, submissionId: number, reviewId: number,
		comment: string
	) => dispatch(addReviewComment(courseId, slideId, submissionId, reviewId, comment)),

	deleteReviewComment: (courseId: string, slideId: string, submissionId: number, reviewId: number,
		commentId: number
	) => dispatch(deleteReviewComment(courseId, slideId, submissionId, reviewId, commentId)),

	visitAcceptedSolutions: (courseId: string, slideId: string,
	) => dispatch(userProgressUpdateAction(courseId, slideId, { isSkipped: true })),
});

export default connect(mapStateToProps, mapDispatchToProps)(Exercise);
