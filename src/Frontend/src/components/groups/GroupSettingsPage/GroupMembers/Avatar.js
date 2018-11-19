import React, {Component} from "react";
import PropTypes from "prop-types";
import colorHash from '../../../../utils/colorHash';

import './style.less';

export default class Avatar extends Component {

	render() {
		const { user } = this.props;
		const imageUrl = user.avatar_url;
		const userName = user.visible_name;
		const userFirstLetter = userName[userName.search(/[a-zA-Zа-яА-Я]/)].toUpperCase();

		return (
			<div>
				{ imageUrl ? this.renderImage(imageUrl) : this.renderCircle(userName, userFirstLetter) }
			</div>
			)
	}

	renderImage(url) {
		return (
			<img
			alt="фото"
			className="teacher-photo"
			src={url}
			/>
		);
	}

	renderCircle(name, letter) {
		let divStyle = {
			backgroundColor: `${colorHash(name)}`,
		};

		return (
			<div style={divStyle} className="teacher-photo color-avatar">{ letter }</div>
		)
	}
}
