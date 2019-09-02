import CourseCards from './CourseCards';
import React from 'react';
import {storiesOf} from '@storybook/react';
import cardsByUnitExample from "./cardsByUnitExample";

storiesOf('Cards/CoursePage/CourseCards', module)
	.add('def', () => (
		<CourseCards flashcardsInfos={cardsByUnitExample}/>
	));


