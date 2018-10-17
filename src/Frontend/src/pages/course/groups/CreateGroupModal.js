import { Component } from 'react';
import PropTypes from "prop-types";
import Modal from '@skbkontur/react-ui/components/Modal/Modal';
import Input from '@skbkontur/react-ui/components/Input/Input';
import Button from '@skbkontur/react-ui/components/Button/Button';
import React from 'react';

class CreateGroupModal extends Component {
	render() {
		return (
			<Modal onClose={this.props.closeModal} width="40%">
				<Modal.Header>Создание группы</Modal.Header>
				<Modal.Body>
					<h2>Название группы</h2>
					<Input placeholder="КН-201 УрФУ 2017"/>
					<p>
						Студенты увидят название группы, поэтому постарайтесь сделать его понятным.
						Пример хорошего названия группы: «КН-201 УрФУ 2017», пример плохого:
						«Моя группа 2».
					</p>
				</Modal.Body>
				<Modal.Footer>
					<Button use="primary" size="medium" onClick={this.props.closeModal}>Создать</Button>
				</Modal.Footer>
			</Modal>
		)
	}
}

CreateGroupModal.propTypes = {
	closeModal: PropTypes.func
};

export default CreateGroupModal;