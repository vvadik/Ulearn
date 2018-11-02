import {Component} from "react";
import Loader from "@skbkontur/react-ui/components/Loader/Loader";
import Input from "@skbkontur/react-ui/components/Input/Input";
import Icon from "@skbkontur/react-ui/components/Icon/Icon";
import GroupInfo from "../GroupInfo/GroupInfo";
import PropTypes from "prop-types";
import React from "react";

import './style.less';

class GroupsList extends Component {

	render() {
		if (this.props.loading) {
			return (
				<Loader type="big" active />
			)
		}
		return (
			<div className="groups-wrapper">
				<Input className="search-field" placeholder="Название группы" leftIcon={<Icon name="Search" />} />
				{ this.props.groups.map(group =>
					<GroupInfo
						key={group.id}
						group={group}
						deleteGroup={this.props.deleteGroup}
						makeArchival={this.props.makeArchival}
					/>) }
			</div>
		);
	}
}

GroupsList.propTypes = {
	groups: PropTypes.array,
	loading: PropTypes.bool
};

export default GroupsList;