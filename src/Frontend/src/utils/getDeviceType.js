/*
all sizes should be derived from variables.less
@minDesktopWidth: 1600px;
@minLaptopWidth: 1280px;
@minTabletWidth: 800px;
 */

import { deviceType } from "src/consts/deviceType";

export function isMobile() {
	return window.matchMedia("(max-width: 800px)").matches;
}

export function isTablet() {
	return !isMobile() && window.matchMedia("(max-width: 1280px)").matches;
}

export function isLaptop() {
	return !isTablet() && window.matchMedia("(max-width: 1600px)").matches;
}

export function isDesktop() {
	return window.matchMedia("(min-width: 1600px)").matches;
}

export function getDeviceType() {
	if(isMobile()) {
		return deviceType.mobile;
	}

	if(isTablet()) {
		return deviceType.tablet;
	}

	if(isLaptop()) {
		return deviceType.laptop;
	}

	return deviceType.desktop;
}