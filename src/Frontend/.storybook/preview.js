const customViewports = {
	desktop: {
		name: 'desktop',
		styles: {
			width: '1280px',
			height: '1024px',
		},
	},
	tablet: {
		name: 'tablet',
		styles: {
			width: '800px',
			height: '600px',
		},
	},
	mobile: {
		name: 'mobile',
		styles: {
			width: '320px',
			height: '240px',
		},
	},
};

export const parameters = {
	viewport: {
		viewports: customViewports,
		defaultViewport: 'desktop',
	},
}