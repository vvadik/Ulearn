import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import GroupMembers from './GroupMembers';

import './groupMembers.less';

storiesOf('Settings/GroupMembers', module)
	.add('default', () => (
		<GroupMembers group={getGroup()}/>
	));

function getGroup() {
	return {
		"id": 17,
		"name": "asdfasdfasdfasdf",
		"is_archived": false,
		"owner": {
			"id": "4052ea63-34dd-4398-b8bb-ac4e6a85d1d0",
			"visible_name": "paradeeva",
			"avatar_url": null,
		},
		"invite_hash": "b7638c37-62c6-49a9-898c-38788169987c",
		"is_invite_link_enabled": true,
		"is_manual_checking_enabled": false,
		"is_manual_checking_enabled_for_old_solutions": false,
		"default_prohibit_further_review": true,
		"can_students_see_group_progress": true,
		"students_count": 0,
		"accesses": [],
		"api_url": "/groups/17"
	}
}