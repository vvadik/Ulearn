import config from "../config"

export function getNotificationsCount(lastTimestamp) {
    return dispatch => {
        return fetch(config.api.endpoint + "notifications/count?last_timestamp=" + (lastTimestamp ? lastTimestamp : ""), { credentials: 'include' })
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