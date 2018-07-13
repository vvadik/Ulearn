import config from "../config"
import api from "../api"

export function getCurrentUser() {
    return dispatch => {
        // dispatch({ type: 'ACCOUNT_USER_INFO_STARTED' })
        return fetch(config.api.endpoint + 'account', { credentials: 'include' })
            .then(response => response.json())
            .then(json => {
                let isAuthenticated = json.is_authenticated;
                if (! isAuthenticated)
                    dispatch({ type: 'ACCOUNT__USER_INFO_UPDATED', isAuthenticated: false });
                else {
                    let user = json.user;
                    dispatch({
                        type: 'ACCOUNT__USER_INFO_UPDATED',
                        isAuthenticated: true,
                        login: user.login,
                        firstName: user.first_name,
                        lastName: user.last_name
                    });
                    dispatch(api.account.getRoles());
                }
            })
    }
}

export function getRoles() {
    return dispatch => {
        return fetch(config.api.endpoint + 'account/roles', { credentials: 'include' })
            .then(response => response.json())
            .then(json => {
                let courseRoles = json.course_roles;
                let courseRolesObject = {};
                courseRoles.forEach(c => courseRolesObject[c.course_id] = c.role);
                dispatch({
                    type: 'ACCOUNT__USER_ROLES_UPDATED',
                    isSystemAdministrator: json.is_system_administrator,
                    roleByCourse: courseRolesObject
                })
            })
    }
}
