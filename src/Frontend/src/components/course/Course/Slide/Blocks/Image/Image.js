import React from "react";

import ImageGallery from 'react-image-gallery';

import classNames from "classnames";
import PropTypes from 'prop-types';

import styles from './Image.less';

import 'react-image-gallery/styles/css/image-gallery.css';

class Image extends React.Component {
	constructor(props) {
		super(props);

		this.state = {
			fullscreen: false,
		}
	}

	render() {
		const { imageUrls, className, } = this.props;
		const { fullscreen, } = this.state;

		return (
			<div className={ classNames(styles.wrapper, className) }>
				<ImageGallery
					additionalClass={ classNames(styles.imageWrapper, { [styles.open]: fullscreen }) }
					useBrowserFullscreen={ false }
					showBullets={ imageUrls.length !== 1 }
					showPlayButton={ false }
					showThumbnails={ false }
					onScreenChange={ this.onScreenChange }
					items={ imageUrls.map(url => {
						return { original: url, };
					}) }/>
			</div>
		);
	}


	onScreenChange = (isFullScreen) => {
		this.setState({
			fullscreen: isFullScreen,
		})
	}
}

Image.propTypes = {
	className: PropTypes.string,
	imageUrls: PropTypes.arrayOf(PropTypes.string),
}

export default Image;
