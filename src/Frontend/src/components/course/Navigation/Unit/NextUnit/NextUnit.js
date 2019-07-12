import React, { Component } from "react";
import PropTypes from 'prop-types';
import classnames from 'classnames';
import styles from './NextUnit.less';


class NextUnit extends Component {
	render () {
		const { title } = this.props.unit;
		return (
			<button className={ styles.root }>
				{ title }
			</button>
		);
	}


}

NextUnit.propTypes ={
	unit: PropTypes.object, // TODO описать
};

export default NextUnit
