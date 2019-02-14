import React, {Component} from "react";
import PropTypes from "prop-types";
import {CopyToClipboard} from 'react-copy-to-clipboard';
import api from "../../../../../api";
import Toggle from "@skbkontur/react-ui/components/Toggle/Toggle";
import Button from "@skbkontur/react-ui/components/Button/Button";
import Toast from "@skbkontur/react-ui/components/Toast/Toast";
import LinkIcon from "@skbkontur/react-icons/Link";
import Input from "@skbkontur/react-ui/components/Input/Input";

import styles from './inviteBlock.less';

class InviteBlock extends Component {
	constructor(props) {
		super(props);
		this.state = {
			inviteLinkEnabled: props.group.is_invite_link_enabled
		};
	}

	render() {
		const inviteLinkEnabled = this.state.inviteLinkEnabled;

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
			<div className={styles["invite-link"]}>
				<div className={styles["invite-link-text"]}>
					<CopyToClipboard text={`${window.location.origin}/Account/JoinGroup?hash=${group.invite_hash}`}>
						<Button use="link" icon={<LinkIcon />} onClick={() => Toast.push('Ссылка скопирована')}>
							Скопировать ссылку
						</Button>
					</CopyToClipboard>
				</div>
				<div className={styles["invite-link-input"]}>
					<Input
						type="text"
						value={`${window.location.origin}/Account/JoinGroup?hash=${group.invite_hash}`}
						readOnly
						selectAllOnFocus
						width="65%"
					/>
				</div>
			</div>
		)
	}

	onToggle = () => {
		const { group } = this.props;
		const inviteLinkEnabled = this.state.inviteLinkEnabled;

		this.setState ({
			inviteLinkEnabled: !inviteLinkEnabled,
		});
		this.props.group.is_invite_link_enabled = !inviteLinkEnabled;

		api.groups.saveGroupSettings(group.id, {'is_invite_link_enabled': !inviteLinkEnabled})
			.catch(console.error);
	};
}

InviteBlock.propTypes = {
	group: PropTypes.object,
};

export default InviteBlock;