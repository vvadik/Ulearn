import React, {Component} from "react";
import PropTypes from "prop-types";
import {CopyToClipboard} from 'react-copy-to-clipboard';
import api from "../../../../../api";
import Toggle from "@skbkontur/react-ui/components/Toggle/Toggle";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Toast from "@skbkontur/react-ui/components/Toast/Toast";
import Gapped from "@skbkontur/react-ui/components/Gapped/Gapped";
import Icon from "@skbkontur/react-icons";
import Input from "@skbkontur/react-ui/components/Input/Input";

import styles from './style.less';

class InviteBlock extends Component {

	state = {
		copied: false,
	};

	render() {
		const { group } = this.props;
		const inviteLink = group.is_invite_link_enabled || false;

		return (
			<label>
				<div className={styles["toggle-invite"]}>
					<Toggle
						checked={inviteLink}
						onChange={this.onToggleHash}
						color="default">
					</Toggle>
					<span className={styles["toggle-invite-text"]}>
						Ссылка для вступления в группу { inviteLink ? ' включена' : ' выключена' }
					</span>
				</div>
				{ inviteLink && this.renderInvite() }
			</label>
		)
	}

	renderInvite() {
		const { group } = this.props;

		return (
			<div className={styles["inviteLink-block"]}>
				<div className={styles["inviteLink-text"]}>
					<CopyToClipboard
						text={`https://ulearn.me/Account/JoinGroup?hash=${group.invite_hash}`}
						onCopy={() => this.setState({copied: true})}>
						<Button use="link" onClick={() => Toast.push('Ссылка скопирована')}>
							<Gapped gap={5}>
								<Icon name="Link" />
								Скопировать ссылку
							</Gapped>
						</Button>
					</CopyToClipboard>
				</div>
				<div className={styles["inviteLink-input"]}>
					<Input
						type="text"
						value={`https://ulearn.me/Account/JoinGroup?hash=${group.invite_hash}`}
						readOnly
						selectAllOnFocus
					/>
				</div>
			</div>
		)
	}

	onToggleHash = () => {
		const { group, onChangeSettings } = this.props;
		const inviteLink = group.is_invite_link_enabled || false;
		const field = 'is_invite_link_enabled';
		const updatedField = {[field]: !inviteLink};

		this.setState({copied: false});
		onChangeSettings(field, !inviteLink);

		api.groups.saveGroupSettings(group.id, updatedField)
			.then(response => response)
			.catch(console.error);
	};
}

InviteBlock.propTypes = {
	group: PropTypes.object,
	onChangeSettings: PropTypes.func,
};

export default InviteBlock;