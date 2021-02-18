import React from "react";

import { Link } from 'ui';
import { ArrowChevron2Left } from 'icons';
import ProgressBar from "../../ProgressBar";
import LinksToGroupsStatements from "../../LinksToGroupsStatements";

import { Progress } from '../../types';
import { GroupAsStudentInfo } from "src/models/groups";

import styles from './CourseNavigationHeader.less';

interface Props {
	title: string;
	description?: string;
	courseProgress: Progress;
	groupsAsStudent: GroupAsStudentInfo[];
}

function CourseNavigationHeader({ title, description, groupsAsStudent, courseProgress, }: Props): React.ReactElement {
	return (
		<header className={ styles.root }>
			{ renderBreadcrumb() }

			<h1 className={ styles.h1 } title={ title }>{ title }</h1>

			{ description && <p className={ styles.description }>{ description }</p> }
			{ renderProgress() }

			{ groupsAsStudent.length > 0 && <LinksToGroupsStatements groupsAsStudent={ groupsAsStudent }/> }
		</header>
	);

	function renderBreadcrumb() {
		return (
			<nav className={ styles.breadcrumbs }>
				<Link
					icon={ <ArrowChevron2Left/> }
					href={ '/' }>
					Все курсы
				</Link>
			</nav>
		);
	}

	function renderProgress() {
		const percentage = courseProgress.current / courseProgress.max;
		if(courseProgress) {
			return (
				<div className={ styles.progressBarWrapper }
					 title={ `${ courseProgress.current } из ${ courseProgress.max }` }>
					<ProgressBar value={ percentage } color={ percentage >= 1 ? 'green' : 'blue' }/>
				</div>
			);
		}
	}
}

export default CourseNavigationHeader;
