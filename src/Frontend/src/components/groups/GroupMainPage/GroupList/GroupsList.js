import React, {Component} from "react";
import PropTypes from "prop-types";
import Loader from "@skbkontur/react-ui/components/Loader/Loader";
import Input from "@skbkontur/react-ui/components/Input/Input";
import Icon from "@skbkontur/react-icons";
import GroupInfo from "../GroupInfo/GroupInfo";

import styles from "./style.less";

class GroupsList extends Component {
	render() {
		return (
			<section className={styles["groups-wrapper"]}>
				<Input className={styles["search-field"]} placeholder="Название группы" leftIcon={<Icon name="Search" />} />
				<Loader type="big" active={this.props.loading}>
					<div className={styles["groups-container"]}>
						{ this.props.groups && this.props.groups.map(group =>
							<GroupInfo
								key={group.id}
								courseId={this.props.courseId}
								group={group}
								deleteGroup={this.props.deleteGroup}
								toggleArchived={this.props.toggleArchived}
							/>)
						}
					</div>
				</Loader>
			</section>
		);
	}
}

GroupsList.propTypes = {
	courseId: PropTypes.string.isRequired,
	groups: PropTypes.array,
	loading: PropTypes.bool,
	deleteGroup: PropTypes.func,
	toggleArchived: PropTypes.func,
};

export default GroupsList;