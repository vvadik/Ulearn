import React from "react";

import { Loader } from "@skbkontur/react-ui";

import styles from "./CourseLoader.less";

const showLoaderTimout = 1000;

class CourseLoader extends React.Component {
	constructor(props) {
		super(props);
		this.state = {
			timeoutAwaited: false,
		}

		this.timeout = setTimeout(() => {
			this.setState({
				timeoutAwaited: true,
			})
		}, showLoaderTimout);
	}

	componentWillUnmount() {
		const { timeoutAwaited } = this.state;
		if(!timeoutAwaited) {
			clearTimeout(this.timeout);
		}
	}

	render() {
		const { timeoutAwaited } = this.state;

		return (
			timeoutAwaited
				? <Loader className={ styles.loaderWhileLoading } type={ "big" } active={ true }/>
				: <div className={ styles.loaderWhileLoading }/>
		);
	}

}

export default CourseLoader;
