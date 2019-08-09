import React, { Component } from "react";
import PropTypes from "prop-types";
import Button from "@skbkontur/react-ui/Button";
import LeftIcon from '@skbkontur/react-icons/ArrowChevron2Left';
import styles from './NavigationHeader.less';

class NavigationHeader extends Component {
	render() {
		const { createRef } = this.props;
		return (
			<header ref={ (ref) => createRef(ref) } className={ styles.root }>
				{ this.renderBreadcrumb() }
				{ this.renderTitle() }
			</header>
		); // TODO: Еще ссылка на "Ведомость модуля" у тех, у кого права есть
	}

	renderBreadcrumb() {
		const { courseName, onCourseClick } = this.props;


		return (
			<nav className={ styles.breadcrumbs }>
				<Button
					use="link"
					icon={ <LeftIcon/> }
					onClick={ onCourseClick }>{ courseName }</Button>
			</nav>
		);
	}

	renderTitle() {
		const { title } = this.props;

		return <h2 className={ styles.h2 } title={ title }>{ title }</h2>;
	}
}

NavigationHeader.propTypes = {
	title: PropTypes.string.isRequired,
	courseName: PropTypes.string,
	onCourseClick: PropTypes.func,
};

export default NavigationHeader
