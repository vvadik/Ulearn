import React from 'react';

import YouTube, { Options } from 'react-youtube';
import { BlocksWrapper, Text, } from "src/components/course/Course/Slide/Blocks";
import { ArrowChevronUp, ArrowChevronDown, } from "@skbkontur/react-icons";
import { Link } from "@skbkontur/react-ui";

import classNames from 'classnames';
import { Cookies, withCookies } from 'react-cookie';

import styles from './Video.less';

const videoCookieName = 'youtube-video-rate';

interface AnnotationFragment {
	text: string,
	offset: string,
}

interface Annotation {
	text: string,
	fragments: AnnotationFragment[],
}

interface Props {
	autoplay: boolean,
	annotation: Annotation,
	openAnnotation: boolean,
	googleDocLink: string,
	annotationWithoutBottomPaddings: boolean,
	videoId: string,
	className: string,
	containerClassName: string,
	hide: boolean,
	cookies: Cookies,
}

interface State {
	showedAnnotation: boolean,
}

class Video extends React.Component<Props, State> {
	private ytPlayer: YT.Player | null = null;

	constructor(props: Props) {
		super(props);

		const { openAnnotation, } = this.props;

		this.state = {
			showedAnnotation: openAnnotation,
		};
	}

	componentDidUpdate(prevProps: Props) {
		if(prevProps.openAnnotation !== this.props.openAnnotation) {
			this.setState({ showedAnnotation: this.props.openAnnotation });
		}

		if(this.ytPlayer) {
			const newVideoRate = parseFloat(this.props.cookies.get(videoCookieName) || '1');

			this.ytPlayer.setPlaybackRate(newVideoRate);
		}
	}

	render() {
		const { videoId, className, containerClassName, autoplay, googleDocLink, hide, } = this.props;

		const containerClassNames = classNames(styles.videoContainer, { [containerClassName]: containerClassName });
		const frameClassNames = classNames(styles.frame, { [className]: className });

		const opts: Options = {
			playerVars: {
				autoplay: autoplay ? 1 : 0,
				/* Disable related videos */
				rel: 0,
			},
		};

		return (
			<React.Fragment>
				<YouTube
					containerClassName={ containerClassNames }
					className={ frameClassNames }
					videoId={ videoId }
					opts={ opts }
					onReady={ this.onReady }
					onPlaybackRateChange={ this.onPlaybackRateChange }
				/>
				{ hide && <BlocksWrapper hide isBlock withoutBottomPaddings={ !!googleDocLink }>
					<Text className={ styles.withoutBottomMargins }>
						<p>Видео выше скрыто</p>
					</Text>
				</BlocksWrapper> }
				{ googleDocLink && this.renderAnnotation() }
			</React.Fragment>
		);
	}


	onReady = (event: { target: YT.Player }): void => {
		const { cookies } = this.props;

		this.ytPlayer = event.target;
		const rate = parseFloat(cookies.get(videoCookieName) || '1');
		this.ytPlayer.setPlaybackRate(rate);
	};

	onPlaybackRateChange = (event: { target: YT.Player, data: number }) => {
		const { cookies } = this.props;

		cookies.set(videoCookieName, event.data);
	};

	renderAnnotation = () => {
		const { showedAnnotation } = this.state;
		const { annotation, googleDocLink, hide, annotationWithoutBottomPaddings } = this.props;

		return (
			<BlocksWrapper
				withoutEyeHint
				withoutBottomPaddings={ annotationWithoutBottomPaddings }
				hide={ hide }
				isBlock
				className={ styles.withoutBottomMargins }>
				<Text>
					{
						annotation
							? this.renderAnnotationContent(showedAnnotation, annotation, googleDocLink)
							: <p>
								Помогите написать <Link target="_blank" href={ googleDocLink }>текстовое
								содержание</Link> этого видео.
							</p>
					}
				</Text>
			</BlocksWrapper>
		);
	};

	renderAnnotationContent = (showedAnnotation: boolean, annotation: Annotation, googleDocLink: string) => {
		const titleClassName = showedAnnotation ? styles.opened : styles.closed;

		return (
			<React.Fragment>
				<h3 className={ classNames(styles.annotationTitle, titleClassName) } onClick={ this.toggleAnnotation }>
					Содержание видео
					<span className={ styles.annotationArrow }>
						{ showedAnnotation
							? <ArrowChevronUp/>
							: <ArrowChevronDown/> }
					</span>
				</h3>
				{ showedAnnotation &&
				<React.Fragment>
					<p>{ annotation.text }</p>
					{ annotation.fragments.map(({ text, offset }) => {
						const [hours, minutes, seconds] = offset.split(':');
						const [hoursAsInt, minutesAsInt, secondsAsInt] = [hours, minutes, seconds].map(
							t => Number.parseInt(t));
						const timeInSeconds = hoursAsInt * 60 * 60 + minutesAsInt * 60 + secondsAsInt;
						return (
							<p key={ offset }>
								<Link onClick={ () => this.setVideoTime(timeInSeconds) }>
									{ hoursAsInt > 0 && `${ hours }:` }
									{ `${ minutes }:` }
									{ seconds }
								</Link>
								{ ` — ${ text }` }
							</p>
						);
					})
					}
					<p>
						Ошибка в содержании? <Link target="_blank" href={ googleDocLink }>Предложите
						исправление!</Link>
					</p>
				</React.Fragment>
				}
			</React.Fragment>
		);
	};

	toggleAnnotation = () => {
		this.setState({
			showedAnnotation: !this.state.showedAnnotation,
		});
	};

	setVideoTime = (seconds: number) => {
		this.ytPlayer?.seekTo(seconds, true);
	};
}

export default withCookies(Video);
