import moment from "moment-timezone";
import { DEFAULT_TIMEZONE } from "../consts/general";

export default function getMoment(time) {
	return moment(moment.tz(time, DEFAULT_TIMEZONE).format()).fromNow();
}

export function getDateDDMMYY(time, format = 'DD MMMM YYYY в HH:mm') {
	return moment(time).format(format);
}

export function convertDefaultTimezoneToLocal(timeInDefaultTimezone) {
	return moment.tz(timeInDefaultTimezone, DEFAULT_TIMEZONE).local();
}
