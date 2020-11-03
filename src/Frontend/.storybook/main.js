const path = require("path");
const base = require('../config/webpack.config.base');
const merge = require('webpack-merge');

module.exports = {
	stories: ['../src/**/**.story.@(js|jsx)'],
	addons: [
		'@storybook/addon-actions/register',
		'@storybook/addon-viewport/register',
	],
	webpackFinal: async (config, { configType }) => {
		config = merge(base, config);

		config.module.rules.push(
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
				],
				include: path.resolve(__dirname, '../src/')
			},
		);

		return config;
	},
};
