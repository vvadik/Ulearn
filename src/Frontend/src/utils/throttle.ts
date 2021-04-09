export default function throttle(fn: (...args: any[]) => void, wait = 16.6): (...args: any[]) => void { //16.6 is 60 repeats time per sec
	let time = Date.now();
	return function (...args: any[]) {
		if((time + wait - Date.now()) < 0) {
			fn(...args);
			time = Date.now();
		}
	};
}
