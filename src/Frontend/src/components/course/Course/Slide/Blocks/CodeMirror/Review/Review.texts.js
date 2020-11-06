import moment from "moment";

import { convertDefaultTimezoneToLocal } from "src/utils/getMoment";

const texts = {
	sendButton: 'Ответить',

	getLineCapture: (startLine, finishLine) => {
		return startLine === finishLine ? `строка ${ startLine + 1 }` : `строки ${ startLine + 1 }-${ finishLine + 1 }`;
	},

	getAddingTime: (addingTime) => {
		return moment(convertDefaultTimezoneToLocal(addingTime)).format("DD MMMM YYYY")
	},
};

export default texts;
