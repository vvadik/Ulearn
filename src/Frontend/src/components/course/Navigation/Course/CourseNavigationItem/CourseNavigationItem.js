import React, { Component } from "react";

import ProgressBar from "../../ProgressBar";

import { courseMenuItemType } from "../../types"
import { Calendar, EyeClosed, } from "icons";
import { Hint, } from "ui";

import { getDateDDMMYY } from "src/utils/getMoment";
import classnames from "classnames";

import styles from "./CourseNavigationItem.less";
import { isMobile, isTablet } from "src/utils/getDeviceType";

class CourseNavigationItem extends Component {
	render() {
		const { title, isActive, isNotPublished, publicationDate } = this.props;

		const classes = classnames(
			styles.itemLink,
			{ [styles.active]: isActive },
		);

		return (
			<li className={ styles.root } onClick={ this.clickHandle }>
				<div className={ classes }>
					<span className={ styles.text }>
						{ isNotPublished &&
						<span className={ styles.isNotPublishedIcon } onClick={ this.hintClickHandle }>
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
					{ this.renderProgress() }
				</div>
			</li>
		);
	}

	renderProgress() {
		const { progress, isActive } = this.props;
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

	clickHandle = () => {
		this.props.onClick(this.props.id);
	};

	hintClickHandle = (e) => {
		if(isMobile() || isTablet()) {
			e.stopPropagation();
		}
	};
}

CourseNavigationItem.propTypes = courseMenuItemType;

export default CourseNavigationItem

