import React from "react";
import { Link } from "react-router-dom";
import classnames from 'classnames';

import { Copy, EyeClosed, } from "icons";
import { Hint } from "ui";

import { SlideType } from 'src/models/slide';
import { MenuItem } from "../../types";

import styles from './NavigationItem.less';

const icons: { [T in SlideType]: React.ReactNode } = {
	[SlideType.Quiz]: '?',
	[SlideType.Exercise]: <div className={ styles.exerciseIcon }>{ '<>' }</div>,
	[SlideType.Flashcards]: <Copy/>,
	[SlideType.Lesson]: null,
	[SlideType.CourseFlashcards]: null,
	[SlideType.PreviewFlashcards]: null,
};

interface Props extends MenuItem<SlideType> {
	metro: {
		isFirstItem: boolean;
		isLastItem: boolean;
		connectToPrev: boolean;
		connectToNext: boolean;
	},
	description: string | null;
	onClick: () => void;
}

function NavigationItem({
	title,
	url,
	isActive,
	description,
	metro,
	onClick,
	hide,
	score,
	maxScore,
	type,
	visited,
}: Props): React.ReactElement {
	const scrollIfNeeded = (ref: HTMLLIElement) => {
		if(isActive && ref) {
			if(ref.getBoundingClientRect().top > window.innerHeight) {
				ref.scrollIntoView();
			}
		}
	};

	const classes = {
		[styles.itemLink]: true,
		[styles.active]: isActive,
		[styles.withMetro]: metro
	};

	return (
		<li className={ styles.root } ref={ scrollIfNeeded }>
			<Link to={ url } className={ classnames(classes) } onClick={ onClick }>
				{ metro && renderMetro() }
				<div className={ styles.firstLine }>
						<span className={ styles.text }>
							{ hide && <span className={ styles.isHiddenIcon }>
								<Hint text={ `Этот слайд скрыт` }>
									<EyeClosed/>
								</Hint>
							</span> }
							{ title }
						</span>
					{ renderScore() }
				</div>
				{ description &&
				<div className={ styles.description }>{ description }</div>
				}
			</Link>
		</li>
	);


	function renderScore() {
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

	function renderMetro() {
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
				{ renderPointer() }
			</div>
		);
	}

	function renderPointer() {
		if(type === SlideType.Lesson) {
			return (
				<span className={ classnames(styles.pointer, { [styles.complete]: visited }) }/>
			);
		}

		return (
			<span className={ classnames(styles.icon, { [styles.complete]: visited }) }>{ type && icons[type] }</span>
		);
	}
}

export default NavigationItem;

