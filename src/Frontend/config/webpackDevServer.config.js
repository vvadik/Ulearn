const errorOverlayMiddleware = require('react-dev-utils/errorOverlayMiddleware');
const noopServiceWorkerMiddleware = require('react-dev-utils/noopServiceWorkerMiddleware');
const ignoredFiles = require('react-dev-utils/ignoredFiles');
const config = require('./webpack.config.dev');
const paths = require('./paths');

module.exports = function (proxy, allowedHost, protocol, host) {
	return {
		disableHostCheck: !proxy || process.env.DANGEROUSLY_DISABLE_HOST_CHECK === 'true',
		compress: true,
		clientLogLevel: 'none',
		contentBase: paths.appPublic,
		watchContentBase: true,
		hot: true,
		transportMode: "ws",
		injectClient: false,
		quiet: true,
		watchOptions: {
			ignored: ignoredFiles(paths.appSrc),
		},
		https: protocol === 'https',
		host,
		overlay: false,
		proxy,
		historyApiFallback: {
			disableDotRule: true,
		},
		public: allowedHost,
		publicPath: config.output.publicPath,
		before(app) {
			app.use(errorOverlayMiddleware());
			app.use(noopServiceWorkerMiddleware(paths.serviceWorker));
		},
	};
};
