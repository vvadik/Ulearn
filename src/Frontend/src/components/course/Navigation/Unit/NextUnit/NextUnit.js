import React, { Component } from "react";
import PropTypes from 'prop-types';
import styles from './NextUnit.less';


class NextUnit extends Component {
	render () {
		const { title } = this.props.unit;
		return (
			<button className={ styles.root }>
				<h5 className={ styles.header }>Следующий модуль</h5>
				{ title }
			</button>
		);
	}


}

NextUnit.propTypes ={
	unit: PropTypes.object, // TODO описать
};

export default NextUnit
