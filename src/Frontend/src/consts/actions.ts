// common
export const START = "_START";
export const SUCCESS = "_SUCCESS";
export const FAIL = "_FAIL";

const load = 'LOAD';
export const loadStart = load + START;
export const loadSuccess = load + SUCCESS;
export const loadFail = load + FAIL;

export interface FailAction {
	error: string;
}
