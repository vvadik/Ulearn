import { Component } from "react";
import CreateGroupModal from "./CreateGroupModal";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Tabs from "@skbkontur/react-ui/components/Tabs/Tabs";
import PropTypes from "prop-types";
import React from "react";


class GroupHeader extends Component {

	state = {
		modalOpened: false
	};

	onChange = (_, v) => {
		this.props.onTabClick(v);
	};

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

	render() {
		return (
			<React.Fragment>
				{ this.renderHeader() }
				{this.state.modalOpened && <CreateGroupModal closeModal={this.closeModal} courseId={this.props.courseId}/>}
			</React.Fragment>
		)
	}

	renderHeader() {
		return (
			<div className="group-header">
				<div className="group-header-container">
					<h2>Группы</h2>
					<div className="buttons-container">
						<Button use="primary" size="medium" onClick={this.openModal}>Создать группу</Button>
						<Button use="default" size="medium">Скопировать группу</Button>
					</div>
				</div>
				<div className="tabs-container">
					<Tabs  value={this.props.filter} onChange={this.onChange}>
						<Tabs.Tab id="active">Активные</Tabs.Tab>
						<Tabs.Tab id="archived">Архивные</Tabs.Tab>
					</Tabs>
				</div>
			</div>
		)
	}
}

GroupHeader.propTypes = {
	onTabClick: PropTypes.func,
	filter: PropTypes.string,
	openModal: PropTypes.bool,
	closeModal: PropTypes.bool,
	courseById: PropTypes.string
};

export default GroupHeader;