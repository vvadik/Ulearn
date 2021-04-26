import React from "react";
import { Hint } from "ui";
import { Ok } from "icons";

import styles from "./Passed.less";

interface Props {
	isPassed: boolean,
}

export default function Passed({ isPassed }: Props): React.ReactElement {
	if(!isPassed) {
		return <React.Fragment/>;
	}
	return <div className={ `${ styles.wrapper }` }><Hint text="Решил задачу"><Ok size={ 15 }/></Hint></div>
}
