import React, { Component } from "react";
import PropTypes from "prop-types";
import Link from '@skbkontur/react-ui/Link';
import LeftIcon from '@skbkontur/react-icons/ArrowChevron2Left';
import styles from './NavigationHeader.less';

class NavigationHeader extends Component {
	render () {
		return (
			<header className={ styles.root }>
				{ this.renderBreadcrumb() }
				{ this.renderTitle() }
			</header>
		); // TODO: Еще ссылка на "Ведомость модуля" у тех, у кого права есть
	}

	renderBreadcrumb() {
		const { courseName, courseUrl } = this.props;


		return (
			<nav className={ styles.breadcrumbs }>
				<Link
				  	icon={ <LeftIcon /> }
				  	href={ courseUrl }>{ courseName }</Link>
			</nav>
		);
	}

	renderTitle() {
		const { title } = this.props;

		return <h2 className={ styles.h2 }>{ title }</h2>;
	}
}

NavigationHeader.propTypes ={
	title: PropTypes.string.isRequired,
	courseName: PropTypes.string,
	courseUrl: PropTypes.string,
};

export default NavigationHeader
