import React from "react";
import { Provider } from "react-redux";

import configureStore from "src/configureStore";
import theme from "src/uiTheme";

import { ThemeContext } from "@skbkontur/react-ui/index";

const viewports = {
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

const reduxStore = configureStore();

export const parameters = {
	viewport: {
		viewports: viewports,
		defaultViewport: 'desktop',
	},
}

export const decorators = [
	(Story) => (
		<Provider store={ reduxStore }>
			<ThemeContext.Provider value={ theme }>
				<Story/>
			</ThemeContext.Provider>
		</Provider>
	),
];
