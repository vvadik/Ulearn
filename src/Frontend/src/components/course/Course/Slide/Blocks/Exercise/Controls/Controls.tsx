import React from "react";

import { isMobile, isTablet, } from "src/utils/getDeviceType.js";

import { ThemeContext } from "ui";
import SubmitButton from "./SubmitButton";
import ShowHintButton from "./ShowHintButton";
import OutputButton from "./OutputButton";
import ResetButton from "./ResetButton";
import StatisticsHint from "./StatisticsHint";
import AcceptedSolutionsButton from "./AcceptedSolutionsButton";
import { darkFlat } from "src/uiTheme.js";

import styles from './Controls.less';

interface Props {
	children: [
			JSX.Element & typeof SubmitButton,
			JSX.Element & typeof ShowHintButton,
			JSX.Element & typeof ResetButton,
			JSX.Element & typeof OutputButton,
			JSX.Element & typeof StatisticsHint,
			JSX.Element & typeof AcceptedSolutionsButton,
	]
}

interface State {
	resizeTimeout?: NodeJS.Timeout,
	showControlsText: boolean,
}

const isControlsTextSuits = (): boolean => !isMobile() && !isTablet();

class Controls extends React.Component<Props, State> {
	state = {
		resizeTimeout: undefined,
		showControlsText: isControlsTextSuits(),
	};

	componentDidMount = (): void => {
		window.addEventListener("resize", this.onWindowResize);
	};

	componentWillUnmount = (): void => {
		window.removeEventListener("resize", this.onWindowResize);
	};

	onWindowResize = (): void => {
		const { resizeTimeout, } = this.state;

		const throttleTimeout = 66;

		//resize event can be called rapidly, to prevent performance issue, we throttling event handler
		if(!resizeTimeout) {
			this.setState({
				resizeTimeout: setTimeout(() => {
					this.setState({
						resizeTimeout: undefined,
						showControlsText: isControlsTextSuits(),
					});
				}, throttleTimeout),
			});
		}
	};

	render = (): React.ReactNode => {
		const { showControlsText, } = this.state;
		const [submit, hint, reset, output, statistics, acceptedSolutions] = this.props.children;

		return (
			<div className={ styles.exerciseControlsContainer }>
				<ThemeContext.Provider value={ darkFlat }>
					{ submit }
					{ hint && React.cloneElement(hint, { showControlsText }) }
					{ reset && React.cloneElement(reset, { showControlsText }) }
					{ output && React.cloneElement(output, { showControlsText }) }
					{ statistics }
				</ThemeContext.Provider>
				{ acceptedSolutions && React.cloneElement(acceptedSolutions, { showControlsText }) }
			</div>
		);
	};
}

export default Controls;
export {
	SubmitButton,
	ShowHintButton,
	OutputButton,
	ResetButton,
	StatisticsHint,
	AcceptedSolutionsButton,
};
