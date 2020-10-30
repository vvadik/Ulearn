import React from 'react';
import {storiesOf} from '@storybook/react';
import Flashcards from "./Flashcards";
import flashcardsExample from "./flashcardsExample";

storiesOf('Cards', module)
	.add('default', () => (
		<Flashcards flashcards={flashcardsExample}/>
	));
