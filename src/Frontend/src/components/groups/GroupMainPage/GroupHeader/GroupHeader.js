import { Component } from "react";
import CreateGroupModal from "../CreateGroupModal/CreateGroupModal";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Tabs from "@skbkontur/react-ui/components/Tabs/Tabs";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import PropTypes from "prop-types";
import React from "react";

import './style.less';

const TABS = {
	active: 'active',
	archived: 'archived',
};

class GroupHeader extends Component {

	state = {
		modalOpened: false
	};

	render() {
		return (
			<React.Fragment>
				{ this.renderHeader() }
				{ this.state.modalOpened &&
					<CreateGroupModal closeModal={this.closeModal} courseId={this.props.courseId}
				/>}
			</React.Fragment>
		)
	}

	renderHeader() {
		return (
			<div className="group-header">
				<div className="group-header-container">
					<h2>Группы</h2>
					<div className="buttons-container">
						<Gapped gap={20}>
							<Button use="primary" size="medium" onClick={this.openModal}>Создать группу</Button>
							<Button use="default" size="medium">Скопировать группу</Button>
						</Gapped>
					</div>
				</div>
				<div className="tabs-container">
					<Tabs value={this.props.filter} onChange={this.onChange}>
						<Tabs.Tab id={TABS.active}>Активные</Tabs.Tab>
						<Tabs.Tab id={TABS.archived}>Архивные</Tabs.Tab>
					</Tabs>
				</div>
			</div>
		)
	}

	openModal = () => {
		this.setState({
			modalOpened: true
		})
	};

	closeModal = () => {
		this.setState({
			modalOpened: false
		})
	};

	onChange = (_, v) => {
		this.props.onTabClick(v);
	};
}

GroupHeader.propTypes = {
	onTabClick: PropTypes.func,
	filter: PropTypes.string,
	openModal: PropTypes.bool,
	closeModal: PropTypes.bool,
	courseById: PropTypes.string
};

export default GroupHeader;