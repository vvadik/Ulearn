'use strict';

require('es6-promise').polyfill();
require('promise.prototype.finally');

// const Promise = require('es6-promise');
const Promise = require('es6-promise').Promise;

if (typeof Promise === 'undefined') {
  // Rejection tracking prevents a common issue where React gets into an
  // inconsistent state due to an error, but it gets swallowed by a Promise,
  // and the user has no idea what causes React's erratic future behavior.
  require('promise/lib/rejection-tracking').enable();
  window.Promise = require('promise/lib/es6-extensions.js');
}

if (!('Promise' in window)) {
	window.Promise = Promise;
} else if (!('finally' in window.Promise.prototype)) {
	window.Promise.prototype.finally = Promise.prototype.finally;
}

// fetch() polyfill for making API calls.
require('whatwg-fetch');

// Object.assign() is commonly used with React.
// It will use the native implementation if it's present and isn't buggy.
Object.assign = require('object-assign');

// In tests, polyfill requestAnimationFrame since jsdom doesn't provide it yet.
// We don't polyfill it in the browser--this is user's responsibility.
if (process.env.NODE_ENV === 'test') {
  require('raf').polyfill(global);
}

// All previous code are from create-react-app. Following code adds another polyfills needed for ulearn
Array.from = require('array-from');

require('url-polyfill');

require('polyfill-array-includes');