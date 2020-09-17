import React from "react";
import PropTypes from "prop-types";
import { Button, Modal } from 'ui';


function NotificationModal({ text, title, onClose, width, }) {
	return (
		<Modal onClose={ onClose } width={ window.matchMedia(`(min-width: ${width}px)`).matches ? width : undefined }>
			<Modal.Header>
				{ title }
			</Modal.Header>
			<Modal.Body>
				{ text }
			</Modal.Body>
			<Modal.Footer panel={ true }>
				<Button onClick={ onClose }>Закрыть</Button>
			</Modal.Footer>
		</Modal>

	);
}

NotificationModal.propTypes = {
	text: PropTypes.object,
	title: PropTypes.object,
	onClose: PropTypes.func,
	width: PropTypes.number,
}

export default NotificationModal;
