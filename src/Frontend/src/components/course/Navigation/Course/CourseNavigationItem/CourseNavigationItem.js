import React, { Component } from "react";

import ProgressBar from "../../ProgressBar";

import { courseMenuItemType } from "../../types"
import { Calendar, EyeClosed, } from "icons";
import { Hint, } from "ui";

import { getDateDDMMYY } from "src/utils/momentUtils";
import classnames from "classnames";

import styles from "./CourseNavigationItem.less";
import { isMobile, isTablet } from "src/utils/getDeviceType";

function CourseNavigationItem({ title, isActive, isNotPublished, publicationDate, progress, onClick, }) {
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
				{ renderProgress() }
			</div>
		</li>
	);

	function renderProgress() {
		const percentage = progress.current / progress.max;

		if(percentage > 0) {
			return (
				<span className={ styles.progressWrapper } title={ `${ progress.current } из ${ progress.max }` }>
					<ProgressBar value={ percentage }
								 small
								 color={ percentage >= 1 ? 'green' : 'blue' }
								 active={ isActive }
					/>
				</span>
			);
		}
	}

	function clickHandle() {
		onClick(this.props.id);
	}

	function hintClickHandle(e) {
		if(isMobile() || isTablet()) {
			e.stopPropagation();
		}
	}
}

CourseNavigationItem.propTypes = courseMenuItemType;

export default CourseNavigationItem

