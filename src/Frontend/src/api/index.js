import config from "../config"

import * as account from "./account"
import * as courses from "./courses"
import * as notifications from "./notifications"
import * as groups from "./groups"
import * as users from "./users"
import * as comments from "./comments"

const API_JWT_TOKEN_UPDATED = "API_JWT_TOKEN_UPDATED";
let apiJwtToken = "";
let serverErrorHandler = () => {
};

function setServerErrorHandler(handler) {
	serverErrorHandler = handler;
}

function refreshApiJwtToken() {
	return fetch(config.api.endpoint + "account/token", {credentials: "include", method: "POST"})
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
		if (!token)
			return Promise.reject(new Error('Can\'t get token from API: /account/token returned bad json: ' + JSON.stringify(json)));
		apiJwtToken = token;
		return Promise.resolve(API_JWT_TOKEN_UPDATED);
	})
}

function clearApiJwtToken() {
	apiJwtToken = ""
}

function request(url, options, isRetry) {
	options = options || {};
	options.credentials = options.credentials || "include";
	options.headers = options.headers || {};
	options.headers["Authorization"] = "Bearer " + apiJwtToken;
	return fetch(config.api.endpoint + url, options)
	.catch((error) => {
		if (window.navigator.onLine === false)
			serverErrorHandler("Не можем подключиться к серверу");
		else
			serverErrorHandler("Не можем подключиться к серверу. Попробуйте обновить страницу.");

		throw error;
	})
	.then(response => {
		if (response.status >= 200 && response.status < 300)
			return response;
		if (response.status === 401 && !isRetry)
			return refreshApiJwtToken();

		if (response.status >= 500)
			serverErrorHandler();

		throw new Error(`HTTP response code: ${response.status}`);
	})
	.catch((error) => {
		console.error(error);
		throw error;
	})
	.then(value => {
		if (value === API_JWT_TOKEN_UPDATED)
			return request(url, options, true);

		return value.json();
	})
}

function get(url, options) {
	return request(url, options);
}

function post(url, options) {
	options = options || {};
	options.method = "POST";
	return request(url, options);
}

function patch(url, options) {
	options = options || {};
	options.method = "PATCH";
	return request(url, options);
}

function put(url, options) {
	options = options || {};
	options.method = "PUT";
	return request(url, options);
}

function deleteRequest(url, options) { /* delete - зарезервированное слово, поэтому так */
	options = options || {};
	options.method = "DELETE";
	return request(url, options);
}

function createRequestParams(body) {
	return {
		headers: {
			"Content-Type": "application/json",
		},
		body: JSON.stringify(body)
	}
}

let api = {
	refreshApiJwtToken: refreshApiJwtToken,
	clearApiJwtToken: clearApiJwtToken,
	setServerErrorHandler: setServerErrorHandler,

	request: request,
	createRequestParams: createRequestParams,

	get: get,
	post: post,
	patch: patch,
	put: put,
	delete: deleteRequest,

	account: account,
	courses: courses,
	notifications: notifications,
	groups: groups,
	users: users,
	comments: comments,
};

export default api;

