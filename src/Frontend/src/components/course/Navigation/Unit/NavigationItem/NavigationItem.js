import React, { Component } from "react";
import { Link } from "react-router-dom";

import { Copy, EyeClosed, } from "icons";
import { Hint } from "ui";

import { SlideType } from 'src/models/slide';
import { menuItemType } from '../../types';
import classnames from 'classnames';

import styles from './NavigationItem.less';

const icons = {
	[SlideType.Quiz]: '?',
	[SlideType.Exercise]: <div className={ styles.exerciseIcon }>{ '<>' }</div>,
	[SlideType.Flashcards]: <Copy/>,
};

class NavigationItem extends Component {
	componentDidMount() {
		const { ref } = this;

		if(ref) {
			ref.scrollIntoView();
		}
	}

	createRefIfNeeded = (ref) => {
		const { isActive } = this.props;

		if(isActive) {
			this.ref = ref;
		}
	};

	render() {
		const { text, url, isActive, description, metro, toggleNavigation, hide, } = this.props;

		const classes = {
			[styles.itemLink]: true,
			[styles.active]: isActive,
			[styles.withMetro]: metro
		};


		return (
			<li className={ styles.root } ref={ this.createRefIfNeeded }>
				<Link to={ url } className={ classnames(classes) } onClick={ toggleNavigation }>
					{ metro && this.renderMetro() }
					<div className={ styles.firstLine }>
						<span className={ styles.text }>
							{ hide && <span className={ styles.isHiddenIcon }>
								<Hint text={ `Этот слайд скрыт` }>
									<EyeClosed/>
								</Hint>
							</span> }
							{ text }
						</span>
						{ this.renderScore() }
					</div>
					{ description &&
					<div className={ styles.description }>{ description }</div>
					}
				</Link>
			</li>
		);
	}


	renderScore() {
		const { score, maxScore, type } = this.props;

		if(!maxScore) {
			return;
		}

		if(type === SlideType.Exercise || type === SlideType.Quiz) {
			return (
				<div className={ styles.scoreWrapper }>
					<span className={ styles.score }>{ score || 0 }/{ maxScore }</span>
				</div>
			);
		}
	}

	renderMetro() {
		const { metro } = this.props;

		if(!metro) {
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
			<div className={ classnames(classes) }>
				{ this.renderPointer() }
			</div>
		);
	}

	renderPointer() {
		const { type, visited } = this.props;

		if(type === SlideType.Lesson) {
			return (
				<span className={ classnames(styles.pointer, { [styles.complete]: visited }) }/>
			);
		}

		return (
			<span className={ classnames(styles.icon, { [styles.complete]: visited }) }>{ icons[type] }</span>
		);
	}
}

NavigationItem.propTypes = menuItemType;

export default NavigationItem;

