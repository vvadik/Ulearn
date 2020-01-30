const autoprefixer = require('autoprefixer');
const path = require('path');
const webpack = require('webpack');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const CaseSensitivePathsPlugin = require('case-sensitive-paths-webpack-plugin');
const InterpolateHtmlPlugin = require('react-dev-utils/InterpolateHtmlPlugin');
const WatchMissingNodeModulesPlugin = require('react-dev-utils/WatchMissingNodeModulesPlugin');
const eslintFormatter = require('react-dev-utils/eslintFormatter');
const getClientEnvironment = require('./env');
const paths = require('./paths');

// Webpack uses `publicPath` to determine where the app is being served from.
// In development, we always serve from the root. This makes config easier.
const publicPath = '/';
// `publicUrl` is just like `publicPath`, but we will provide it to our app
// as %PUBLIC_URL% in `index.html` and `process.env.PUBLIC_URL` in JavaScript.
// Omit trailing slash as %PUBLIC_PATH%/xyz looks better than %PUBLIC_PATH%xyz.
const publicUrl = '';
// Get environment variables to inject into our app.
const env = getClientEnvironment(publicUrl);

let base = require('./webpack.config.base');
const merge = require('webpack-merge');

module.exports = merge(base,{
	mode: 'development',
	devtool: 'cheap-module-source-map',
	entry: {
		main: [
			'./config/polyfills',
			'./config/sentry',
			//'react-dev-utils/webpackHotDevClient',
			paths.appIndexJs,
		],
		oldBrowser: [
			paths.oldBrowserJs
		],
	},
	output: {
		pathinfo: true,
		filename: '[name].[hash:8].js',
		sourceMapFilename: '[name].[hash:8].map',
		chunkFilename: '[id].[hash:8].js',
		publicPath: publicPath,
		devtoolModuleFilenameTemplate: info =>
			path.resolve(info.absoluteResourcePath).replace(/\\/g, '/'),
	},
	module: {
		strictExportPresence: true,
		rules: [
			{
				test: /\.(js|jsx|mjs)$/,
				include: paths.appSrc,
				loader: 'eslint-loader',
				enforce: 'pre',
				options: {
					formatter: eslintFormatter,
					eslintPath: 'eslint',
				},
			},
			{
				oneOf: [
					{
						test: [/\.bmp$/, /\.gif$/, /\.jpe?g$/, /\.png$/],
						loader: 'url-loader',
						options: {
							limit: 10000,
							name: 'static/media/[name].[hash:8].[ext]',
						},
					},
					{
						test: /\.(js|jsx|mjs)$/,
						loader: 'babel-loader',
						include: paths.appSrc,
						options: {
							configFile: "./babel.config.js",
							cacheDirectory: true,
						},
					},
					{
						test: /\.less$/,
						use: [
							'style-loader',
							{
								loader: 'css-loader',
								options: {
									modules: {
										mode: 'local',
										localIdentName: '[name]__[local]--[hash:base64:5]',
									}
								},
							},
							'less-loader',
						]
					},
					{
						test: /\.css$/,
						use: [
							'style-loader',
							{
								loader: 'css-loader',
								options: {
									modules: true,
									importLoaders: 1,
								},
							},
							{
								loader: 'postcss-loader',
								options: {
									ident: 'postcss',
									plugins: () => [
										require('postcss-flexbugs-fixes'),
										autoprefixer({ flexbox: 'no-2009' }),
									],
								},
							},
						],
					},
					{
						exclude: [/\.(js|jsx|mjs)$/, /\.html$/, /\.json$/],
						loader: 'file-loader',
						options: {
							name: 'static/media/[name].[hash:8].[ext]',
						},
					},
				],
			},
			// ** STOP ** Are you adding a new loader?
			// Make sure to add the new loader(s) before the "file" loader.
		],
	},
	plugins: [
		// Makes some environment variables available in index.html.
		// The public URL is available as %PUBLIC_URL% in index.html, e.g.:
		// <link rel="shortcut icon" href="%PUBLIC_URL%/favicon.ico">
		// In development, this will be an empty string.
		new InterpolateHtmlPlugin(HtmlWebpackPlugin, env.raw),
		// Generates an `index.html` file with the <script> injected.
		new HtmlWebpackPlugin({
			inject: true,
			template: paths.appHtml,
			chunksSortMode: (chunk1, chunk2) => {
				/* oldBrowser.js should be the first bundle. For more complex cases see solution
				   at https://github.com/jantimon/html-webpack-plugin/issues/481#issuecomment-287370259*/
				if (chunk1 === 'oldBrowser') return -1;
				if (chunk2 === 'oldBrowser') return 1;
				return 0;
			},
		}),
		// Add module names to factory functions so they appear in browser profiler.
		// andgein: mode=development enabled this by default (https://webpack.js.org/concepts/mode/)
		// new webpack.NamedModulesPlugin(),

		// Makes some environment variables available to the JS code, for example:
		// if (process.env.NODE_ENV === 'development') { ... }. See `./env.js`.
		// andgein: mode=development enabled this by default (https://webpack.js.org/concepts/mode/)
		// new webpack.DefinePlugin(env.stringified),

		// This is necessary to emit hot updates (currently CSS only):
		new webpack.HotModuleReplacementPlugin(),
		// Watcher doesn't work well if you mistype casing in a path so we use
		// a plugin that prints an error when you attempt to do this.
		// See https://github.com/facebookincubator/create-react-app/issues/240
		new CaseSensitivePathsPlugin(),
		// If you require a missing module and then `npm install` it, you still have
		// to restart the development server for Webpack to discover it. This plugin
		// makes the discovery automatic so you don't have to restart.
		// See https://github.com/facebookincubator/create-react-app/issues/186
		new WatchMissingNodeModulesPlugin(paths.appNodeModules),
		// Moment.js is an extremely popular library that bundles large locale files
		// by default due to how Webpack interprets its code. This is a practical
		// solution that requires the user to opt into importing specific locales.
		// https://github.com/jmblog/how-to-optimize-momentjs-with-webpack
		// You can remove this if you don't use Moment.js:
		new webpack.IgnorePlugin(/^\.\/locale$/, /moment$/),
	],
	// Turn off performance hints during development because we don't do any
	// splitting or minification in interest of speed. These warnings become
	// cumbersome.
	performance: {
		hints: false,
	},
});
