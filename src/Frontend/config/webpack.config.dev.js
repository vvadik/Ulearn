const autoprefixer = require('autoprefixer');
const path = require('path');
const webpack = require('webpack');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const CaseSensitivePathsPlugin = require('case-sensitive-paths-webpack-plugin');
const InterpolateHtmlPlugin = require('react-dev-utils/InterpolateHtmlPlugin');
const WatchMissingNodeModulesPlugin = require('react-dev-utils/WatchMissingNodeModulesPlugin');
const eslintFormatter = require('react-dev-utils/eslintFormatter');
const { GenerateSW } = require('workbox-webpack-plugin');
const WebpackPwaManifest = require('webpack-pwa-manifest')
const getClientEnvironment = require('./env');
const paths = require('./paths');

const publicPath = '/';
const publicUrl = '';
const env = getClientEnvironment(publicUrl);

const base = require('./webpack.config.base');
const { merge } = require('webpack-merge');

module.exports = merge([base, {
	mode: 'development',
	devtool: 'eval-cheap-source-map',
	entry: {
		main: [
			paths.legacy,
			'react-dev-utils/webpackHotDevClient',
			paths.appIndexTsx,
		],
		oldBrowser: [
			paths.oldBrowserJs
		],
	},
	output: {
		filename: '[name].[hash:8].js',
		sourceMapFilename: '[name].[hash:8].map',
		chunkFilename: 'chunk_[id].[hash:8].js',
		publicPath: publicPath,
		devtoolModuleFilenameTemplate: info =>
			path.resolve(info.absoluteResourcePath).replace(/\\/g, '/'),
	},
	resolve: {
		extensions: ['.ts', '.tsx', '.js', '.json']
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
							name: paths.static.media + '/[name].[hash:8].[ext]',
						},
					},
					{
						test: /\.(js|jsx|mjs|ts|tsx)$/,
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
							'@teamsupercell/typings-for-css-modules-loader',
							{
								loader: 'css-loader',
								options: {
									modules: {
										mode: 'local',
										localIdentName: '[name]__[local]--[hash:5]',
									},
									importLoaders: 2,
								},
							},
							{
								loader: 'postcss-loader',
								options: {
									postcssOptions: {
										ident: 'postcss',
										plugins: [
											[
												"postcss-preset-env",
												{
													autoprefixer: { flexbox: 'no-2009' }
												},
											]
										],
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
							'@teamsupercell/typings-for-css-modules-loader',
							{
								loader: 'css-loader',
								options: {
									modules: {
										auto: (resourcePath) => !resourcePath.endsWith('.global.css'),
										mode: 'global',
									},
									importLoaders: 1,
								},
							},
							{
								loader: 'postcss-loader',
								options: {
									postcssOptions: {
										ident: 'postcss',
										plugins: [
											"postcss-preset-env",
											{
												autoprefixer: { flexbox: 'no-2009' }
											},
										]
									}
								},
							},
						],
					},
					{
						loader: 'file-loader',
						exclude: [/\.(js|jsx|mjs|ts|tsx)$/, /\.html$/, /\.json$/],
						options: {
							name: paths.static.media + '/[name].[hash:8].[ext]',
						},
					},
				],
			},
			// ** STOP ** Are you adding a new loader?
			// Make sure to add the new loader(s) before the "file" loader.
		],
	},
	plugins: [
		new HtmlWebpackPlugin({
			inject: true,
			template: paths.appHtml,
			favicon: paths.appPublic + '/favicon.ico',
			chunksSortMode: (chunk1, chunk2) => {
				if(chunk1 === 'oldBrowser') return -1;
				if(chunk2 === 'oldBrowser') return 1;
				return 0;
			},
		}),
		new InterpolateHtmlPlugin(HtmlWebpackPlugin, env.raw),
		new webpack.DefinePlugin(env.stringified),
		new webpack.ProvidePlugin({
			process: 'process/browser',
			$: 'jquery',
			jQuery: 'jquery',
			"window.$": 'jquery',
			"window.jQuery": 'jquery',
		}),
		new CaseSensitivePathsPlugin(),
		new WatchMissingNodeModulesPlugin(paths.appNodeModules),
		new webpack.IgnorePlugin({
			resourceRegExp: /^\.\/locale$/,
			contextRegExp: /moment$/,
		}),
		new WebpackPwaManifest({
			filename: "manifest.json",
			inject: true,
			fingerprints: true,
			start_url: '/',
			scope: '/',

			name: 'Ulearn.me',
			short_name: 'Ulearn',
			description: 'Интерактивные онлайн-курсы по программированию',
			background_color: '#ffffff',
			theme_color: "#000000",
			prefer_related_applications: true,
			related_applications: [],
			ios: true,

			icons: [
				{
					src: paths.appPublic + '/logo.png',
					sizes: [512, 256, 192, 128, 64, 32,],
					type: "image/png",
					purpose: "any maskable",
					destination: paths.static.media + '/icons',
					ios: true,
				},
				{
					src: paths.appPublic + '/favicon.ico',
					sizes: [16,],
					type: "image/x-icon",
					purpose: "any maskable",
					destination: paths.static.media + '/icons',
					ios: 'startup',
				}
			],
		}),
		new GenerateSW({
			swDest: 'sw',
			navigateFallbackAllowlist: [/^(?!\/__).*/],
			exclude: [/\.map$/, /asset-manifest\.json$/, /\.(?:png|jpg|jpeg|svg)$/],
			clientsClaim: true,
			skipWaiting: true,
			maximumFileSizeToCacheInBytes: 1024 * 1024 * 4,
			cleanupOutdatedCaches: true,
			// Define runtime caching rules.
			runtimeCaching: [{
				// Match any request that ends with .png, .jpg, .jpeg or .svg.
				urlPattern: /\.(?:png|jpg|jpeg|svg)$/,
				// Apply a cache-first strategy.
				handler: 'CacheFirst',
				options: {
					// Use a custom cache name.
					cacheName: 'images',
					// Only cache 10 images.
					expiration: {
						maxEntries: 10,
					},
				},
			}],
		}),
	],
	performance: {
		hints: false,
	},
}]);
