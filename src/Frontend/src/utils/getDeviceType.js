/*
all sizes should be derived from variables.less
@minDesktopWidth: 1600px;
@minLaptopWidth: 1280px;
@minTabletWidth: 800px;
 */

import { DeviceType } from "src/consts/deviceType";

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
		return DeviceType.mobile;
	}

	if(isTablet()) {
		return DeviceType.tablet;
	}

	if(isLaptop()) {
		return DeviceType.laptop;
	}

	return DeviceType.desktop;
}
