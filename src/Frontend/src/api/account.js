import api from "../api"

export function getCurrentUser() {
    return dispatch => {
        return api.get('account')
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
                        lastName: user.last_name,
                        visibleName: user.visible_name,
                        accountProblems: json.account_problems
                    });
                    dispatch(api.account.getRoles());
                }
            })
            .catch(error => {
                if (error.response && error.response.status === 401) { // Unauthorized
                    dispatch({ type: 'ACCOUNT__USER_INFO_UPDATED', isAuthenticated: false });
                }
            })
    }
}

export function getRoles() {
    return dispatch => {
        return api.get('account/roles')
            .then(response => response.json())
            .then(json => {
                let courseRoles = json.course_roles;
                let courseRolesObject = {};
                courseRoles.forEach(c => courseRolesObject[c.course_id.toLowerCase()] = c.role);

                let courseAccesses = json.course_accesses;
                let courseAccessesObject = {};
                courseAccesses.forEach(c => courseAccessesObject[c.course_id.toLowerCase()] = c.accesses);

                dispatch({
                    type: 'ACCOUNT__USER_ROLES_UPDATED',
                    isSystemAdministrator: json.is_system_administrator,
                    roleByCourse: courseRolesObject,
                    accessesByCourse: courseAccessesObject
                })
            })
    }
}

export function logout() {
    return dispatch => {
        return api.post('account/logout')
            .then(response => response.json())
            .then(json => {
                if (json.logout) {
                	api.clearApiJwtToken();
					dispatch({
						type: 'ACCOUNT__USER_LOGOUTED',
					});
				}
            })
    }
}
