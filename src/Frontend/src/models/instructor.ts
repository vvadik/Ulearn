import { SubmissionInfo, } from "./exercise";

export interface StudentSubmissionsResponse {
	submissions: SubmissionInfo[];
}

export type SuspicionLevel = 'notChecking' | 'accepted' | 'warning' | 'strongWarning' | 'running';

export interface AntiplagiarismStatusResponse {
	status: "notChecked" | "checked";
	suspicionLevel: SuspicionLevel;
	suspiciousAuthorsCount: number;
}

export interface FavouriteReview {
	isFavourite: boolean;
	renderedComment: string;
	id: number;
}


export interface FavouriteReviewResponse {
	reviews: FavouriteReview[];
}
