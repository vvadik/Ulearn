import React, { Component } from "react";
import PropTypes from 'prop-types';
import { connect } from "react-redux";

import { Link } from "react-router-dom";
import { flashcards, constructPathToSlide } from '../../../../../consts/routes';

import classnames from 'classnames';
import { toggleNavigation } from "../../../../../actions/navigation";

import styles from './Flashcards.less';


class Flashcards extends Component {
	render() {
		const { courseId, isActive } = this.props;
		return (
			<Link to={ constructPathToSlide(courseId, flashcards) }
				  className={ classnames(styles.root, { [styles.isActive]: isActive }) }
				  onClick={ this.props.toggleNavigation }
			>
				<h5 className={ styles.header }>Карточки</h5>
				<span className={ styles.text }>Вопросы для самопроверки</span>
			</Link>
		);
	}
}

Flashcards.propTypes = {
	isActive: PropTypes.bool,
	courseId: PropTypes.string,

	toggleNavigation: PropTypes.func,
};

const mapStateToProps = (state) => {
	return {};
};

const mapDispatchToProps = (dispatch) => ({
	toggleNavigation: () => dispatch(toggleNavigation()),
});

export default connect(mapStateToProps, mapDispatchToProps)(Flashcards);

