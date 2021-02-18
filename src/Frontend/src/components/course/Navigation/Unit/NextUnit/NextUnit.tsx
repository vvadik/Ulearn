import React from "react";
import { Link } from "react-router-dom";

import { ArrowChevronRight } from "icons";

import { UnitInfo } from "src/models/course";

import styles from './NextUnit.less';


interface Props {
	unit: UnitInfo;
	onClick: () => void;
}

function NextUnit({ onClick, unit, }: Props): React.ReactElement {
	const { title, slides } = unit;

	const slideId = slides[0].slug;

	return (
		<Link to={ slideId } className={ styles.root } onClick={ onClick }>
			<div className={ styles.wrapper }>
				<h5 className={ styles.header }>Следующий модуль</h5>
				<h4 className={ styles.title } title={ title }>{ title }</h4>
			</div>
			<ArrowChevronRight size={ 24 } className={ styles.arrow }/>
		</Link>
	);
}

export default NextUnit;
