import "core-js/stable";
import "regenerator-runtime/runtime";
import 'whatwg-fetch';

// In tests, polyfill requestAnimationFrame since jsdom doesn't provide it yet.
// We don't polyfill it in the browser--this is user's responsibility.
if (process.env.NODE_ENV === 'test') {
	require('raf').polyfill(global);
}

// Adding Element.remove() for DOM elements, suggested in the Mozilla documentation. https://developer.mozilla.org/en-US/docs/Web/API/ChildNode/remove
(function (arr) {
	arr.forEach(function (item) {
		if (item.hasOwnProperty('remove')) {
			return;
		}
		Object.defineProperty(item, 'remove', {
			configurable: true,
			enumerable: true,
			writable: true,
			value: function remove() {
				this.parentNode.removeChild(this);
			}
		});
	});
})([Element.prototype, CharacterData.prototype, DocumentType.prototype]);