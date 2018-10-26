import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import GroupMembers from './GroupMembers';

storiesOf('Settings/GroupMembers', module)
	.add('default', () => (
		<GroupMembers />
	));