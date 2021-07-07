import { UserInfo } from "src/utils/courseRoles";
import { ShortGroupInfo } from "src/models/comments";
import {
	AntiplagiarismInfo,
	AntiplagiarismStatusResponse,
	FavouriteReview,
	FavouriteReviewResponse
} from "src/models/instructor";
import { ShortUserInfo } from "src/models/users";
import { ReviewInfo, SubmissionInfo } from "src/models/exercise";
import { GroupsInfoResponse } from "src/models/groups";
import React from "react";
import { SlideContext } from "../Slide";
import { ReviewInfoWithMarker } from "../Blocks/Exercise/ExerciseUtils";
import { InstructorReviewTabs } from "./InstructorReviewTabs";
import { DiffInfo } from "./utils";
import CodeMirror, { Editor } from "codemirror";

export interface PropsFromRedux {
	user?: UserInfo;
	groups?: ShortGroupInfo[];

	favouriteReviews?: FavouriteReview[];

	student?: ShortUserInfo;
	studentSubmissions?: SubmissionInfo[];

	antiplagiarismStatus?: AntiplagiarismInfo;
	prohibitFurtherManualChecking: boolean;
}

export interface ApiFromRedux {
	getStudentInfo: (studentId: string,) => Promise<ShortUserInfo | string>;
	getStudentSubmissions: (studentId: string, courseId: string,
		slideId: string,
	) => Promise<SubmissionInfo[] | string>;
	getAntiplagiarismStatus: (submissionId: number,) => Promise<AntiplagiarismStatusResponse | string>;
	getFavouriteReviews: (courseId: string, slideId: string,) => Promise<FavouriteReviewResponse | string>;
	getStudentGroups: (courseId: string, studentId: string,) => Promise<GroupsInfoResponse | string>;

	onScoreSubmit: (score: number) => void;
	onProhibitFurtherReviewToggleChange: (value: boolean) => void;
	onAddReview: (comment: string) => Promise<FavouriteReview>;
	onAddReviewToFavourite: (comment: string) => Promise<FavouriteReview>;
	onToggleReviewFavourite: (commentId: number) => void;

	addReview: (
		submissionId: number,
		comment: string,
		startLine: number,
		startPosition: number,
		finishLine: number,
		finishPosition: number,
	) => Promise<ReviewInfo>;
	addReviewComment: (submissionId: number, reviewId: number, comment: string) => void;
	deleteReviewOrComment: (submissionId: number, reviewId: number, commentId?: number) => void;
	editReviewOrComment: (text: string, submissionId: number, reviewId: number, commentId?: number) => void;
}

export interface Props extends PropsFromRedux, ApiFromRedux {
	authorSolution?: React.ReactNode;
	formulation?: React.ReactNode;
	slideContext: SlideContext;
	studentId: string;
}

export interface InstructorExtraFields {
	outdated?: boolean;
	isFavourite?: boolean;
}

export interface ReviewCompare {
	comment: string;
	comments: string[];
	instructor?: InstructorExtraFields;
}

export interface InstructorReviewInfo extends ReviewInfo {
	instructor?: InstructorExtraFields;
}

export type InstructorReviewInfoWithMarker = InstructorReviewInfo & ReviewInfoWithMarker;

export interface State {
	currentTab: InstructorReviewTabs;

	currentSubmission: SubmissionInfo | undefined;

	diffInfo?: DiffInfo;
	selectedReviewId: number;
	reviewsWithTextMarkers: InstructorReviewInfoWithMarker[];

	editor: null | Editor;

	commentValue: string;
	addCommentFormCoords?: { left: number; top: number; bottom: number };
	ranges?: { startRange: CodeMirror.Position; endRange: CodeMirror.Position; };

	showDiff: boolean;
	initialCode?: string;

	prevReviewScore?: number;
	currentScore?: number;

	favouriteReviewsSet: Set<string>;
	favouriteByUserSet: Set<string>;
	outdatedReviewsSet: Set<number>;
}
