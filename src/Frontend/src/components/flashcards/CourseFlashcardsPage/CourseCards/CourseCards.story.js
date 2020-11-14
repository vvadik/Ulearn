import CourseCards from "./CourseCards";
import React from "react";
import cardsByUnitExample from "./cardsByUnitExample";

export default {
	title: "Cards/CoursePage/CourseCards",
};

export const Def = () => <CourseCards flashcardsInfos={cardsByUnitExample} />;

Def.storyName = "def";
