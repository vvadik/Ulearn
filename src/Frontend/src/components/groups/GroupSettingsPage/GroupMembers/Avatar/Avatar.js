import React, {Component} from "react";
import PropTypes from "prop-types";
import colorHash from '../../../../../utils/colorHash';

import styles from './style.less';

class Avatar extends Component {

	render() {
		const { user, size } = this.props;
		const imageUrl = user.avatar_url;
		const userName = user.visible_name;
		const userFirstLetter = userName[userName.search(/[a-zA-Zа-яА-Я]/)].toUpperCase();
		let className = `${styles["photo-avatar"]} ${size}`;

		return (
			<React.Fragment>
				{ imageUrl ? this.renderImage(imageUrl, className) : this.renderCircle(userName, userFirstLetter, className) }
			</React.Fragment>
			)
	}

	renderImage(url, className) {
		return (
			<img
			alt="фото"
			className={className}
			src={url}
			/>
		);
	}

	renderCircle(name, letter, className) {
		let divStyle = {
			backgroundColor: `${colorHash(name)}`,
		};

		return (
			<div style={divStyle} className={ `${className} ${styles["color-avatar"]}` }>{ letter }</div>
		)
	}
}

Avatar.propTypes = {
	size: PropTypes.string,
	user: PropTypes.object,
};

export default Avatar;
