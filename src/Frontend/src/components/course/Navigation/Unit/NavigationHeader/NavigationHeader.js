import React, { Component } from "react";
import PropTypes from "prop-types";

import { Link, } from "ui";
import { ArrowChevron2Left } from 'icons';

import { groupAsStudentType, progressType } from "../../types";
import LinksToGroupsStatements from "../../LinksToGroupsStatements/LinksToGroupsStatements";

import styles from './NavigationHeader.less';
import ProgressBar from "../../ProgressBar";

class NavigationHeader extends Component {
	render() {
		const { createRef, groupsAsStudent, } = this.props;
		return (
			<React.Fragment>
				{ this.renderBreadcrumb() }
				<header ref={ (ref) => createRef(ref) } className={ styles.root }>
					{ this.renderTitle() }
					{ this.renderProgress() }
					{ groupsAsStudent.length > 0 && <LinksToGroupsStatements groupsAsStudent={ groupsAsStudent }/> }
				</header>
			</React.Fragment>
		);
	}

	renderBreadcrumb() {
		const { courseName, onCourseClick } = this.props;

		return (
			<nav className={ styles.breadcrumbs }>
				<Link
					icon={ <ArrowChevron2Left/> }
					onClick={ onCourseClick }>
						<span className={ styles.breadcrumbsText }>
							{ courseName }
						</span>
				</Link>
			</nav>
		);
	}

	renderTitle() {
		const { title } = this.props;

		return <h2 className={ styles.h2 } title={ title }>{ title }</h2>;
	}

	renderProgress() {
		const { progress } = this.props;
		const percentage = progress.current / progress.max;

		if(percentage > 0) {
			return (
				<div className={ styles.progressBarWrapper } title={ `${ progress.current } из ${ progress.max }` }>
					<ProgressBar value={ percentage } color={ percentage >= 1 ? 'green' : 'blue' }/>
				</div>
			);
		}
	}
}

NavigationHeader.propTypes = {
	title: PropTypes.string.isRequired,
	courseName: PropTypes.string,
	progress: PropTypes.shape(progressType),
	groupsAsStudent: PropTypes.arrayOf(PropTypes.shape(groupAsStudentType)),
	onCourseClick: PropTypes.func,
};

export default NavigationHeader
