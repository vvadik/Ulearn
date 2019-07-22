import React, {Component} from "react";
import PropTypes from 'prop-types';
import styles from './Flashcards.less';
import {Link} from "react-router-dom";
import {flashcards, constructPathToSlide} from '../../../../../consts/routes';
import classnames from 'classnames';


class Flashcards extends Component {
	render() {
		const {courseId, isActive} = this.props;
		return (
			<Link to={constructPathToSlide(courseId, flashcards)}
				  className={classnames(styles.root, {[styles.isActive]: isActive})}>
				<h5 className={styles.header}>Карточки</h5>
				<span className={styles.text}>Вопросы для самопроверки</span>
			</Link>
		);
	}
}

Flashcards.propTypes = {
	isActive: PropTypes.bool,
	courseId: PropTypes.string,
};

export default Flashcards
