import { ShortUserInfo } from "./users";

export interface AcceptedSolutionsResponse {
	promotedSolutions: AcceptedSolution[],
	randomLikedSolutions: AcceptedSolution[],
	newestSolutions: AcceptedSolution[],
}

export interface LikedAcceptedSolutionsResponse {
	likedSolutions: AcceptedSolution[],
}

export interface AcceptedSolution {
	submissionId: number,
	code: string,
	likesCount: number | null,
	likedByMe: boolean | null,
	promotedBy?: ShortUserInfo,
}
