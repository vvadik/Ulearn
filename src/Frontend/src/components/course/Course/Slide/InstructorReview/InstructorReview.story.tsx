import React from "react";
import InstructorReview from "./InstructorReview";
import { Language } from "src/consts/languages";
import type { Story } from "@storybook/react";
import {
	AutomaticExerciseCheckingProcessStatus,
	AutomaticExerciseCheckingResult, ReviewCommentResponse,
	ReviewInfo, SubmissionInfo,
} from "src/models/exercise";
import { ShortUserInfo } from "src/models/users";
import { returnPromiseAfterDelay } from "src/utils/storyMock";
import { getMockedUser } from "../../../../comments/storiesData";
import { renderMd, StoryUpdater } from "src/storiesUtils";
import {
	AntiplagiarismInfo,
	AntiplagiarismStatusResponse,
	FavouriteReview,
	FavouriteReviewResponse
} from "src/models/instructor";
import { GroupInfo, GroupsInfoResponse } from "src/models/groups";
import { UserInfo } from "src/utils/courseRoles";
import { BlocksWrapper, StaticCode } from "../Blocks";
import { Props } from "./InstructorReview.types";
import { clone } from "../../../../../utils/jsonExtensions";


const user: UserInfo = getMockedUser({
	visibleName: 'Пользователь ДлиннаяФамилияКоторояМожетСломатьВерстку',
	lastName: 'ДлиннаяФамилияКоторояМожетСломатьВерстку',
	firstName: 'Пользователь',
	id: "0",
	avatarUrl: "",
	email: "user@email.com",
	login: 'Administrator of everything on ulearn.me',
});

const Template: Story<Props> = (args: Props) => {
	return (
		<StoryUpdater args={ args } childrenBuilder={ (args) =>
			<InstructorReview
				{ ...args }
			/> }
		/>);
};

const extra = {
	suspicionLevel: 0,
	reviewId: 0,
};

const addIdToReview = (review: any): ReviewInfo => ({
	...review, id: extra.reviewId++,
});

const studentGroups: GroupInfo[] = [{
	id: 12,
	apiUrl: 'groupApi',
	isArchived: false,
	name: 'группа Екатеринбург АТ-666, 333 юг-запад Авеню Гейб',
	accesses: [],
	areYouStudent: false,
	canStudentsSeeGroupProgress: false,
	createTime: null,
	defaultProhibitFurtherReview: true,
	inviteHash: '',
	isInviteLinkEnabled: false,
	isManualCheckingEnabled: true,
	isManualCheckingEnabledForOldSolutions: false,
	owner: user,
	studentsCount: 20,
}];

const favouriteReviews = [
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
].map((c, i) => ({ ...c, renderedText: renderMd(c.text), id: i }));

const submissions = [
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
}));

const loadingTimes = {
	student: 100,
	groups: 100,
	favouriteReviews: 100,
	addReview: 100,
	toggleReviewFavourite: 100,
	getPlagiarismStatus: 100,
	submissions: 100,
};

const args: Props = {
	slideContext: { slideId: 'slide', courseId: 'basic', title: 'Angry Birds', },
	authorSolution: <BlocksWrapper>
		<StaticCode
			language={ Language.cSharp }
			code={ 'void Main()\n{\n\tConsole.WriteLine("Coding is awesome");\n}' }/>
	</BlocksWrapper>,
	formulation:
		<BlocksWrapper>
			<p>Вам надо сделать кое-что, сами гадайте что и как, но сделайте обязательно</p>
		</BlocksWrapper>,
	user,
	studentId: '1',
	prohibitFurtherManualChecking: true,

	onAddReview(commentText: string) {
		const comment: FavouriteReview = {
			id: this.favouriteReviews?.length || 0,
			renderedText: renderMd(commentText),
			text: commentText,
			useCount: 1,
			isFavourite: true,
		};

		const existingCommentIndex = this.favouriteReviews?.findIndex(c => c.text === commentText);
		if(existingCommentIndex && this.favouriteReviews) {
			if(existingCommentIndex < 0) {
				return returnPromiseAfterDelay(loadingTimes.addReview, comment, () => {
					this.favouriteReviews?.push(comment);
				});
			} else {
				const comment = this.favouriteReviews[existingCommentIndex];
				return returnPromiseAfterDelay(loadingTimes.addReview, comment, () => {
					comment.useCount++;
				});
			}
		}

		return returnPromiseAfterDelay(loadingTimes.addReview, comment);
	},
	onAddReviewToFavourite(commentText: string) {
		const comment: FavouriteReview = {
			id: this.favouriteReviews?.length || 0,
			renderedText: renderMd(commentText),
			text: commentText,
			useCount: 1,
			isFavourite: true,
		};

		if(!this.favouriteReviews) {
			return Promise.resolve(comment);
		}
		this.favouriteReviews = [...this.favouriteReviews, comment];
		return returnPromiseAfterDelay(loadingTimes.toggleReviewFavourite, comment);
	},
	onToggleReviewFavourite(favouriteCommentId: number) {
		if(this.favouriteReviews) {
			const commentIndex = this.favouriteReviews.findIndex(c => c.id === favouriteCommentId);
			if(commentIndex > -1) {
				this.favouriteReviews = clone(this.favouriteReviews);
				const comment = this.favouriteReviews[commentIndex];
				this.favouriteReviews[commentIndex] = { ...comment, isFavourite: !comment.isFavourite };
			}
		}
	},
	onProhibitFurtherReviewToggleChange(value: boolean) {
		this.prohibitFurtherManualChecking = value;
	},
	onScoreSubmit(score: number) {
		return;
	},
	deleteReviewOrComment(submissionId: number, id: number, reviewId?: number,) {
		const submission = this.studentSubmissions?.find(s => s.id === submissionId);
		if(submission) {
			if(reviewId !== undefined) {
				const review = submission.manualCheckingReviews.find(c => c.id === id);
				if(review) {
					review.comments = review.comments.filter(c => c.id !== reviewId);
					submission.manualCheckingReviews = [...submission.manualCheckingReviews.slice(0,
						submission.manualCheckingReviews.length - 2), { ...review }];
					return;
				}
			}
			submission.manualCheckingReviews = submission.manualCheckingReviews.filter(c => c.id !== id);
		}
	},
	addReview(
		submissionId: number,
		comment: string,
		startLine: number,
		startPosition: number,
		finishLine: number,
		finishPosition: number
	) {
		const submission = this.studentSubmissions?.find(s => s.id === submissionId);
		const review: ReviewInfo = {
			id: extra.reviewId++,
			author: this.user || null,
			startLine,
			startPosition,
			finishLine,
			finishPosition,
			comment,
			renderedComment: renderMd(comment),
			addingTime: new Date().toDateString(),
			comments: [],
		};
		if(submission) {
			submission.manualCheckingReviews.push(review);
		}
		return returnPromiseAfterDelay(loadingTimes.addReview, review);
	},
	editReviewOrComment(text, submissionId, id, reviewId) {
		const review = this.studentSubmissions
			?.find(s => s.id === submissionId)?.manualCheckingReviews
			.find(r => r.id === id || r.id === reviewId || r.comments.some(c => c.id === id));
		if(review) {
			if(reviewId) {
				const comment = review.comments.find(c => c.id === id);
				if(comment) {
					comment.text = text;
					comment.renderedText = renderMd(text);
				}
			} else {
				review.comment = text;
				review.renderedComment = renderMd(text);
			}
		}
	},
	addReviewComment(submissionId: number, reviewId: number, commentText: string) {
		const submission = this.studentSubmissions?.find(s => s.id === submissionId);
		const review = submission?.manualCheckingReviews.find(r => r.id === reviewId);
		if(review && this.user) {
			const comment: ReviewCommentResponse = {
				id: review.comments.length,
				text: commentText,
				renderedText: renderMd(commentText),
				publishTime: new Date().toDateString(),
				author: this.user,
			};
			review.comments.push(comment);
		}
	},
	getAntiplagiarismStatus(submissionId: number): Promise<AntiplagiarismStatusResponse | string> {
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
		const info: AntiplagiarismStatusResponse = {
			suspiciousAuthorsCount: suspicionCount,
			suspicionLevel,
			status: 'checked'
		};

		return returnPromiseAfterDelay(loadingTimes.getPlagiarismStatus, info, () => {
			this.antiplagiarismStatus = info;
		});
	},
	getStudentGroups(courseId: string, userId: string): Promise<GroupsInfoResponse | string> {
		return returnPromiseAfterDelay(loadingTimes.groups, { groups: studentGroups }, () => {
			this.groups = studentGroups.map(g => ({ ...g, courseId, }));
		});
	},
	getFavouriteReviews(courseId: string, slideId: string): Promise<FavouriteReviewResponse | string> {
		const reviews = clone(favouriteReviews);
		const response: FavouriteReviewResponse = { reviews: reviews };
		return returnPromiseAfterDelay(loadingTimes.favouriteReviews, response, () => {
			this.favouriteReviews = reviews;
		});
	},
	getStudentInfo(studentId: string): Promise<ShortUserInfo | string> {
		const student = {
			visibleName: 'Студент Студентовичниковогоропараболладвойкавкоде',
			lastName: 'Студентовичниковогоропараболладвойкавкоде',
			firstName: 'Студент',
			id: studentId,
			avatarUrl: "",
			email: "user@email.com",
			login: 'superStudnet',
		};
		return returnPromiseAfterDelay(loadingTimes.student, student, () => {
			this.student = student;
		});
	},
	getStudentSubmissions(studentId: string, courseId: string, slideId: string): Promise<SubmissionInfo[] | string> {
		return returnPromiseAfterDelay(loadingTimes.submissions, submissions, () => {
			this.studentSubmissions = submissions;
		});
	},
};

export const Default = Template.bind({});
Default.args = args;

export default {
	title: 'Exercise/InstructorReview',
};
