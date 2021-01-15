import React, { ReactElement } from "react";

import { isMobile, isTablet, } from "src/utils/getDeviceType";

import { ThemeContext } from "ui";
import SubmitButton from "./SubmitButton";
import ShowHintButton from "./ShowHintButton";
import OutputButton from "./OutputButton";
import ResetButton from "./ResetButton";
import StatisticsHint from "./StatisticsHint";
import AcceptedSolutionsButton from "./AcceptedSolutionsButton";
import { darkFlat } from "src/uiTheme";

import ShowControlsTextContext from "./ShowControlsTextContext";

import styles from './Controls.less';

interface Props {
	children: React.ReactNode[],
}

interface State {
	resizeTimeout?: NodeJS.Timeout,
	showControlsText: boolean,
}

const isControlsTextSuits = (): boolean => !isMobile() && !isTablet();

class Controls extends React.Component<Props, State> {
	public static SubmitButton = SubmitButton;
	public static ShowHintButton = ShowHintButton;
	public static ResetButton = ResetButton;
	public static OutputButton = OutputButton;
	public static StatisticsHint = StatisticsHint;
	public static AcceptedSolutionsButton = AcceptedSolutionsButton;

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
		const [submit, hint, reset, output, statistics, acceptedSolutions] = this.parseChildren();

		return (
			<div className={ styles.exerciseControlsContainer }>
				<ShowControlsTextContext.Provider value={ showControlsText }>
					{ hint }
					<ThemeContext.Provider value={ darkFlat }>
						{ submit }
						{ reset }
						{ output }
						{ statistics }
					</ThemeContext.Provider>
					{ acceptedSolutions }
				</ShowControlsTextContext.Provider>
			</div>
		);
	};

	parseChildren = ()
		: [typeof SubmitButton?,
		typeof ShowHintButton?,
		typeof ResetButton?,
		typeof OutputButton?,
		typeof StatisticsHint?,
		typeof AcceptedSolutionsButton?
	] => {
		const childArray = this.props.children;
		let submit: typeof SubmitButton | undefined = undefined;
		let hint: typeof ShowHintButton | undefined = undefined;
		let reset: typeof ResetButton | undefined = undefined;
		let output: typeof OutputButton | undefined = undefined;
		let stat: typeof StatisticsHint | undefined = undefined;
		let solutions: typeof AcceptedSolutionsButton | undefined = undefined;

		for (const child of childArray) {
			const reactElement = child as ReactElement;
			if(reactElement) {
				switch (reactElement.type) {
					case SubmitButton:
						submit = child as typeof SubmitButton;
						break;
					case ShowHintButton:
						hint = child as typeof ShowHintButton;
						break;
					case ResetButton:
						reset = child as typeof ResetButton;
						break;
					case OutputButton:
						output = child as typeof OutputButton;
						break;
					case StatisticsHint:
						stat = child as typeof StatisticsHint;
						break;
					case AcceptedSolutionsButton:
						solutions = child as typeof AcceptedSolutionsButton;
						break;
				}
			}
		}
		return [submit, hint, reset, output, stat, solutions];
	};
}

export default Controls;
