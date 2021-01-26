import React, { Component } from "react";
import PropTypes from 'prop-types';

import { Link } from "react-router-dom";
import { flashcards, constructPathToSlide } from 'src/consts/routes';

import classnames from 'classnames';

import styles from './Flashcards.less';


class Flashcards extends Component {
	render() {
		const { courseId, isActive, toggleNavigation } = this.props;
		return (
			<Link to={ constructPathToSlide(courseId, flashcards) }
				  className={ classnames(styles.root, { [styles.isActive]: isActive }) }
				  onClick={ toggleNavigation }
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

export default Flashcards;

