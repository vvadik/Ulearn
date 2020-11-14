import React, { Component } from 'react';
import { Switch, Route, Redirect } from 'react-router-dom';

import AnyPage from "./pages/AnyPage";
import GroupListPage from "./pages/course/groups/GroupListPage";
import GroupPage from "./pages/course/groups/GroupPage";
import Course from './pages/course/CoursePage';

import { getQueryStringParameter } from "./utils";


function Router({ account }) {
	let routes = [
		<Route key={'groups'} path="/Admin/Groups" component={ redirectLegacyPage("/:courseId/groups") }/>,
		<Route key={'course'} path="/course/:courseId/:slideSlugOrAction" component={ Course }/>,
	];

	if(account.accountLoaded) {
		if(account.isAuthenticated) {
			routes = [
				...routes,
				<Route key={'groupsList'} path="/:courseId/groups/" component={ GroupListPage } exact/>,
				<Route key={'groupPage'} path="/:courseId/groups/:groupId/" component={ GroupPage } exact/>,
				<Route key={'groupPageSettings'} path="/:courseId/groups/:groupId/:groupPage" component={ GroupPage } exact/>,
			];
		}
		routes.push(<Route key={'anyPage'} component={ AnyPage }/>);
	}

	return (
		<Switch>
			{ routes }
		</Switch>
	)
}

function redirectLegacyPage(to) {
	return class extends Component {
		constructor(props) {
			super(props);
			let courseId = getQueryStringParameter("courseId");
			if(courseId)
				to = to.replace(":courseId", courseId);
		}

		render() {
			return <Redirect to={ to }/>;
		}
	};
}

export default Router;
