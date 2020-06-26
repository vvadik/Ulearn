import React from "react";

import ImageGallery from 'react-image-gallery';

import classNames from "classnames";
import PropTypes from 'prop-types';

import styles from './Image.less';

import 'react-image-gallery/styles/css/image-gallery.css';

class Image extends React.Component {
	constructor(props) {
		super(props);

		const images = this.props.imageUrls.map((_, index) => {
			return { index };
		});

		this.state = {
			fullscreen: false,
			showFullscreenButton: true,
			currentImage: images[0],
			images,
			maxWidth: null,
			maxHeight: null,
		}
	}

	get width() {
		const { maxWidth } = this.state;

		if(!maxWidth) {
			return null;
		}

		return maxWidth + 'px';
	}

	get height() {
		const { maxHeight } = this.state;

		if(!maxHeight) {
			return null;
		}

		return maxHeight + 'px';
	}

	get failedImagesCount() {
		const { images, } = this.state;

		return images.filter(i => i.error).length;
	}

	render() {
		const { imageUrls, className, } = this.props;
		const { fullscreen, showFullscreenButton, currentImage, } = this.state;


		const wrapperStyle = currentImage.img //first img loaded
			? { width: this.width, }
			: { opacity: 0, height: 0, }; //prevent showing extended img

		return (
			<div className={ classNames(styles.wrapper, className) } style={ wrapperStyle } onClick={ this.onClick }>
				<ImageGallery
					ref={ (ref) => this.gallery = ref }
					onImageLoad={ this.onImageLoad }
					onImageError={ this.onImageError }
					onBeforeSlide={ this.onBeforeSlide }
					additionalClass={ classNames(styles.imageWrapper, { [styles.open]: fullscreen }) }
					useBrowserFullscreen={ false }
					showBullets={ imageUrls.length !== 1 }
					showFullscreenButton={ showFullscreenButton && (!currentImage.error || fullscreen) }
					showPlayButton={ false }
					showThumbnails={ false }
					onScreenChange={ this.onScreenChange }
					items={ imageUrls.map(url => {
						return {
							original: url,
							originalClass: styles.img,
						};
					}) }/>
			</div>
		);
	}

	onImageLoad = (event) => {
		const img = event.target;
		const { maxWidth, } = this.state;

		if(!maxWidth) {
			if(img.width > img.naturalWidth) { //set all img width to real size of first img, if not bigger than slide width
				this.setState({
					maxWidth: img.naturalWidth,
					maxHeight: img.naturalHeight,
					showFullscreenButton: false,
				});
			} else {
				this.setState({
					maxWidth: img.width,
					maxHeight: img.height,
				});
			}
		}

		this.addAttributeToImage(img);
	}

	onImageError = (event) => {
		const img = event.target;
		const { imageUrls, } = this.props;
		const { maxHeight, } = this.state;

		if(this.failedImagesCount === imageUrls.length - 1) {
			this.setState({
				maxWidth: img.width,
				showFullscreenButton: false,
			});
		}

		if(maxHeight) {
			img.style.height = this.height;
			img.style.width = this.width;
		}

		this.addAttributeToImage(img, true);
	}

	addAttributeToImage = (img, error) => {
		const { imageUrls, } = this.props;
		const { images, } = this.state;

		const src = img.getAttribute("src");
		const index = imageUrls.findIndex(url => url === src);
		const newImages = [...images];
		newImages[index].img = img;

		if(error) {
			newImages[index].error = true;
			if(index === 0) {
				this.setState({
					currentImageError: true,
				});
			}
		}

		this.setState({
			images: newImages,
		})
	}

	onScreenChange = (isFullScreen) => {
		this.setState({
			fullscreen: isFullScreen,
		})
	}

	onBeforeSlide = (index) => {
		const { images, } = this.state;
		const currentImage = images[index];

		this.setState({
			currentImage,
		});
	}

	onClick = (e) => {
		if(this.gallery.imageGallery.current === e.target && this.state.fullscreen) { //if root component clicked (in fullscreen its background)
			this.gallery.exitFullScreen();
		}
	}
}

Image.propTypes = {
	className: PropTypes.string,
	imageUrls: PropTypes.arrayOf(PropTypes.string),
}

export default Image;
