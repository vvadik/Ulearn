const path = require('path');
const paths = require('./paths');

module.exports = {
	resolve: {
		alias: {
			ui: path.resolve(paths.appNodeModules, '@skbkontur/react-ui'),
			icons: path.resolve(paths.appNodeModules, '@skbkontur/react-icons/icons'),
			src: path.resolve(paths.appSrc),
		}
	},
	// Some libraries import Node modules but don't use them in the browser.
	// Tell Webpack to provide empty mocks for them so importing them works.
	node: {
		dgram: 'empty',
		fs: 'empty',
		net: 'empty',
		tls: 'empty',
		child_process: 'empty',
	},
};
