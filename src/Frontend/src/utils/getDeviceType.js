/*
all sizes should be derived from variables.less
@maxLaptopWidth: 1280px;
@maxTabletWidth: 991px;
@maxPhoneWidth: 767px;
 */

import { deviceType } from "src/consts/deviceType";

export function isMobile() {
	return window.matchMedia("(max-width: 767px)").matches;
}

export function isTablet() {
	return !isMobile() && window.matchMedia("(max-width: 991px)").matches;
}

export function isLaptop() {
	return !isTablet() && window.matchMedia("(max-width: 1280px)").matches;
}

export function getDeviceType() {
	if(isMobile()) {
		return deviceType.mobile;
	}

	if(isTablet()) {
		return deviceType.tablet;
	}

	return deviceType.laptop;
}