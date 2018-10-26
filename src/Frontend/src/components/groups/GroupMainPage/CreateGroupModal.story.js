import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import CreateGroupModal from './CreateGroupModal'

storiesOf('Group/CreateGroupModal', module)
	.add('default', () => (
		<CreateGroupModal closeModal={action('closeModal')}/>
	));