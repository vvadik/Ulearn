import React, { Component } from "react";
import PropTypes from "prop-types";
import classnames from 'classnames';
import styles from "./ProgressBar.less";

class ProgressBar extends Component {
	render() {
		const { value, small, color, active, } = this.props;
		return (
			<div className={ classnames(styles.wrapper, { [styles.small]: small }, { [styles.active]: active }) }>
				<div
					className={ classnames(styles.value, { [styles.blue]: color === 'blue' }) }
					style={ { width: `${ value * 100 }%` } }/>
			</div>
		);
	}
}

ProgressBar.propTypes = {
	value: PropTypes.number.isRequired,
	small: PropTypes.bool,
	color: PropTypes.oneOf(['green', 'blue']),
	active: PropTypes.bool,
};

ProgressBar.defaultProps = {
	color: 'green'
};

export default ProgressBar
