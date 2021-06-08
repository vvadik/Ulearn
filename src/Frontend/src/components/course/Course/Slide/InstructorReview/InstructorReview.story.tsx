import React from "react";
import InstructorReview, { Props } from "./InstructorReview";
import { Language } from "src/consts/languages";
import type { Story } from "@storybook/react";
import {
	AutomaticExerciseCheckingProcessStatus,
	AutomaticExerciseCheckingResult,
	ReviewInfo,
} from "src/models/exercise";
import { ShortUserInfo } from "src/models/users";
import { returnPromiseAfterDelay } from "src/utils/storyMock";
import { AntiplagiarismInfo } from "./AntiplagiarismHeader/AntiplagiarismHeader";


const user: ShortUserInfo = {
	visibleName: 'Пользователь ДлиннаяФамилияКоторояМожетСломатьВерстку',
	lastName: 'ДлиннаяФамилияКоторояМожетСломатьВерстку',
	firstName: 'Пользователь',
	id: "0",
	avatarUrl: "",
	email: "user@email.com",
	login: 'Administrator of everything on ulearn.me',
};

export const Default: Story<Props> = (args: Props) => <InstructorReview { ...args }/>;

function renderMd(text: string) {
	const regexBold = /\*\*(\S(.*?\S)?)\*\*/gm;
	const regexItalic = /__(\S(.*?\S)?)__/gm;
	const regexCode = /```(\S(.*?\S)?)```/gm;
	text = text.replace(regexBold, '<b>$1</b>');
	text = text.replace(regexItalic, '<i>$1</i>');
	text = text.replace(regexCode, '<code>$1</code>');
	return (text.replace('**', '<b>'));
}

const extra = {
	suspicionLevel: 0,
	reviewId: 0,
};

const addIdToReview = (review: any): ReviewInfo => ({
	...review, id: extra.reviewId++,
});

const args: Props = {
	studentSubmissions: [
		{
			code: `\t\t\t\tif (course == null || tempCourse.LastUpdateTime < tempCourse.LoadingTime)
\t\t\t\t{
\t\t\t\t\tTryReloadCourse(courseId);
\t\t\t\t\tvar tempCoursesRepo = new TempCoursesRepo();
\t\t\t\t\ttempCoursesRepo.UpdateTempCourseLastUpdateTime(courseId);
\t\t\t\t\tcourseVersionFetchTime[courseId] = DateTime.Now;
\t\t\t\t} else if (tempCourse.LastUpdateTime > tempCourse.LoadingTime)
\t\t\t\t\tcourseVersionFetchTime[courseId] = DateTime.Now;
\t\t\t}
\t\t\tcatch (Exception ex)
\t\t\t{
\t\t\t\t\t\tTryReloadCourse(courseId);
\t\t\t\t\t\tvar tempCoursesRepo = new TempCoursesRepo();
\t\t\t\t\t\ttempCoursesRepo.UpdateTempCourseLastUpdateTime(courseId);
\t\t\t\t\t\tcourseVersionFetchTime[courseId] = DateTime.Now;
\t\t\t\t\t} else if (tempCourse.LastUpdateTime > tempCourse.LoadingTime)
\t\t\t\t\t\tcourseVersionFetchTime[courseId] = DateTime.Now;
\t\t\t\t}
\t\t\t}
\t\t\tcatch (Exception ex)
\t\t\t\treturn lastFetchTime > DateTime.Now.Subtract(tempCourseUpdateEvery);
\t\t\treturn false;
\t\t}
\t}
}
`,
			language: Language.cSharp,
			timestamp: '2020-04-06',
			manualCheckingPassed: false,
			manualCheckingReviews: [],
			automaticChecking: null,
		},
		{
			code: `\t\t\t\tif (course == null || tempCourse.LastUpdateTime < tempCourse.LoadingTime)
\t\t\t\t{
\t\t\t\t\tTryReloadCourse(courseId);
\t\t\t\t\tvar tempCoursesRepo = new TempCoursesRepo();
\t\t\t\t\ttempCoursesRepo.UpdateTempCourseLastUpdateTime(courseId);
\t\t\t\t\ttempCourseUpdateTime[courseId] = DateTime.Now;
\t\t\t\t} else if (tempCourse.LastUpdateTime > tempCourse.LoadingTime)
\t\t\t\t\ttempCourseUpdateTime[courseId] = DateTime.Now;
\t\t\t}
\t\t\tcatch (Exception ex)
\t\t\t{
\t\t\t\t\t\tTryReloadCourse(courseId);
\t\t\t\t\t\tvar tempCoursesRepo = new TempCoursesRepo();
\t\t\t\t\t\ttempCoursesRepo.UpdateTempCourseLastUpdateTime(courseId);
\t\t\t\t\t\ttempCourseUpdateTime[courseId] = DateTime.Now;
\t\t\t\t\t} else if (tempCourse.LastUpdateTime > tempCourse.LoadingTime)
\t\t\t\t\t\ttempCourseUpdateTime[courseId] = DateTime.Now;
\t\t\t\t}
\t\t\t}
\t\t\tcatch (Exception ex)
\t\t\t\treturn lastFetchTime > DateTime.Now.Subtract(tempCourseUpdateEvery);
\t\t\treturn false;
\t\t}

\t\tpublic bool IsTempCourse(string courseId)
\t\t{
\t\t\treturn GetTempCoursesWithCache().Any(c => string.Equals(c.CourseId, courseId, StringComparison.OrdinalIgnoreCase));
\t\t}
\t}
}`,
			language: Language.cSharp,
			timestamp: '2020-04-06',
			manualCheckingPassed: true,
			manualCheckingReviews: [],
			automaticChecking: null,
		},
		{
			code: 'void Main()\n' +
				'{\n' +
				'\tvar i = 0;\n' +
				'\tvar i = 0;\n' +
				'\tvar i = 0;\n' +
				'\tvar i = 0;\n' +
				'\tvar i = 0;\n' +
				'\tvar i = 0;\n' +
				'\tvar i = 0;\n' +
				'\tvar i = 0;\n' +
				'\tvar i = 0;\n' +
				'\tEnd\n' +
				'}',
			language: Language.cSharp,
			timestamp: '2020-04-06',
			manualCheckingPassed: false,
			manualCheckingReviews: [],
			automaticChecking: null,
		},
		{
			code: 'void Main()\n' +
				'{\n' +
				'\tint i = 0;\n' +
				'\tvar i = 0;\n' +
				'\tint i = 0;\n' +
				'\tint i = 0;\n' +
				'\tint i = 0;\n' +
				'\tvar i = 0;\n' +
				'\tint i = 0;\n' +
				'\tvar i = 0;\n' +
				'\tint i = 0;\n' +
				'\tint i = 0;\n' +
				'\tint i = 0;\n' +
				'\tvar i = 0;\n' +
				'}',
			language: Language.cSharp,
			timestamp: '2020-04-06',
			manualCheckingPassed: true,
			manualCheckingReviews: [
				{
					author: user,
					startLine: 2,
					startPosition: 0,
					finishLine: 2,
					finishPosition: 100,
					comment: "var",
					renderedComment: "var",
					addingTime: "2020-08-04 23:04",
					comments: [],
				},
				{
					author: user,
					startLine: 2,
					startPosition: 0,
					finishLine: 2,
					finishPosition: 100,
					comment: "var",
					renderedComment: "var",
					addingTime: "2020-08-04 23:04",
					comments: [],
				},
				{
					author: user,
					startLine: 5,
					startPosition: 0,
					finishLine: 5,
					finishPosition: 100,
					comment: "var",
					renderedComment: "var",
					addingTime: "2020-08-04 23:04",
					comments: [],
				},
				{
					author: user,
					startLine: 6,
					startPosition: 0,
					finishLine: 6,
					finishPosition: 100,
					comment: "var",
					renderedComment: "var",
					addingTime: "2020-08-04 23:04",
					comments: [],
				},
				{
					author: user,
					startLine: 7,
					startPosition: 0,
					finishLine: 7,
					finishPosition: 100,
					comment: "var",
					renderedComment: "var",
					addingTime: "2020-08-04 23:04",
					comments: [],
				},
				{
					author: user,
					startLine: 0,
					startPosition: 0,
					finishLine: 0,
					finishPosition: 100,
					comment: "var ВЕЗДЕ",
					renderedComment: "var ВЕЗДЕ",
					addingTime: "2020-08-04 23:04",
					comments: [],
				},
			],
			automaticChecking: null,
		},
		{
			code: 'void Main()\n' +
				'{\n' +
				'\tConsole.WriteLine("Coding is there tatat");\n' +
				'\tConsole.WriteLine("Coding is there tatat");\n' +
				'\tConsole.WriteLine("Coding is there tatat");\n' +
				'\tConsole.WriteLine("Coding is there tatat");\n' +
				'\tConsole.WriteLine("Coding is there tatat");\n' +
				'\tConsole.WriteLine("Coding is there tatat");\n' +
				'\tConsole.WriteLine("Coding is there tatat");\n' +
				'\tConsole.WriteLine("Coding is there tatat");\n' +
				'\tConsole.WriteLine("Coding is there tatat");\n' +
				'\tConsole.WriteLine("Coding is there tatat");\n' +
				'\tEnd\n' +
				'}',
			language: Language.cSharp,
			timestamp: '2020-04-06',
			manualCheckingPassed: false,
			manualCheckingReviews: [],
			automaticChecking: null,
		},
		{
			code: 'void Main()\n' +
				'{\n' +
				'\tConsole.WriteLine("Coding is here right now tarara");\n' +
				'}',
			language: Language.java,
			timestamp: '2020-04-05',
			manualCheckingPassed: false,
			manualCheckingReviews: [],
			automaticChecking: null,
		},
		{
			code: 'void Main()\n' +
				'{\n' +
				'\tConsole.WriteLine("Coding is here right now tarasfaasfsaf PASSED 1ra");\n' +
				'\tConsole.WriteLine("Coding is here right now tarasfaasfsaf PASSED 2ra");\n' +
				'\tConsole.WriteLine("Coding is here right now tarasfaasfsaf PASSED 3ra");\n' +
				'\tEnd\n' +
				'}',
			language: Language.java,
			timestamp: '2020-04-01',
			manualCheckingPassed: true,
			manualCheckingReviews: [
				{
					author: user,
					startLine: 0,
					startPosition: 0,
					finishLine: 1,
					finishPosition: 1,
					comment: "Это ты зряяя",
					renderedComment: "Это ты зряяя",
					addingTime: "2020-08-04 23:04",
					comments: [],
				}
			],
			automaticChecking: {
				processStatus: AutomaticExerciseCheckingProcessStatus.Done,
				result: AutomaticExerciseCheckingResult.RightAnswer,
				output: null,
				checkerLogs: null,
				reviews: [
					{
						author: null,
						startLine: 2,
						startPosition: 0,
						finishLine: 2,
						finishPosition: 1,
						comment: "Робот не доволен",
						renderedComment: "Робот не доволен",
						addingTime: null,
						comments: [],
					}
				]
			},
		},
	].map((c, i) => ({
		...c,
		id: i,
		automaticChecking: c.automaticChecking
			? {
				...c.automaticChecking,
				reviews: c.automaticChecking?.reviews.map(addIdToReview) || null,
			}
			: null,
		manualCheckingReviews: c.manualCheckingReviews.map(addIdToReview)
	})),
	comments: [
		{ isFavourite: true, useCount: 5, text: 'комментарий', renderedText: 'комментарий' },
		{
			isFavourite: true,
			useCount: 10,
			text: '**bold** __italic__ ```code```',
		},
		{
			isFavourite: false,
			useCount: 100,
			text: 'Ой! Наш робот нашёл решения других студентов, подозрительно похожие на ваше. ' +
				'Так может быть, если вы позаимствовали части программы, взяли их из открытых источников либо сами поделились своим кодом. ' +
				'Выполняйте задания самостоятельно.',
		},
		{
			isFavourite: false,
			useCount: 122,
			text: 'Так делать не стоит из-за сложности в O(N^3). Есть более оптимизированные алгоритмы',
		},
	].map((c, i) => ({ ...c, renderedText: renderMd(c.text), id: i })),
	authorSolution: {
		language: Language.cSharp,
		code: 'void Main()\n{\n\tConsole.WriteLine("Coding is awesome");\n}',
	},
	formulation:
		<p>Вам надо сделать кое-что, сами гадайте что и как, но сделайте обязательно</p>,
	student: {
		visibleName: 'Студент Студентовичниковогоропараболладвойкавкоде',
		lastName: 'Студентовичниковогоропараболладвойкавкоде',
		firstName: 'Студент',
		id: "1",
		avatarUrl: "",
		email: "user@email.com",
		login: 'superStudnet',
	},
	group: {
		id: "groupId",
		apiUrl: 'groupApi',
		courseId: 'basicprogramming',
		isArchived: false,
		name: 'группа Екатеринбург АТ-666, 333 юг-запад Авеню Гейб'
	},
	user: {
		visibleName: 'Пользователь ДлиннаяФамилияКоторояМожетСломатьВерстку',
		lastName: 'ДлиннаяФамилияКоторояМожетСломатьВерстку',
		firstName: 'Пользователь',
		id: "0",
		avatarUrl: "",
		email: "user@email.com",
		login: 'Administrator of everything on ulearn.me',
	},
	prevReviewScore: 25,
	exerciseTitle: 'Angry Birds',
	onAddComment: (commentText: string) => {
		const comment = {
			id: args.comments.length,
			renderedText: renderMd(commentText),
			text: commentText,
			useCount: 1,
			isFavourite: true
		};
		const existingCommentIndex = args.comments.findIndex(c => c.text === commentText);
		if(existingCommentIndex < 0) {
			args.comments.push(comment);
			return returnPromiseAfterDelay(300, comment);
		} else {
			args.comments[existingCommentIndex].useCount++;
			return returnPromiseAfterDelay(300, args.comments[existingCommentIndex]);
		}
	},
	onAddCommentToFavourite: (commentText: string) => {
		const comment = {
			id: args.comments.length,
			renderedText: renderMd(commentText),
			text: commentText,
			useCount: 1,
			isFavourite: true
		};
		args.comments.push(comment);
		return returnPromiseAfterDelay(200, comment);
	},
	onToggleCommentFavourite: (commentId) => {
		const comment = args.comments[commentId];
		comment.isFavourite = !comment.isFavourite;
	},
	prohibitFurtherManualChecking: true,
	onProhibitFurtherReviewToggleChange: (value: boolean) => args.prohibitFurtherManualChecking = value,
	onScoreSubmit: (score: number) => {
		args.currentScore = score;
	},
	antiplagiarismStatus: undefined,
	getAntiPlagiarismStatus: () => {
		const rnd = Math.random();
		let suspicionLevel: AntiplagiarismInfo['suspicionLevel'] = 'accepted';
		let suspicionCount = 0;

		if(extra.suspicionLevel === 1) {
			suspicionCount = Math.ceil(rnd * 10);
			suspicionLevel = 'warning';
		}
		if(extra.suspicionLevel === 2) {
			suspicionCount = Math.ceil(rnd * 50);
			suspicionLevel = "strongWarning";
		}
		extra.suspicionLevel++;
		extra.suspicionLevel %= 3;
		const info: AntiplagiarismInfo = { suspicionCount, suspicionLevel };
		return returnPromiseAfterDelay(2000, info);
	},
	deleteReviewComment: (submissionId, reviewId, commentId) => {
		const submission = args.studentSubmissions.find(s => s.id === submissionId);
		if(submission) {
			if(commentId !== undefined) {
				const review = submission.manualCheckingReviews.find(c => c.id === reviewId)!;
				review.comments = review.comments.filter(c => c !== commentId);
				submission.manualCheckingReviews = [...submission.manualCheckingReviews.slice(0,
					submission.manualCheckingReviews.length - 2), { ...review }];
				return;
			}
			submission.manualCheckingReviews = submission.manualCheckingReviews.filter(c => c.id !== reviewId);
		}
	},
	addReview: (
		submissionId: number,
		comment: string,
		startLine: number,
		startPosition: number,
		finishLine: number,
		finishPosition: number
	) => {
		const submission = args.studentSubmissions.find(s => s.id === submissionId)!;
		const review: ReviewInfo = {
			id: extra.reviewId++,
			author: args.user,
			startLine,
			startPosition,
			finishLine,
			finishPosition,
			comment,
			renderedComment: renderMd(comment),
			addingTime: new Date().toDateString(),
			comments: [],
		};
		submission.manualCheckingReviews.push(review);

		return returnPromiseAfterDelay(300, review);
	},
	addReviewComment: (submissionId, reviewId, commentText) => {
		const submission = args.studentSubmissions.find(s => s.id === submissionId)!;
		const review = submission.manualCheckingReviews.find(r => r.id === reviewId)!;
		const comment = {
			id: review.comments.length,
			text: commentText,
			renderedText: renderMd(commentText),
			publishTime: new Date().toDateString(),
			author: args.user,
		};
		review.comments.push(comment);
	},
	currentScore: undefined,
};

Default.args = args;

export default {
	title: 'Exercise/InstructorReview',
};
