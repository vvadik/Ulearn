import React, {Component} from 'react';
import PropTypes from "prop-types";
import Modal from '@skbkontur/react-ui/components/Modal/Modal';
import Button from '@skbkontur/react-ui/components/Button/Button';

import Textarea from '@skbkontur/react-ui/components/Textarea/Textarea';

//import styles from "./copyGroupModal.less";

class UsersRolesPage extends Component {
	constructor(props) {
		super(props);

		this.state = {
			comment: null,
			modalOpened: false,
		};
	}


	render() {
		const {onClose, onSubmit} = this.props;
		const {comment} = this.state;

		return (
			<Modal onClose={onClose}>
				<Modal.Header>
					Оставьте комментарий
				</Modal.Header>
				<Modal.Body>
					<Textarea
						onChange={(_, value) => this.setState({comment: value})}
						autoResize
						placeholder="Оставьте комментарий"
					/>
				</Modal.Body>
				<Modal.Footer>
					<Button use='primary' onClick={() => { onSubmit(comment) }}>
						Сохранить
					</Button>
				</Modal.Footer>
			</Modal>
		)
	}
}

UsersRolesPage.propTypes = {
	onSubmit: PropTypes.func,
	onClose: PropTypes.func,
};

export default UsersRolesPage;