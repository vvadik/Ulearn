import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import GroupsList from './GroupsList'
import { MemoryRouter } from 'react-router';

import './style.less';

storiesOf('Group/GroupsList', module)
	.addDecorator(story => (
		<MemoryRouter initialEntries={['/groups/']}>{story()}</MemoryRouter>
	))
	.add('default', () => (
		<GroupsList groups={getGroups()} />
	));

function getGroups() {
	return  [
			{
				"id": 17,
				"create_time": "2018-10-19T13:46:28.153",
				"name": "asdfasdfasdfasdf",
				"is_archived": false,
				"owner": {
					"id": "4052ea63-34dd-4398-b8bb-ac4e6a85d1d0",
					"first_name": "",
					"last_name": "",
					"visible_name": "paradeeva",
					"avatar_url": null,
					"gender": null
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
			},
			{
				"id": 14,
				"create_time": null,
				"name": "asdfghj",
				"is_archived": false,
				"owner": {
					"id": "4052ea63-34dd-4398-b8bb-ac4e6a85d1d0",
					"first_name": "",
					"last_name": "",
					"visible_name": "paradeeva",
					"avatar_url": null,
					"gender": null
				},
				"invite_hash": "e22214b7-774c-4f14-b80c-910c6e715301",
				"is_invite_link_enabled": true,
				"is_manual_checking_enabled": true,
				"is_manual_checking_enabled_for_old_solutions": true,
				"default_prohibit_further_review": true,
				"can_students_see_group_progress": true,
				"students_count": 0,
				"accesses": [],
				"api_url": "/groups/14"
			},
		]
}
