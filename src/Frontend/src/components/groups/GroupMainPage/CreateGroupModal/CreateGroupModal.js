import React, { Component } from 'react';
import PropTypes from "prop-types";
import Modal from '@skbkontur/react-ui/components/Modal/Modal';
import Input from '@skbkontur/react-ui/components/Input/Input';
import Button from '@skbkontur/react-ui/components/Button/Button';
import Gapped from '@skbkontur/react-ui/components/Gapped';
import Tooltip from '@skbkontur/react-ui/components/Tooltip/Tooltip';
import Loader from "@skbkontur/react-ui/components/Loader/Loader";
import api from "../../../../api";

import './style.less';

class CreateGroupModal extends Component {

	state = {
		name: '',
		hasError: false,
		error: null,
		loading: false
	};

	render() {
		const { closeModal } = this.props;
		return (
			<Modal onClose={closeModal} width={640}>
				<form onSubmit={this.onSubmit}>
					{this.renderModalBody()}
					<Modal.Footer>
						<Button
							use="primary"
							size="medium"
							type="submit"
						>
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
				<Loader type="big" active={this.state.loading}>
					<label className="modal-label">
						<Gapped gap={20}>
							Название группы
							<Tooltip render={this.tooltipRender} trigger='focus' pos="right top">
								<Input placeholder="КН-201 УрФУ 2017"
									   maxLength="20"
									   value={name || ''}
									   error={hasError}
									   onChange={this.onChangeInput}
									   onFocus={this.onFocus}
									   autoFocus
								/>
							</Tooltip>
						</Gapped>
					</label>
					<p>
						Студенты увидят название группы, поэтому постарайтесь сделать его понятным.<br />
						Пример хорошего названия группы:
						<span className="group-content-state_on">«КН-201 УрФУ 2017»,</span><br />
						пример плохого: <span className="group-content-state_off"> «Моя группа 2»</span>
					</p>
				</Loader>
			</Modal.Body>
		)
	}

	onSubmit = async (e) => {
		const { name } = this.state;
		const { closeModal, createGroup, courseId } = this.props;
		e.preventDefault();
		if (!name) {
			this.setState({
				hasError: true,
				error: 'Введите название группы',
			});
			return;
		}
		const newGroup = await api.groups.createGroup(courseId, name);
		if (!newGroup) {
			this.setState({
				loading: true,
			});
		}
		closeModal();
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
	closeModal: PropTypes.func,
	courseId: PropTypes.string,
	createGroup: PropTypes.func,
};

export default CreateGroupModal;