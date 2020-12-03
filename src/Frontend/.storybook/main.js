const path = require("path");
const autoprefixer = require('autoprefixer');
const base = require('../config/webpack.config.base');
const merge = require('webpack-merge');

module.exports = {
	stories: ['../src/**/**.story.@(js|jsx|tsx)'],
	addons: [
		'@storybook/addon-essentials',
		'@storybook/addon-controls',
	],
	webpackFinal: async (config, { configType }) => {
		config = merge(base, config);

		config.module.rules.find(
			rule => rule.test.toString() === '/\\.css$/'
		).exclude = /\.module\.css$/;

		config.module.rules.push(
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
								localIdentName: '[name]__[local]--[hash:base64:5]',
							}
						},
					},
					'less-loader',
				],
				include: path.resolve(__dirname, '../src/')
			},
			{
				test: /\.module\.css$/,
				use: [
					'style-loader',
					'@teamsupercell/typings-for-css-modules-loader',
					{
						loader: 'css-loader',
						options: {
							modules: 'global',
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
		);

		return config;
	},
};
