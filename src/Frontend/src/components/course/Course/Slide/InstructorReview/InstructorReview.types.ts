import { UserInfo } from "src/utils/courseRoles";
import { Response, ShortGroupInfo } from "src/models/comments";
import {
	AntiplagiarismInfo,
	AntiplagiarismStatusResponse,
	FavouriteReview,
	FavouriteReviewResponse
} from "src/models/instructor";
import { ShortUserInfo } from "src/models/users";
import { ReviewCommentResponse, ReviewInfo, SubmissionInfo } from "src/models/exercise";
import { GroupsInfoResponse } from "src/models/groups";
import React from "react";
import { SlideContext } from "../Slide";
import { ReviewInfoWithMarker, TextMarkersByReviewId } from "../Blocks/Exercise/ExerciseUtils";
import { InstructorReviewTabs } from "./InstructorReviewTabs";
import { DiffInfo } from "./utils";
import CodeMirror, { Editor } from "codemirror";

export interface PropsFromRedux {
	user?: UserInfo;

	favouriteReviews?: FavouriteReview[];

	student?: ShortUserInfo;
	studentGroups?: ShortGroupInfo[];
	studentSubmissions?: SubmissionInfo[];
	scoresBySubmissionId?: { [submissionId: number]: number; };

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

	onScoreSubmit: (submissionId: number, score: number) => Promise<number>;
	onProhibitFurtherReviewToggleChange: (value: boolean) => void;
	onAddReview: (comment: string) => Promise<FavouriteReview>;
	onAddReviewToFavourite: (comment: string) => Promise<FavouriteReview>;
	onToggleReviewFavourite: (commentId: number) => Promise<FavouriteReview>;

	addReview: (
		submissionId: number,
		comment: string,
		startLine: number,
		startPosition: number,
		finishLine: number,
		finishPosition: number,
	) => Promise<ReviewInfo>;
	addReviewComment: (submissionId: number, reviewId: number, comment: string) => Promise<ReviewCommentResponse>;
	deleteReviewOrComment: (submissionId: number, reviewId: number, commentId?: number) => Promise<Response>;
	editReviewOrComment: (text: string, submissionId: number, reviewId: number,
		commentId?: number
	) => Promise<ReviewInfo | ReviewCommentResponse>;
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
	startLine: number;
	anchor?: number;
	instructor?: InstructorExtraFields;
}

export interface InstructorReviewInfo extends ReviewInfo {
	instructor?: InstructorExtraFields;
}

export interface InstructorReviewInfoWithAnchor extends InstructorReviewInfo {
	anchor: number;
}

export type InstructorReviewInfoWithMarker = InstructorReviewInfo & ReviewInfoWithMarker;

export interface State {
	currentTab: InstructorReviewTabs;

	currentSubmission: SubmissionInfo | undefined;

	showDiff: boolean;
	diffInfo?: DiffInfo;

	selectedReviewId: number;
	reviews: ReviewInfo[];
	outdatedReviews: ReviewInfo[];
	markers: TextMarkersByReviewId;

	curScore?: number;
	prevScore?: number;

	editor: null | Editor;

	addCommentValue: string;
	addCommentFormCoords?: { left: number; top: number; bottom: number };
	addCommentRanges?: { startRange: CodeMirror.Position; endRange: CodeMirror.Position; };

	initialCode?: string;

	favouriteReviewsSet: Set<string>;
	favouriteByUserSet: Set<string>;
}
