import React from "react";

import { Link, } from "ui";
import { ArrowChevron2Left } from 'icons';

import LinksToGroupsStatements from "../../LinksToGroupsStatements";
import ProgressBar from "../../ProgressBar";

import { GroupAsStudentInfo } from "src/models/groups";
import { Progress } from "../../types";


import styles from './NavigationHeader.less';

interface Props {
	title: string;
	courseName: string;
	progress: Progress;
	groupsAsStudent: GroupAsStudentInfo[];

	createRef: React.RefObject<HTMLElement>;

	onClick: () => void;
}

function NavigationHeader({
	createRef,
	groupsAsStudent,
	courseName,
	onClick,
	title,
	progress,
}: Props): React.ReactElement {
	return (
		<React.Fragment>
			{ renderBreadcrumb() }
			<header ref={ createRef } className={ styles.root }>
				{ renderTitle() }
				{ renderProgress(progress || { max: 0, current: 0 }) }
				{ groupsAsStudent && groupsAsStudent.length > 0 &&
				<LinksToGroupsStatements groupsAsStudent={ groupsAsStudent }/> }
			</header>
		</React.Fragment>
	);

	function renderBreadcrumb() {
		return (
			<nav className={ styles.breadcrumbs }>
				<Link
					icon={ <ArrowChevron2Left/> }
					onClick={ onClick }>
						<span className={ styles.breadcrumbsText }>
							{ courseName }
						</span>
				</Link>
			</nav>
		);
	}

	function renderTitle() {
		return <h2 className={ styles.h2 } title={ title }>{ title }</h2>;
	}

	function renderProgress(progress: Progress) {
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

export default NavigationHeader;
