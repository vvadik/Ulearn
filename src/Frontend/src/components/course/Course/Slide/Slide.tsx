import React from "react";

import { BlocksWrapper, } from "./Blocks";
import BlocksRenderer from "./BlocksRenderer";
import CourseLoader from "src/components/course/Course/CourseLoader";

import { Block, BlockTypes, ShortSlideInfo, SlideType, SpoilerBlock, } from "src/models/slide";

import InstructorReview from "./InstructorReview/InstructorReview.redux";
import { args, } from "./InstructorReview/InstructorReview.story";
import { RouteComponentProps } from "react-router-dom";

export interface SlideContext {
	courseId: string;
	slideId: string;
}

export interface PropsFromRedux extends SlideProps, SlideContext {
	showHiddenBlocks: boolean;
}

interface SlideProps {
	slideBlocks: Block[];
	slideError: string | null;

	slideLoading: boolean;
}

interface SlidePropsWithContext extends SlideProps {
	slideContext: SlideContext;
}

export interface DispatchFromRedux {
	loadSlide: (courseId: string, slideId: string,) => void;
}

export interface Props extends PropsFromRedux, DispatchFromRedux, RouteComponentProps {
	slideInfo: ShortSlideInfo;

	isLti: boolean;
	isReview: boolean;
}

class Slide extends React.Component<Props> {
	componentDidMount(): void {
		const { slideBlocks, } = this.props;
		if(slideBlocks.length === 0) {
			this.loadSlide();
		}
	}

	componentDidUpdate(prevProps: Props): void {
		const { slideBlocks, slideLoading, slideError } = this.props;

		if(prevProps.slideId !== this.props.slideId || slideBlocks.length === 0 && !slideLoading && !slideError) {
			this.loadSlide();
		}
	}

	loadSlide = (): void => {
		const { loadSlide, courseId, slideId, } = this.props;
		loadSlide(courseId, slideId);
	};

	render = (): React.ReactElement => {
		const {
			slideBlocks,
			showHiddenBlocks,
			slideInfo,
			isLti,
			isReview,
			slideError,
			slideLoading,
			slideId,
			courseId,
		} = this.props;
		const slideProps = {
			slideBlocks: JSON.parse(JSON.stringify(slideBlocks)),
			slideError,
			slideLoading,
			slideContext: { slideId, courseId },
		};

		if(isLti && slideInfo.type == SlideType.Exercise) {
			return <LtiExerciseSlide { ...slideProps }/>;
		}

		if(isReview && slideInfo.type == SlideType.Exercise) {
			return <ReviewSlide { ...slideProps }/>;
		}

		if(!showHiddenBlocks) {
			return <StudentModeSlide { ...slideProps } isHiddenSlide={ slideInfo.hide }/>;
		}


		return <DefaultSlide { ...slideProps }/>;
	};
}


const LtiExerciseSlide = ({
	slideBlocks,
	slideError,
}: SlideProps): React.ReactElement => {
	if(slideError) {
		return <p>slideError</p>;
	}

	if(slideBlocks.length === 0) {
		return (<CourseLoader/>);
	}

	const exerciseSlideBlock = slideBlocks.find(sb => sb.$type === BlockTypes.exercise)!;
	return <>{ BlocksRenderer.renderBlocks([exerciseSlideBlock]) }</>;
};

const ReviewSlide: React.FC<SlidePropsWithContext> = ({
	slideBlocks,
	slideError,
}): React.ReactElement => {
	if(slideError) {
		return <p>slideError</p>;
	}

	if(slideBlocks.length === 0) {
		return (<CourseLoader/>);
	}

	const exerciseSlideBlockIndex = slideBlocks.findIndex(sb => sb.$type === BlockTypes.exercise);
	const authorSolution = slideBlocks[slideBlocks.length - 1].$type === BlockTypes.code
		? {
			...slideBlocks[slideBlocks.length - 1],
			hide: false,
		}
		: undefined;
	const formulation = slideBlocks.slice(0, exerciseSlideBlockIndex);
	const exerciseSlideBlock = slideBlocks[exerciseSlideBlockIndex];

	return <InstructorReview
		{ ...args }
		getAntiPlagiarismStatus={ () => Promise.resolve({ suspicionLevel: 'notChecking', suspicionCount: 0, }) }
		authorSolution={ authorSolution
			? BlocksRenderer.renderBlocks([exerciseSlideBlock])
			: undefined }
		formulation={ formulation.length > 0
			? BlocksRenderer.renderBlocks(formulation)
			: undefined }
	/>;
};

const DefaultSlide = ({
	slideBlocks,
	slideError,
	slideContext,
}: SlidePropsWithContext): React.ReactElement => {
	if(slideError) {
		return <p>slideError</p>;
	}

	if(slideBlocks.length === 0) {
		return (<CourseLoader/>);
	}

	return <>{ BlocksRenderer.renderBlocks(slideBlocks, slideContext,) }</>;
};

interface StudentModeProps extends SlidePropsWithContext {
	isHiddenSlide?: boolean;
}

const StudentModeSlide = ({
	slideBlocks,
	slideError,
	isHiddenSlide,
	slideContext,
}: StudentModeProps): React.ReactElement => {
	if(slideError) {
		return <p>slideError</p>;
	}

	if(slideBlocks.length === 0) {
		return (<CourseLoader/>);
	}


	if(isHiddenSlide) {
		return renderHiddenSlide();
	}

	const slideBlocksForStudent = getBlocksForStudent(slideBlocks);


	if(slideBlocksForStudent.length === 0) {
		return renderHiddenSlide();
	}
	return <>{ BlocksRenderer.renderBlocks(slideBlocksForStudent, slideContext,) }</>;


	function getBlocksForStudent(blocks: Block[]): Block[] {
		const slideBlocksForStudent = [];

		for (const block of blocks) {
			if(block.hide) {
				continue;
			}
			if(block.$type === BlockTypes.spoiler) {
				const spoilerBlock = { ...block } as SpoilerBlock;
				spoilerBlock.blocks = getBlocksForStudent(spoilerBlock.blocks);
			}

			slideBlocksForStudent.push(block);
		}

		return slideBlocksForStudent;
	}

	function renderHiddenSlide(): React.ReactElement {
		return (
			<BlocksWrapper>
				<p>Студенты не увидят этот слайд в навигации</p>
			</BlocksWrapper>
		);
	}
};

export default Slide;
