export default function colorHash(str) {
	let sum = 0;

	for (let i in str) {
		sum += str.charCodeAt(i);
	}

	let r, g, b;
	r = ~~(('0.' + Math.sin(sum + 1).toString().substr(6)) * 256);
	g = ~~(('0.' + Math.sin(sum + 2).toString().substr(6)) * 256);
	b = ~~(('0.' + Math.sin(sum + 3).toString().substr(6)) * 256);

	return "rgb(" + r + ", " + g + ", " + b + ")";
}

