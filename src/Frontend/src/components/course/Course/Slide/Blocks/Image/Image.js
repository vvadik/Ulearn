import React from "react";

import ImageGallery from 'react-image-gallery';

import classNames from "classnames";
import PropTypes from 'prop-types';

import styles from './Image.less';

import 'react-image-gallery/styles/css/image-gallery.css';

function Image({ imageUrls, className, }) {
	return (
		<div className={ classNames(styles.wrapper, className) }>
			<ImageGallery
				showBullets={ imageUrls.length !== 1 }
				showPlayButton={ false }
				showThumbnails={ false }
				items={ imageUrls.map(url => {
					return { original: url, };
				}) }/>
		</div>
	);
}

Image.propTypes = {
	className: PropTypes.string,
	imageUrls: PropTypes.arrayOf(PropTypes.string),
}

export default Image;
