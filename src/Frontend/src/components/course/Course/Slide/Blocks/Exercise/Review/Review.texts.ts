import { convertDefaultTimezoneToLocal } from "src/utils/momentUtils.js";

export default {
	sendButton: 'Ответить',

	getLineCapture: (startLine: number, finishLine: number): string => {
		return startLine === finishLine ? `строка ${ startLine + 1 }` : `строки ${ startLine + 1 }-${ finishLine + 1 }`;
	},

	getAddingTime: (addingTime: string): string => {
		return convertDefaultTimezoneToLocal(addingTime).format("DD MMMM YYYY");
	},
};
