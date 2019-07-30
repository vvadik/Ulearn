import React, { Component } from "react";
import PropTypes from 'prop-types';
import { connect } from "react-redux";

import NavigationHeader from '../NavigationHeader';
import NavigationContent from '../NavigationContent';
import { menuItemType } from '../../types';
import NextUnit from "../NextUnit";

import { toggleNavigation } from "../../../../../actions/navigation";

import styles from './Navigation.less';

class Navigation extends Component {
	componentDidMount() {
		document.getElementsByTagName('body')[0]
			.style.overflow = 'hidden';
	}

	componentWillUnmount() {
		document.getElementsByTagName('body')[0]
			.style.overflow = 'auto';
	}

	render() {
		const { items, nextUnit } = this.props;

		return (
			<aside className={ styles.root }>
				<div className={ styles.overlay } onClick={ this.props.toggleNavigation }/>
				<div className={ styles.contentWrapper }>
					<NavigationHeader { ...this.props } />
					<NavigationContent items={ items }/>
					{ nextUnit && <NextUnit unit={ nextUnit }/> }
				</div>
			</aside>
		);
	}
}

Navigation.propTypes = {
	title: PropTypes.string.isRequired,
	items: PropTypes.arrayOf(PropTypes.shape(menuItemType)),
	nextUnit: PropTypes.object, // TODO: описать нормально

	courseName: PropTypes.string,
	onCourseClick: PropTypes.func,

	toggleNavigation: PropTypes.func,
};

const mapStateToProps = (state) => {
	return {};
};

const mapDispatchToProps = (dispatch) => ({
	toggleNavigation: () => dispatch(toggleNavigation()),
});

export default connect(mapStateToProps, mapDispatchToProps)(Navigation);
