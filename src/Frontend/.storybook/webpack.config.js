const path = require("path");

module.exports = ({ config, mode }) => {

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

	return config
};