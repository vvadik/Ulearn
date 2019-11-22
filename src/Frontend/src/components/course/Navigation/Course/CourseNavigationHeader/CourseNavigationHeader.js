import React, { Component } from "react";
import PropTypes from "prop-types";

import Link from '@skbkontur/react-ui/Link';
import LeftIcon from '@skbkontur/react-icons/ArrowChevron2Left';

import { groupAsStudentType, progressType } from './../../types';

import LinksToGroupsStatements from "../../LinksToGroupsStatements/LinksToGroupsStatements";

import styles from './CourseNavigationHeader.less';
import ProgressBar from "../../ProgressBar";

class CourseNavigationHeader extends Component {
	render() {
		const { title, description, groupsAsStudent } = this.props;
		return (
			<header className={ styles.root }>
				{ this.renderBreadcrumb() }

				<h1 className={ styles.h1 } title={ title }>{ title }</h1>

				{ description && <p className={ styles.description }>{ description }</p> }
				{ this.renderProgress() }

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

	renderProgress() {
		const { courseProgress } = this.props;
		const percentage = courseProgress.current / courseProgress.max;
		if (courseProgress) {
			return (
				<div className={ styles.progressBarWrapper }
					 title={ `${ courseProgress.current } из ${ courseProgress.max }` }>
					<ProgressBar value={ percentage } color={ percentage === 1 ? 'green' : 'blue' }/>
				</div>
			);
		}
	}
}

CourseNavigationHeader.propTypes = {
	title: PropTypes.string.isRequired,
	description: PropTypes.string,
	courseProgress: PropTypes.shape(progressType),
	groupsAsStudent: PropTypes.arrayOf(PropTypes.shape(groupAsStudentType)),
};

export default CourseNavigationHeader
