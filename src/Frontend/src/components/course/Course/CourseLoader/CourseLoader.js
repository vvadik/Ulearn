import React from "react";

import { Loader } from "@skbkontur/react-ui";

import { connect } from "react-redux";
import { setSlideReady } from "src/actions/slides";
import PropTypes from "prop-types";

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

		if(props.isSlideLoader) {
			props.setSlideReady(false);
		}
	}

	componentWillUnmount() {
		const { timeoutAwaited } = this.state;
		if(!timeoutAwaited) {
			clearTimeout(this.timeout);
		}

		if(this.props.isSlideLoader) {
			setTimeout(() => this.props.setSlideReady(true), 100);
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

CourseLoader.propTypes = {
	isSlideLoader: PropTypes.bool,
	setSlideReady: PropTypes.func,
};

CourseLoader.defaultProps = {
	isSlideLoader: true,
};

const mapStateToProps = (state) => {
	return {};
};

const mapDispatchToProps = (dispatch) => ({
	setSlideReady: (courseId) => dispatch(setSlideReady(courseId)),
});


export default connect(mapStateToProps, mapDispatchToProps)(CourseLoader);
