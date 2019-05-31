import React, { Component } from "react";
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
				<a href={ url } className={ classnames(classes) }>
					{ metro && this.renderMetro() }
					<div className={ styles.firstLine }>
						<span className={ styles.text }>{ text }</span>
						{ this.renderScore() }
					</div>
					{ description &&
						<div className={ styles.description }>{description}</div>
					}
				</a>
			</li>
		);
	}

	renderScore() {
		const { score } = this.props;

		if (typeof score === 'number') {
			return (
				<div className={ styles.scoreWrapper }>
					<span className={ styles.score }>{ Math.round(score * 100) }%</span>
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
		const { complete, type } = this.props.metro;

		if (type === itemTypes.lesson) {
			return (
				<span className={ classnames(styles.pointer, {[styles.complete]: complete}) } />
			);
		}

		return (
			<span className={ classnames(styles.icon, {[styles.complete]: complete}) }>{icons[type]}</span>
		);
	}
}

NavigationItem.propTypes = menuItemType;

export default NavigationItem

