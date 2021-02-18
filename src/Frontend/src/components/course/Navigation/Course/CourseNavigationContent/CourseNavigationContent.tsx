import React from "react";

import CourseNavigationItem from '../CourseNavigationItem';

import { CourseMenuItem, } from '../../types';

import styles from './CourseNavigationContent.less';


interface Props {
	items: CourseMenuItem[];
}

function CourseNavigationContent({ items }: Props): React.ReactElement {
	return (
		<div className={ styles.root }>
			<h5 className={ styles.header }>Программа курса</h5>
			{ items.map((item) => <CourseNavigationItem key={ item.id } { ...item }/>) }
		</div>
	);
}

export default CourseNavigationContent;
