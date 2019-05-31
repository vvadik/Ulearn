import React, { Component } from "react";
import PropTypes from 'prop-types';
import styles from './CourseNavigationItem.less';
import classnames from 'classnames';
import ProgressBar from '../../ProgressBar';


class CourseNavigationItem extends Component {
	render () {
		const { text, url, isActive } = this.props;

		const classes = {
			[styles.itemLink]: true,
			[styles.active]: isActive
		};

		return (
			<li className={ styles.root }>
				<a href={ url } className={ classnames(classes) }>
					<div className={ styles.firstLine }>
						<span className={ styles.text }>{ text }</span>
						{ this.renderProgress() }
					</div>
				</a>
			</li>
		);
	}

	renderProgress() {
		const { progress } = this.props;

		if (progress) {
			return (
				<span className={ styles.progressWrapper }>
					<ProgressBar value={ progress } small color={ progress >= 1 ? 'green' : 'blue' } />
				</span>
			);
		}
	}
}

CourseNavigationItem.propTypes ={
	text: PropTypes.string.isRequired,
	url: PropTypes.string,
	progress: PropTypes.number,
};

export default CourseNavigationItem

