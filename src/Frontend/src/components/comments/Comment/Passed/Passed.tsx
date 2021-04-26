import React from "react";
import { Ok } from "icons";
import texts from "../../../course/Course/Slide/Blocks/Exercise/Exercise.texts";
import { Hint } from "ui";

interface Props {
	isPassed: boolean,
}

export default function Like({ isPassed }: Props): React.ReactElement {
	if(!isPassed) {
		return <React.Fragment/>;
	}
	return <Hint text="Решил задачу"><Ok size={ 15 }/></Hint>
}
