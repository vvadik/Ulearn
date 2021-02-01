import api from "./index";
import { Dispatch } from "redux";
import { notificationUpdateAction } from "src/actions/notifications";
import { NotificationsInfo } from "src/models/notifications";

export function getNotificationsCount(lastTimestamp?: string) {
	return (dispatch: Dispatch): Promise<void> => {
		return api.get<NotificationsInfo>("notifications/count?last_timestamp=" + (lastTimestamp ? lastTimestamp : ""))
			.then(json => {
				dispatch(notificationUpdateAction(json.count, json.lastTimestamp));
			});
	};
}
