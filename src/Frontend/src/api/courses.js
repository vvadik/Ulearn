import api from "../api"

export function getCourses() {
    return dispatch => {

        return api.get("courses")
            .then(response => response.json())
            .then(json => {
                let courseById = {};
                json.courses.forEach(c => courseById[c.id.toLowerCase()] = c);
                dispatch({
                    type: 'COURSES__UPDATED',
                    courseById: courseById
                })
            });
    };
}

export function getUserCourses() {
	return api.get('courses?role=instructor')
		.then(response => response.json());
}