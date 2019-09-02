import React, { Component } from "react";
import PropTypes from 'prop-types';
import styles from './NextUnit.less';
import { Link } from "react-router-dom";
import Icon from "@skbkontur/react-icons";

class NextUnit extends Component {
	render() {
		const { toggleNavigation, unit } = this.props;
		const { title, slides } = unit;

		const slideId = slides[0].slug;

		return (
			<Link to={ slideId } className={ styles.root } onClick={ toggleNavigation }>
				<div className={ styles.wrapper }>
					<h5 className={ styles.header }>Следующий модуль</h5>
					<h4 className={ styles.title } title={ title }>{ title }</h4>
				</div>
				<Icon.ArrowChevronRight size={ 24 } className={ styles.arrow }/>
			</Link>
		);
	}
}

NextUnit.propTypes = {
	unit: PropTypes.shape({
		title: PropTypes.string,
		slug: PropTypes.string,
	}),
	isActive: PropTypes.bool,

	toggleNavigation: PropTypes.func,
};

export default NextUnit;
