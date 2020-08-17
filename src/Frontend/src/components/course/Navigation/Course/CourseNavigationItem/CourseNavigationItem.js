import React, { Component } from "react";

import ProgressBar from "../../ProgressBar";

import classnames from "classnames";
import { courseMenuItemType } from "../../types"
import { Calendar, } from "icons";
import { Hint } from "ui";

import { getDateDDMMYY } from "src/utils/getMoment";

import styles from "./CourseNavigationItem.less";

class CourseNavigationItem extends Component {
	render() {
		const { title, isActive, isNotPublished, publicationDate } = this.props;

		const classes = {
			[styles.itemLink]: true,
			[styles.active]: isActive,
			[styles.isNotPublished]: isNotPublished,
		};

		return (
			<li className={ styles.root } onClick={ this.clickHandle }>
				<div className={ classnames(classes) }>
					<span className={ styles.text }>
						{ isNotPublished && publicationDate &&
						<span className={ styles.isNotPublishedIcon }>
							<Hint text={ `Этот модуль будет опубликован ${ getDateDDMMYY(publicationDate) }` }>
									<Calendar/>
							</Hint>
						</span> }
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
}

CourseNavigationItem.propTypes = courseMenuItemType;

export default CourseNavigationItem

