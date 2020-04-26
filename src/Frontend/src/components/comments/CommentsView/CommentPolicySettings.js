import React, { Component } from 'react';
import { Modal, Toggle, RadioGroup, Radio, Gapped, Button } from "ui";

class CommentPolicySettings extends Component {
	render() {
		const {canComment, handleToggleCanComment, handleSaveSettings, handleCloseModal} = this.props;
		return (
			<Modal onClose={() => handleCloseModal()}>
				<Modal.Header>Настройки комментариев курса</Modal.Header>
				<Modal.Body>
					<div>
						<Toggle
							checked={canComment}
							onChange={() => handleToggleCanComment()}
						/>{' '}
						Студенты {canComment ? '' : 'не'} могут оставлять комментарии
					</div>
					<div>
						<RadioGroup name="moderation" defaultValue="3">
							<Gapped vertical gap={10}>
								<Radio value="1">Премодерация</Radio>
								<Radio checked value="3">Постмодерация</Radio>
							</Gapped>
						</RadioGroup>
					</div>
				</Modal.Body>
				<Modal.Footer panel={true}>
					<Button use="primary" onClick={handleSaveSettings}>Сохранить</Button>
					<Button onClick={() => handleCloseModal()}>Отменить</Button>
				</Modal.Footer>
			</Modal>
		)
	}
}

export default CommentPolicySettings;