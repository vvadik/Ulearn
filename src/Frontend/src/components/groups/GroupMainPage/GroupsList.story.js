import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import GroupsList from './GroupsList'

storiesOf('Group/GroupsList', module)
	.add('default', (groups) => (
		<GroupsList groups={groups} />
	));