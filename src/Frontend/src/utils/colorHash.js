export default function colorHash(str) {
		let hash = 0;
		for (let i = 0; i < str.length; i++) {
			hash = str.charCodeAt(i) + ((hash << 5) - hash);
		}
		return `hsla(${hash % 360}, 100%, 50%, 0.5)`;
	}
// export default function colorHash(str) {
	// let sum = 0;
	//
	// for(let i in str) {
	// 	sum += str.charCodeAt(i);
	// }
	//
	// let r, g, b;
	// r = ~~(('0.'+Math.sin(sum+1).toString().substr(6))*256);
	// g = ~~(('0.'+Math.sin(sum+2).toString().substr(6))*256);
	// b = ~~(('0.'+Math.sin(sum+3).toString().substr(6))*256);
	//
	// let rgb = "rgb("+r+", "+g+", "+b+")";
	//
	// let hex = "#";
	// hex += ("00" + r.toString(16)).substr(-2,2).toUpperCase();
	// hex += ("00" + g.toString(16)).substr(-2,2).toUpperCase();
	// hex += ("00" + b.toString(16)).substr(-2,2).toUpperCase();
	//
	// return rgb;
// }

