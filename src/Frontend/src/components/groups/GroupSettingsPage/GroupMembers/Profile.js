import React from "react";
import PropTypes from "prop-types";
import Link from "@skbkontur/react-ui/components/Link/Link";

function Profile(props) {
	const { isSysAdmin, systemAccesses, user } = props;
	const canViewProfiles = systemAccesses.includes("viewAllProfiles") || isSysAdmin;
	const profileUrl = `${window.location.origin}/Account/Profile?userId=${user.id}`;

	return canViewProfiles
		? <Link href={profileUrl}>{user.visible_name}</Link>
		: <div>{user.visible_name}</div>;
}

Profile.propTypes = {
	user: PropTypes.object,
	systemAccesses: PropTypes.array,
	isSysAdmin: PropTypes.bool,
};

export default Profile;