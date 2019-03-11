import React from 'react-dom';
import PropTypes from "prop-types";
import { userRoles, user } from "./commonPropTypes";
import api from "../../api";
import CommentsWrapper from "./CommentsWrapper/CommentsWrapper";

function ProviderApiToComments(props) {
	const {user, userRoles, courseId, slideId} = props;

	return (
		<CommentsWrapper
			user={user}
			userRoles={userRoles}
			courseId={courseId}
			slideId={slideId}
			commentsApi={api.comments}
		/>
	)
}

ProviderApiToComments.propTypes = {
	user: user.isRequired,
	userRoles: userRoles.isRequired,
	courseId: PropTypes.string.isRequired,
	slideId: PropTypes.string.isRequired,
};