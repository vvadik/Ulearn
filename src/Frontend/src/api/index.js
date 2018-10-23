import config from "../config"

import * as account from "./account"
import * as courses from "./courses"
import * as notifications from "./notifications"
import * as groups from "./groups"

const API_JWT_TOKEN_UPDATED = 'API_JWT_TOKEN_UPDATED';
let apiJwtToken = '';

function refreshApiJwtToken() {
    return fetch(config.api.endpoint + 'account/token', { credentials: 'include', method: 'POST' })
        .then(response => {
            if (response.status !== 200) {
                let error = new Error(response.statusText || response.status);
                error.response = response;
                return Promise.reject(error);
            }

            return response.json();
        })
        .then(json => {
            let token = json.token;
            if (! token)
                return Promise.reject(new Error('Can\'t get token from API: /account/token returned bad json: ' + JSON.stringify(json)))
            apiJwtToken = token;
            return Promise.resolve(API_JWT_TOKEN_UPDATED);
        })
}

function request(url, options, isRetry) {
    options = options || {};
    options.credentials = options.credentials || 'include';
    options.headers = options.headers || {};
    options.headers['Authorization'] = 'Bearer ' + apiJwtToken;
    return fetch(config.api.endpoint + url, options)
        .then(response => {
            if (response.status >= 200 && response.status < 300)
                return Promise.resolve(response);
            if (response.status === 401 && ! isRetry)
                return refreshApiJwtToken();
            return Promise.reject(response);
        })
        .then(value => {
            if (value === API_JWT_TOKEN_UPDATED)
                return request(url, options, true);
            return Promise.resolve(value);
        })
}

function get(url, options) {
    return request(url, options);
}

function post(url, options) {
    options = options || {};
    options.method = 'POST';
    return request(url, options);
}

function patch(url, options) {
	options = options || {};
	options.method = 'PATCH';
	return request(url, options);
}

let api = {
    account: account,
    courses: courses,
    notifications: notifications,
	groups: groups,
    get: get,
    post: post,
	patch: patch,
    request: request
};


export default api;

