import React from "react";
import PropTypes from "prop-types";
import Link from "@skbkontur/react-ui/components/Link/Link";

function Profile(props) {
	const { isSysAdmin, systemAccesses, user } = props;
	console.log(isSysAdmin);
	const canViewProfiles = systemAccesses.includes("viewAllProfiles") || isSysAdmin;
	const profileUrl = `${window.location.origin}/Account/Profile?userId=${user.id}`;

	return canViewProfiles
		? <div><Link href={profileUrl}>{user.visible_name}</Link></div>
		: <div>{user.visible_name}</div>;
}

Profile.propTypes = {
	user: PropTypes.string,
	systemAccesses: PropTypes.array,
	isSysAdmin: PropTypes.bool,
};

export default Profile;