import React from "react";

import { Link } from "react-router-dom";
import { flashcards, constructPathToSlide } from 'src/consts/routes';

import classnames from 'classnames';

import styles from './Flashcards.less';


interface Props {
	courseId: string;
	isActive: boolean;

	toggleNavigation: () => void;
}


function Flashcards({ courseId, isActive, toggleNavigation }: Props): React.ReactElement {
	return (
		<Link to={ constructPathToSlide(courseId, flashcards) }
			  className={ classnames(styles.root, { [styles.isActive]: isActive }) }
			  onClick={ toggleNavigation }
		>
			<h5 className={ styles.header }>Карточки</h5>
			<span className={ styles.text }>Вопросы для самопроверки</span>
		</Link>
	);
}

export default Flashcards;

