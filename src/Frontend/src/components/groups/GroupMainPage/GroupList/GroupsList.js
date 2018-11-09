import React, {Component} from "react";
import PropTypes from "prop-types";
import Loader from "@skbkontur/react-ui/components/Loader/Loader";
import Input from "@skbkontur/react-ui/components/Input/Input";
import Icon from "@skbkontur/react-ui/components/Icon/Icon";
import GroupInfo from "../GroupInfo/GroupInfo";

import './style.less';

class GroupsList extends Component {
	render() {
		return (
			<section className="groups-wrapper">
				<Input className="search-field" placeholder="Название группы" leftIcon={<Icon name="Search" />} />
				<Loader type="big" active={this.props.loading} >
					{ this.props.groups && this.props.groups.map(group =>
						<GroupInfo
							key={group.id}
							group={group}
							deleteGroup={this.props.deleteGroup}
							toggleArchived={this.props.toggleArchived}
						/>)
					}
				</Loader>
			</section>
		);
	}
}

GroupsList.propTypes = {
	groups: PropTypes.array,
	loading: PropTypes.bool,
	deleteGroup: PropTypes.func,
	toggleArchived: PropTypes.func,
};

export default GroupsList;