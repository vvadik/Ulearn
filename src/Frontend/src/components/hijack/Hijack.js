import React from "react";

import { Gapped, Tooltip } from "@skbkontur/react-ui";
import { Logout, } from "@skbkontur/react-icons";

import { withCookies } from 'react-cookie';

import PropTypes from 'prop-types';

import styles from './Hijack.less';

const hijackCookieName = 'ulearn.auth.hijack';
const hijackReturnControllerPath = '/Account/ReturnHijack';

function Hijack({ allCookies, name, }) {
	const isHijacked = allCookies[hijackCookieName] ? true : false;

	return (
		<React.Fragment>
			{ isHijacked &&
			<div className={ styles.wrapper } onClick={ returnHijak }>
				<Tooltip trigger='hover&focus' pos={ 'bottom center' } render={ renderTooltip }>
					<Gapped gap={ 5 }>
						<Logout size={ 20 }/>
					</Gapped>
				</Tooltip>
			</div>
			}
		</React.Fragment>
	);

	function renderTooltip() {
		return (
			<span>Вы работаете под пользователем { name }. Нажмите, чтобы вернуться</span>
		);
	}

	function returnHijak() {
		fetch(hijackReturnControllerPath, { method: 'POST' })
			.then(r => {
				if(r.redirected) {
					window.location.href = r.url;// reloading page to url in response location
				}
			});
	}
}

Hijack.propTypes = {
	name: PropTypes.string,
}

export default withCookies(Hijack);
