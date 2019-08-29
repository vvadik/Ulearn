import React, { Component } from "react";
import PropTypes from "prop-types";

import Link from '@skbkontur/react-ui/Link';
import LeftIcon from '@skbkontur/react-icons/ArrowChevron2Left';

import { groupAsStudentType } from './../../types';

import renderLinksToGroupsStatements from "../../renderLinksToGroupsStatements";

import styles from './CourseNavigationHeader.less';

class CourseNavigationHeader extends Component {
	render() {
		const { title, description, groupsAsStudent } = this.props;
		return (
			<header className={ styles.root }>
				{ this.renderBreadcrumb() }

				<h1 className={ styles.h1 } title={ title }>{ title }</h1>

				{ description && <p className={ styles.description }>{ description }</p> }

				{ renderLinksToGroupsStatements(groupsAsStudent) }
			</header>
		);
	}

	renderBreadcrumb() {
		return (
			<nav className={ styles.breadcrumbs }>
				<Link
					icon={ <LeftIcon/> }
					href={ '/' }>
					Все курсы
				</Link>
			</nav>
		);
	}
}

CourseNavigationHeader.propTypes = {
	title: PropTypes.string.isRequired,
	description: PropTypes.string,
	groupsAsStudent: PropTypes.arrayOf(PropTypes.shape(groupAsStudentType)),
};

export default CourseNavigationHeader
