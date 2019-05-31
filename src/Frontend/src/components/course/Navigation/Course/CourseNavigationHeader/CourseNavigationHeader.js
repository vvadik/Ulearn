import React, { Component } from "react";
import PropTypes from "prop-types";
import Link from '@skbkontur/react-ui/Link';
import LeftIcon from '@skbkontur/react-icons/ArrowChevron2Left';
import styles from './CourseNavigationHeader.less';
import ProgressBar from '../../ProgressBar';

class CourseNavigationHeader extends Component {
	render () {
		const { title, description, progress } = this.props;
		return (
			<header className={ styles.root }>
				{ this.renderBreadcrumb() }

				<h1 className={ styles.h1 }>{ title }</h1>

				{ description && <p className={ styles.description }>{ description }</p> }

				<div className={ styles.progressBarWrapper }>
					<ProgressBar value={ progress } />
				</div>
			</header>
		);
	}

	renderBreadcrumb() {
		return (
			<nav className={ styles.breadcrumbs }>
				<Link
				  	icon={ <LeftIcon /> }
				  	href={ '/' }>
					Все курсы
				</Link>
			</nav>
		);
	}
}

CourseNavigationHeader.propTypes ={
	title: PropTypes.string.isRequired,
	progress: PropTypes.number,
	description: PropTypes.string,
};

export default CourseNavigationHeader
