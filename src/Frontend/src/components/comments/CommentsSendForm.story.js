import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import CommentSendForm from './CommentSendForm';

import './comment.less';

storiesOf('Comments/CommentSendForm', module)
	.add('default', () => (
		<CommentSendForm onSubmit={action('sendComment')} />
	));

function getUser() {
	return {
		"id": "1736826p",
		"login": "string",
		"email": "string",
		"first_name": "m",
		"last_name": "paradeeva",
		"visible_name": "maria",
		"avatar_url": null,
		"gender": "female"
	}
}