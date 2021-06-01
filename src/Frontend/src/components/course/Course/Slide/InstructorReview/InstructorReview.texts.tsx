import { InstructorReviewTabs } from "./InstructorReviewTabs";
import { SubmissionInfo } from "src/models/exercise";
import { convertDefaultTimezoneToLocal } from "src/utils/momentUtils";
import React from "react";
import getPluralForm from "src/utils/getPluralForm";

const texts = {
	getTabName: (tab: InstructorReviewTabs): string => {
		switch (tab) {
			case InstructorReviewTabs.AuthorSolution:
				return 'Авторское решение';
			case InstructorReviewTabs.Formulation:
				return 'Формулировка';
			case InstructorReviewTabs.Review:
				return 'Решение студента';
		}
	},
	getStudentInfo: (visibleName: string, groupName?: string): string => {
		return groupName ? `${ visibleName } (${ groupName })` : visibleName;
	},
	getReviewInfo: (submissions: SubmissionInfo[], prevReviewScore?: number, currentScore?: number): string => {
		if(currentScore !== undefined) {
			return `${ currentScore }% за ревью`;
		}
		if(prevReviewScore === undefined) {
			return 'первое ревью';
		}
		return `${ prevReviewScore }% за предыдущее ревью`;
	},
	getSubmissionCaption: (
		submission: SubmissionInfo,
		selectedSubmissionIsLastSuccess: boolean,
		waitingForManualChecking: boolean
	): string => {
		const { timestamp, manualCheckingPassed } = submission;
		const timestampCaption = texts.getSubmissionDate(timestamp);
		if(manualCheckingPassed) {
			return timestampCaption + ", прошло код-ревью";
		} else if(waitingForManualChecking && selectedSubmissionIsLastSuccess) {
			return timestampCaption + ", ожидает код-ревью";
		}
		return timestampCaption;
	},
	getSubmissionDate: (timestamp: string): string => {
		return convertDefaultTimezoneToLocal(timestamp).format('DD MMMM YYYY в HH:mm');
	},
	leaveCommentGuideText: 'Выделите участок кода, чтобы оставить комментарий',
	getDiffText: (
		addedCount: number,
		addedColor: string,
		removedCount: number,
		removedColor: string,
	): React.ReactElement => <>
		Diff с предыдущим ревью:
		<span className={ addedColor }> { addedCount } { getPluralForm(addedCount, 'строку', 'строки', 'строк') } добавили</span>,
		<span className={ removedColor }> { removedCount } – удалили</span>
	</>,
};

export default texts;
