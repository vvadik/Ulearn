import React, {Component} from "react";
import PropTypes from "prop-types";
import {CopyToClipboard} from 'react-copy-to-clipboard';
import api from "../../../../../api";
import Toggle from "@skbkontur/react-ui/components/Toggle/Toggle";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Toast from "@skbkontur/react-ui/components/Toast/Toast";
import LinkIcon from "@skbkontur/react-icons/Link";
import Input from "@skbkontur/react-ui/components/Input/Input";

import styles from './style.less';

class InviteBlock extends Component {

	render() {
		const { group } = this.props;
		const inviteLinkEnabled = group.is_invite_link_enabled || false;

		return (
			<div className={styles["toggle-invite"]}>
				<label>
					<Toggle
						checked={inviteLinkEnabled}
						onChange={this.onToggle}>
					</Toggle>
					<span className={styles["toggle-invite-text"]}>
						Ссылка для вступления в группу { inviteLinkEnabled ? ' включена' : ' выключена' }
					</span>
				</label>
				{ inviteLinkEnabled && this.renderInvite() }
			</div>
		)
	}

	renderInvite() {
		const { group } = this.props;

		return (
			<div className={styles["inviteLink-block"]}>
				<div className={styles["inviteLink-text"]}>
					<CopyToClipboard text={`https://ulearn.me/Account/JoinGroup?hash=${group.invite_hash}`}>
						<Button use="link" icon={<LinkIcon />} onClick={() => Toast.push('Ссылка скопирована')}>
							Скопировать ссылку
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

	onToggle = () => {
		const { group } = this.props;
		const inviteLinkEnabled = group.is_invite_link_enabled;

		api.groups.saveGroupSettings(group.id, {'is_invite_link_enabled': !inviteLinkEnabled})
			.catch(console.error);
	};
}

InviteBlock.propTypes = {
	group: PropTypes.object,
};

export default InviteBlock;