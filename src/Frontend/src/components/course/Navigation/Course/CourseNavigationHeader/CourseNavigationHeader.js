import React, { Component } from "react";
import PropTypes from "prop-types";

import Link from '@skbkontur/react-ui/Link';
import LeftIcon from '@skbkontur/react-icons/ArrowChevron2Left';

import { groupAsStudentType } from './../../types';

import LinksToGroupsStatements from "../../LinksToGroupsStatements/LinksToGroupsStatements";

import styles from './CourseNavigationHeader.less';
import ProgressBar from "../../ProgressBar";

class CourseNavigationHeader extends Component {
	render() {
		const { title, description, groupsAsStudent, courseProgress } = this.props;
		return (
			<header className={ styles.root }>
				{ this.renderBreadcrumb() }

				<h1 className={ styles.h1 } title={ title }>{ title }</h1>

				{ description && <p className={ styles.description }>{ description }</p> }
				{ courseProgress && <ProgressBar value={ courseProgress } color={ courseProgress >= 1 ? 'green' : 'blue' }/> }

				{ groupsAsStudent.length > 0 && <LinksToGroupsStatements groupsAsStudent={ groupsAsStudent }/> }
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
	courseProgress: PropTypes.number,
	groupsAsStudent: PropTypes.arrayOf(PropTypes.shape(groupAsStudentType)),
};

export default CourseNavigationHeader
