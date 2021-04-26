import React from "react";
import { Hint } from "ui";
import { Ok } from "icons";

interface Props {
	isPassed: boolean,
}

export default function Like({ isPassed }: Props): React.ReactElement {
	if(!isPassed) {
		return <React.Fragment/>;
	}
	return <Hint text="Решил задачу"><Ok size={ 15 }/></Hint>
}
