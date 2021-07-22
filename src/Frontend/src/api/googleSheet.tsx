import api from "./index";
import { buildQuery } from "../utils";
import {
	GoogleSheetsExportTaskListResponse,
	GoogleSheetsExportTaskParams,
	GoogleSheetsExportTaskResponse
} from "../models/googleSheet";

export function getAllCourseTasks(courseId: string): Promise<GoogleSheetsExportTaskListResponse> {
	const url = `course-score-sheet/export/to-google-sheets/tasks` + buildQuery({ courseId });
	return api.get(url);
}

export function updateCourseTask(
	taskId: number,
	params: {
		courseId: string,
		isVisibleForStudents: boolean,
		refreshStartDate?: string,
		refreshEndDate?: string,
		refreshTimeInMinutes?: number,
	}
): Promise<GoogleSheetsExportTaskResponse> {
	const url = `course-score-sheet/export/to-google-sheets/tasks/${taskId}`;
	return api.patch(url,
		api.createRequestParams(params));
}
