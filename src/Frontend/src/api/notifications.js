import config from "../config"

export function getNotificationsCount() {
    return dispatch => {
        return fetch(config.api.endpoint + "notifications/count", { credentials: 'include' })
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