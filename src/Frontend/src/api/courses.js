import config from "../config"

export function getCourses() {
    return dispatch => {
        return fetch(config.api.endpoint + "courses", {credentials: 'include'})
            .then(response => response.json())
            .then(json => {
                let courseById = {};
                json.courses.forEach(c => courseById[c.id] = c);
                dispatch({
                    type: 'COURSES__UPDATED',
                    courseById: courseById
                })
            });
    };
}