import api from "../api"

export function getNotificationsCount(lastTimestamp) {
    return dispatch => {
        return api.get("notifications/count?last_timestamp=" + (lastTimestamp ? lastTimestamp : ""))
            .then(response => response.json())
            .then(json => {
                dispatch({
                    type: 'NOTIFICATIONS__COUNT_UPDATED',
                    count: json.count,
                    lastTimestamp: json.last_timestamp,
                })
            })
    }
}