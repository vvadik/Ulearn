import { SubmissionInfo, } from "./exercise";

export interface StudentSubmissionsResponse {
	submissions: SubmissionInfo[];
}

export type SuspicionLevel = 'notChecking' | 'accepted' | 'warning' | 'strongWarning' | 'running';

export interface AntiplagiarismInfo {
	suspicionLevel: SuspicionLevel;
	suspiciousAuthorsCount: number;
}

export interface AntiplagiarismStatusResponse extends AntiplagiarismInfo {
	status: "notChecked" | "checked";
}

export interface FavouriteReview {
	isFavourite: boolean;
	renderedText: string;
	text: string;
	id: number;
	useCount: number;
}


export interface FavouriteReviewResponse {
	reviews: FavouriteReview[];
}
