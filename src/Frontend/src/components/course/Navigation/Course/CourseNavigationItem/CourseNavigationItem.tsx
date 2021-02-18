import React from "react";
import classnames from "classnames";

import { isMobile, isTablet } from "src/utils/getDeviceType";

import ProgressBar from "../../ProgressBar";
import { Calendar, EyeClosed, } from "icons";
import { Hint, } from "ui";

import { getDateDDMMYY } from "src/utils/momentUtils";

import { CourseMenuItem, Progress } from "../../types";

import styles from "./CourseNavigationItem.less";

function CourseNavigationItem({
	title,
	isActive,
	isNotPublished,
	publicationDate,
	progress,
	onClick,
	id,
}: CourseMenuItem): React.ReactElement {
	const classes = classnames(
		styles.itemLink,
		{ [styles.active]: isActive },
	);

	return (
		<li className={ styles.root } onClick={ clickHandle }>
			<div className={ classes }>
					<span className={ styles.text }>
						{ isNotPublished &&
						<span className={ styles.isNotPublishedIcon } onClick={ hintClickHandle }>
								{ publicationDate
									?
									<Hint text={ `Этот модуль будет опубликован ${ getDateDDMMYY(publicationDate) }` }>
										<Calendar/>
									</Hint>
									:
									<Hint text={ `Этот модуль не опубликован` }>
										<EyeClosed/>
									</Hint>
								}
						</span>
						}
						{ title }
					</span>
				{ progress && renderProgress(progress) }
			</div>
		</li>
	);

	function renderProgress(progress: Progress) {
		const percentage = progress.current / progress.max;

		if(percentage > 0) {
			return (
				<span className={ styles.progressWrapper } title={ `${ progress.current } из ${ progress.max }` }>
					<ProgressBar
						value={ percentage }
						small
						color={ percentage >= 1 ? 'green' : 'blue' }
						active={ isActive }
					/>
				</span>
			);
		}
	}

	function clickHandle() {
		if(onClick) {
			onClick(id);
		}
	}

	function hintClickHandle(e: React.MouseEvent) {
		if(isMobile() || isTablet()) {
			e.stopPropagation();
		}
	}
}

export default CourseNavigationItem;

