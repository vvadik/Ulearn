import React, { Component } from 'react';
import PropTypes from "prop-types";
import api from "../../../../api";
import Modal from '@skbkontur/react-ui/components/Modal/Modal';
import Input from '@skbkontur/react-ui/components/Input/Input';
import Button from '@skbkontur/react-ui/components/Button/Button';
import Tooltip from '@skbkontur/react-ui/components/Tooltip/Tooltip';

import styles from "./style.less";

class CreateGroupModal extends Component {

	state = {
		name: '',
		hasError: false,
		error: null,
		loading: false,
	};

	render() {
		const { onCloseModal } = this.props;

		return (
			<Modal onClose={onCloseModal} width={640}>
				<Modal.Header>Название группы</Modal.Header>
				<form onSubmit={this.onSubmit}>
					{this.renderModalBody()}
					<Modal.Footer>
						<Button
							use="primary"
							size="medium"
							type="submit"
							disabled={!this.state.name}
							loading={this.state.loading}>
							Создать
						</Button>
					</Modal.Footer>
				</form>
			</Modal>
		)
	}

	renderModalBody() {
		const { name, hasError } = this.state;

		return (
			<Modal.Body>
				<Tooltip render={this.tooltipRender} trigger='focus' pos="right top">
					<Input placeholder="КН-201 УрФУ 2017"
						   maxLength="20"
						   value={name || ''}
						   error={hasError}
						   onChange={this.onChangeInput}
						   onFocus={this.onFocus}
						   autoFocus />
				</Tooltip>
				<p className={styles["common-info"]}>
					Студенты увидят название группы, поэтому постарайтесь сделать его понятным.<br />
					Пример хорошего названия группы: <span className={styles["good-name"]}>
					КН-201 УрФУ 2017,</span><br />
					пример плохого: <span className={styles["bad-name"]}>Моя группа 2</span>
				</p>
			</Modal.Body>
		)
	}

	onSubmit = async (e) => {
		const { name } = this.state;
		const { onCloseModal, createGroup, courseId } = this.props;

		e.preventDefault();

		if (!name) {
			this.setState({
				hasError: true,
				error: 'Введите название группы',
			});
			return;
		}

		this.setState({ loading: true, });
		const newGroup = await api.groups.createGroup(courseId, name);
		this.setState({ loading: false, });

		onCloseModal();
		createGroup(newGroup.id);
	};

	tooltipRender = () => {
		const { error } = this.state;

		if (!error) {
			return null;
		}
		return error;
	};

	onFocus = () => {
		this.setState({
			hasError: false,
		});
	};

	onChangeInput = (_, value) => {
		this.setState({
			name: value,
		});
	};
}

CreateGroupModal.propTypes = {
	onCloseModal: PropTypes.func,
	courseId: PropTypes.string,
	createGroup: PropTypes.func,
};

export default CreateGroupModal;