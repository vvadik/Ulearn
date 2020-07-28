module.exports = function (api) {
	api.cache(true);

	const presets = [
		["@babel/preset-env",
			{
				modules: false
			}
		],
		"@babel/preset-react",
	];

	const plugins= [
		"@babel/plugin-proposal-class-properties",
		"@babel/plugin-proposal-object-rest-spread",
		"@babel/plugin-transform-arrow-functions",
	];

	return {
		presets,
		plugins
	}
};
