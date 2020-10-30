const path = require("path");
const merge = require('webpack-merge');
const base = require('../config/webpack.config.base');

module.exports = ({ config, mode }) => {

	config = merge(config, base, {
		module: {
			rules: [{
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
				include: path.resolve(__dirname, '../src/'),
			}]
		}
	});

	return config
};