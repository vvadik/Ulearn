import Raven from 'raven-js';

Raven
	.config('https://62e9c6b9ae6a47399a2b79600f1cacc5@sentry.skbkontur.ru/781')
	.install();

const consoleError = console.error;
console.error = function(firstParam) {
	const response = consoleError.apply(console, arguments);
	Raven.captureException(firstParam, { level: 'error' });
	return response;
};
