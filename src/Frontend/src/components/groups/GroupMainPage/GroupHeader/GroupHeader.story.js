import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import GroupHeader from './GroupHeader';

import './style.less';

storiesOf('Group/GroupHeader', module)
	.add('default', () => (
		<GroupHeader onTabClick={action('click')} filter="hello"/>
	));