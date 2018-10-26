import { Component } from 'react';
import PropTypes from "prop-types";
import Modal from '@skbkontur/react-ui/components/Modal/Modal';
import Input from '@skbkontur/react-ui/components/Input/Input';
import Button from '@skbkontur/react-ui/components/Button/Button';
import React from 'react';
import api from "../../../api/index";

class CreateGroupModal extends Component {

	state = {
		name: null
	};

	render() {
		return (
			<Modal onClose={this.props.closeModal} width="40%">
				<Modal.Header className="modal-header"></Modal.Header>
				<Modal.Body>
					<label className="modal-label">Название группы</label>
					<Input width="63%" placeholder="КН-201 УрФУ 2017" onChange={this.onChangeInput}/>
					<p>
						Студенты увидят название группы, поэтому постарайтесь сделать его понятным.
						Пример хорошего названия группы: «КН-201 УрФУ 2017», пример плохого:
						«Моя группа 2».
					</p>
				</Modal.Body>
				<Modal.Footer>
					<Button use="primary" size="medium" onClick={this.onClick}>Создать</Button>
				</Modal.Footer>
			</Modal>
		)
	}

	onChangeInput = (event) => {
		const name = event.target.value;
		this.setState({
			name: name
		});
	};

	onClick = () => {
		const { courseId } = this.props;
		const { name } = this.state;
		api.groups.createGroup(courseId, name);
	}
}

CreateGroupModal.propTypes = {
	closeModal: PropTypes.func,
	courseId: PropTypes.string
};

export default CreateGroupModal;