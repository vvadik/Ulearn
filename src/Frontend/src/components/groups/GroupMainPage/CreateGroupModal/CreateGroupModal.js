import { Component } from 'react';
import PropTypes from "prop-types";
import Modal from '@skbkontur/react-ui/components/Modal/Modal';
import Input from '@skbkontur/react-ui/components/Input/Input';
import Button from '@skbkontur/react-ui/components/Button/Button';
import Gapped from '@skbkontur/react-ui/components/Gapped';
import React from 'react';
import api from "../../../../api/index";

import './style.less';

class CreateGroupModal extends Component {

	state = {
		name: null,
		hasError: false,
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
								<Input placeholder="КН-201 УрФУ 2017"
									   required
									   value={name || ''}
									   error={hasError}
									   onChange={this.onChangeInput}
								/>
							</Gapped>
						</label>
						<p>
							Студенты увидят название группы, поэтому постарайтесь сделать его понятным.
							Пример хорошего названия группы: «КН-201 УрФУ 2017», пример плохого:
							«Моя группа 2».
						</p>
					</Modal.Body>
					<Modal.Footer>
						<Button use="primary" size="medium" type="submit">Создать</Button>
					</Modal.Footer>
				</form>
			</Modal>
		)
	}

	onChangeInput = (_, value) => {
		this.setState({
			name: value,
			hasError: false,
		});
	};

	onSubmit = (e) => {
		const { courseId } = this.props;
		const { name } = this.state;
		e.preventDefault();
		if (!name) {
			this.setState({hasError: true});
			return;
		}
		api.groups.createGroup(courseId, name);
	}
}

CreateGroupModal.propTypes = {
	closeModal: PropTypes.func,
	courseId: PropTypes.string
};

export default CreateGroupModal;