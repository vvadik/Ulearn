import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import GroupInfo from './GroupInfo'

storiesOf('Group/GroupInfo', module)
	.add('default', (group) => (
		<GroupInfo group={group} />
	));