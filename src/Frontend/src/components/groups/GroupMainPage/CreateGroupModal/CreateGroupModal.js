import { Component } from 'react';
import PropTypes from "prop-types";
import Modal from '@skbkontur/react-ui/components/Modal/Modal';
import Input from '@skbkontur/react-ui/components/Input/Input';
import Button from '@skbkontur/react-ui/components/Button/Button';
import Gapped from '@skbkontur/react-ui/components/Gapped';
import Tooltip from '@skbkontur/react-ui/components/Tooltip/Tooltip';
import React from 'react';

import './style.less';
import api from "../../../../api";

class CreateGroupModal extends Component {

	state = {
		name: null,
		hasError: false,
		error: ''
	};

	render() {
		const { closeModal } = this.props;
		const { name, hasError } = this.state;
		return (
			<Modal onClose={closeModal} width={640}>
				<form onSubmit={this.onSubmit} method="post">
					<Modal.Body>
						<label className="modal-label">
							<Gapped gap={20}>
								Название группы
								<Tooltip render={this.tooltipRender} pos="right top">
									<Input placeholder="КН-201 УрФУ 2017"
										   // required
										   maxLength="20"
										   // pattern="/[0-9a-zа-яё]+/g"
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
							Пример хорошего названия группы: <span className="group-content__state_on">«КН-201 УрФУ 2017»,</span><br />
							пример плохого: <span className="group-content__state_off"> «Моя группа 2»</span>
						</p>
					</Modal.Body>
					<Modal.Footer>
						<Button
							use="primary"
							size="medium"
							type="submit"
							onClose={closeModal}
						>
							Создать
						</Button>
					</Modal.Footer>
				</form>
			</Modal>
		)
	}

	onSubmit = async (e) => {
		const { name } = this.state;
		const { closeModal, onSubmit, courseId } = this.props;
		e.preventDefault();
		if (!name) {
			this.setState({
				hasError: true,
				error: 'Введите название группы',
			});
			return;
		}
		const newGroup = await api.groups.createGroup(courseId, name);
		closeModal();
		onSubmit(newGroup.id);
	};

	tooltipRender = () => {
		const { error } = this.state;
		if (!error) {
			return;
		}
		return <div>{ error }</div>;
	};

	onFocus = () => {
		this.setState({
			hasError: false,
		});
		this.tooltipRender();
	};

	onChangeInput = (_, value) => {
		this.setState({
			name: value,
		});
		this.tooltipRender();
	};

}

CreateGroupModal.propTypes = {
	closeModal: PropTypes.func,
	courseId: PropTypes.string
};

export default CreateGroupModal;