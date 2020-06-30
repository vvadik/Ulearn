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
			currentImage: images[0],
			anyImageLoaded: false,
			images,
		}
	}

	componentDidMount() {
		window.addEventListener('resize', this.resizePictures);
	}

	componentWillUnmount() {
		window.removeEventListener('resize', this.resizePictures);
	}

	resizePictures = () => {
		const { images, anyImageLoaded, fullscreen, } = this.state;

		if(anyImageLoaded) {
			this.setSizeForErroredImages(images, fullscreen);
		}
	}

	get failedImagesCount() {
		const { images, } = this.state;

		return images.filter(i => i.error).length;
	}

	render() {
		const { imageUrls, className, } = this.props;
		const { fullscreen, currentImage, anyImageLoaded, } = this.state;

		const wrapperClass = classNames(
			styles.wrapper,
			{ [styles.error]: currentImage.error },
			{ [styles.loading]: !anyImageLoaded },
			className
		);

		return (
			<div className={ wrapperClass } onClick={ this.onClick } ref={ (ref) => this.wrapper = ref }>
				<ImageGallery
					ref={ (ref) => this.gallery = ref }
					onImageLoad={ this.onImageLoad }
					onImageError={ this.onImageError }
					onBeforeSlide={ this.onBeforeSlide }
					additionalClass={ classNames(styles.imageWrapper, { [styles.open]: fullscreen }) }
					useBrowserFullscreen={ false }
					showBullets={ imageUrls.length !== 1 }
					showFullscreenButton={ this.shouldShowFullscreenButton(currentImage) || fullscreen }
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
		const { anyImageLoaded, } = this.state;
		const img = event.target;

		if(!anyImageLoaded) {
			this.setState({
				anyImageLoaded: true,
			});
		}

		this.addAttributeToImage(img);
	}

	onImageError = (event) => {
		const img = event.target;
		const { imageUrls, } = this.props;

		if(this.failedImagesCount === imageUrls.length - 1) {
			this.setState({
				anyImageLoaded: true,
			});
		}

		this.addAttributeToImage(img, true);
	}

	addAttributeToImage = (img, error) => {
		const { imageUrls, } = this.props;
		const { images, anyImageLoaded, } = this.state;

		const src = img.getAttribute("src");
		const index = imageUrls.findIndex(url => url === src);
		const newImages = [...images];

		newImages[index].img = img;
		newImages[index].error = error;

		if(error && anyImageLoaded) {
			this.setSizeForErroredImages(newImages);
		}

		this.setState({
			images: newImages,
		})
	}

	setSizeForErroredImages = (images, fullscreen) => {
		const loadedImage = images.find(({ error }) => !error);

		if(loadedImage) {
			const aspectRatio = loadedImage.img.naturalHeight / loadedImage.img.naturalWidth;
			const width = Math.min(loadedImage.img.naturalWidth, fullscreen ? window.innerWidth : this.slideWidth);
			const height = width * aspectRatio;

			for (const { img } of images.filter(({ error }) => error)) {
				img.style.width = `${ width }px`;
				img.style.height = `${ height }px`;
			}
		}
	}

	get slideWidth() {
		if(this.wrapper) {
			const slideNode = this.wrapper.parentNode;
			const slideStyle = getComputedStyle(slideNode);
			return parseFloat(slideStyle.width) - parseFloat(slideStyle.paddingLeft) - parseFloat(slideStyle.paddingRight);
		}
		return undefined;
	}

	shouldShowFullscreenButton = ({ img, error, }) => {
		if(this.wrapper && img) {
			return !error && img.width >= this.slideWidth;
		}

		return false;
	}

	onScreenChange = (isFullScreen) => {
		this.setSizeForErroredImages(this.state.images, isFullScreen);

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
