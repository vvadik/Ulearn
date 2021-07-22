import { ShortGroupInfo } from "./comments";
import { ShortUserInfo } from "./users";

export interface GoogleSheetsExportTaskResponse {
	id: number;
	groups: ShortGroupInfo[];
	authorInfo: ShortUserInfo;
	isVisibleForStudents: boolean;
	refreshStartDate?: string;
	refreshEndDate?: string;
	refreshTimeInMinutes: number;
	spreadsheetId: string;
	listId: number;
}

export interface GoogleSheetsExportTaskParams {
	courseId: string;
	groupsIds?: number[];
	isVisibleForStudents: boolean;
	refreshStartDate?: string;
	refreshEndDate?: string;
	refreshTimeInMinutes?: number;
	spreadsheetId: string;
	listId: number;
}

export interface GoogleSheetsExportTaskListResponse {
	googleSheetsExportTasks: GoogleSheetsExportTaskResponse[];
}
