import { Dispatch } from "redux";
import api from "./index";
import {
	studentLoadSuccessAction,
	studentLoadFailAction,
	studentLoadStartAction,
	studentSubmissionsLoadStartAction,
	studentSubmissionsLoadSuccessAction,
	studentSubmissionsLoadFailAction,
	antiplagiarimsStatusLoadStartAction,
	antiplagiarimsStatusLoadSuccessAction,
	antiplagiarimsStatusLoadFailAction,
	favouriteReviewsLoadStartAction,
	favouriteReviewsLoadSuccessAction,
	favouriteReviewsLoadFailAction,
} from "src/actions/instructor";
import { ShortUserInfo } from "src/models/users";
import { antiplagiarism, favouriteReviews, submissions } from "src/consts/routes";
import { buildQuery } from "src/utils";
import {
	AntiplagiarismStatusResponse,
	FavouriteReviewResponse,
	StudentSubmissionsResponse
} from "src/models/instructor";
import { SubmissionInfo } from "src/models/exercise";

export function getStudentInfo(studentId: string,) {
	return (dispatch: Dispatch): Promise<string | ShortUserInfo> => {
		dispatch(studentLoadStartAction(studentId,));
		return api.users.getUserInfo(studentId,)
			.then(json => {
				const user = json.users.find(userInfo => userInfo.user.id === studentId)?.user;
				if(user) {
					dispatch(studentLoadSuccessAction(user));
					return user;
				} else {
					throw new Error('User not found, or you don\'t have permission');
				}
			})
			.catch(error => {
				dispatch(studentLoadFailAction(studentId, error));
				return error;
			});
	};
}

export function getStudentSubmissions(studentId: string, courseId: string, slideId: string,) {
	return (dispatch: Dispatch): Promise<SubmissionInfo[] | string> => {
		dispatch(studentSubmissionsLoadStartAction(studentId, courseId, slideId,));
		const url = submissions + buildQuery({ studentId, courseId, slideId, });
		return api.get<StudentSubmissionsResponse>(url)
			.then(json => {
				dispatch(studentSubmissionsLoadSuccessAction(studentId, courseId, slideId, json));
				return json.submissions;
			})
			.catch(error => {
				dispatch(studentSubmissionsLoadFailAction(studentId, courseId, slideId, error,));
				return error;
			});
	};
}

export function getAntiplagiarismStatus(submissionId: string,) {
	return (dispatch: Dispatch): Promise<AntiplagiarismStatusResponse | string> => {
		dispatch(antiplagiarimsStatusLoadStartAction(submissionId,));
		const url = `${ antiplagiarism }/${ submissionId }`;
		return api.get<AntiplagiarismStatusResponse>(url)
			.then(json => {
				dispatch(antiplagiarimsStatusLoadSuccessAction(submissionId, json,));
				return json;
			})
			.catch(error => {
				dispatch(antiplagiarimsStatusLoadFailAction(submissionId, error,));
				return error;
			});
	};
}

export function getFavouriteReviews(courseId: string, slideId: string,) {
	return (dispatch: Dispatch): Promise<FavouriteReviewResponse | string> => {
		dispatch(favouriteReviewsLoadStartAction(courseId, slideId,));
		const url = favouriteReviews + buildQuery({ courseId, slideId, });
		return api.get<FavouriteReviewResponse>(url)
			.then(json => {
				dispatch(favouriteReviewsLoadSuccessAction(courseId, slideId, json,));
				return json;
			})
			.catch(error => {
				dispatch(favouriteReviewsLoadFailAction(courseId, slideId, error,));
				return error;
			});
	};
}
