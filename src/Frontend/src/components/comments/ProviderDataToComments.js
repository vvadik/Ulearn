import React, { Component } from 'react';
import PropTypes from "prop-types";
import { userRoles, user } from "./commonPropTypes";
import api from "../../api";
import CommentsView from "./CommentsView/CommentsView";

class ProviderDataToComments extends Component {
	render() {
		const {user, userRoles, courseId, slideId} = this.props;
		console.log(user);

		return (
			<CommentsView
				user={user}
				userRoles={userRoles}
				courseId={courseId}
				slideId={slideId}
				commentsApi={api.comments}
			/>
		)
	}
}

ProviderDataToComments.propTypes = {
	user: user.isRequired,
	userRoles: userRoles.isRequired,
	courseId: PropTypes.string.isRequired,
	slideId: PropTypes.string.isRequired,
};

export default ProviderDataToComments;