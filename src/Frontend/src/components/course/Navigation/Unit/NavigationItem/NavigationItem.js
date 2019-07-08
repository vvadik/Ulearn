import React, { Component } from "react";
import { Link } from "react-router-dom";
import styles from './NavigationItem.less';
import classnames from 'classnames';
import { itemTypes } from '../../constants';
import { menuItemType } from '../../types';


const icons = {
	[itemTypes.quiz]: '?',
	[itemTypes.exercise]: '{}',
};

class NavigationItem extends Component {
	render () {
		const { text, url, isActive, description, metro } = this.props;

		const classes = {
			[styles.itemLink]: true,
			[styles.active]: isActive,
			[styles.withMetro]: metro
		};


		return (
			<li className={ styles.root }>
				<Link to={ url } className={ classnames(classes) }>
					{ metro && this.renderMetro() }
					<div className={ styles.firstLine }>
						<span className={ styles.text }>{ text }</span>
						{ this.renderScore() }
					</div>
					{ description &&
						<div className={ styles.description }>{description}</div>
					}
				</Link>
			</li>
		);
	}

	renderScore() {
		const { score, maxScore, type } = this.props;

		if (!maxScore) {
			return;
		}

		if (type === itemTypes.exercise || type === itemTypes.quiz) {
			return (
				<div className={ styles.scoreWrapper }>
					<span className={ styles.score }>{ score || 0 }/{ maxScore }</span>
				</div>
			);
		}
	}

	renderMetro() {
		const { metro } = this.props;

		if (!metro) {
			return null;
		}

		const { isFirstItem, isLastItem, connectToPrev, connectToNext } = metro;

		const classes = {
			[styles.metroWrapper]: true,
			[styles.withoutBottomLine]: isLastItem,
			[styles.longTopLine]: isFirstItem,
			[styles.completeTop]: connectToPrev,
			[styles.completeBottom]: connectToNext,
		};

		return (
			<div className={ classnames(classes)}>
				{ this.renderPointer() }
			</div>
		);
	}

	renderPointer() {
		const { type, visited } = this.props;

		if (type === itemTypes.lesson) {
			return (
				<span className={ classnames(styles.pointer, {[styles.complete]: visited}) } />
			);
		}

		return (
			<span className={ classnames(styles.icon, {[styles.complete]: visited}) }>{icons[type]}</span>
		);
	}
}

NavigationItem.propTypes = menuItemType;

export default NavigationItem

