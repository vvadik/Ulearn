import { getSlideBlocks } from "src/api/slides";
import {
	SLIDE_LOAD_START,
	SLIDE_LOAD_SUCCESS,
	SLIDE_LOAD_FAIL,
	SLIDES_SLIDE_READY,
	SlideAction,
} from "src/actions/slides.types";
import { Block, BlockTypes } from "src/models/slide";
import { Dispatch } from "redux";

export const loadSlideStartAction = (): SlideAction => ({
	type: SLIDE_LOAD_START,
});

export const loadSlideSuccessAction = (courseId: string, slideId: string,
	slideBlocks: Block<BlockTypes>[]
): SlideAction => ({
	type: SLIDE_LOAD_SUCCESS,
	courseId,
	slideId,
	slideBlocks,
});

export const loadSlideFailAction = (error: string): SlideAction => ({
	type: SLIDE_LOAD_FAIL,
	error,
});

export const slideReadyAction = (isSlideReady: boolean): SlideAction => ({
	type: SLIDES_SLIDE_READY,
	isSlideReady,
});

export const loadSlide = (courseId: string, slideId: string): (dispatch: Dispatch) => void => {
	courseId = courseId.toLowerCase();

	return (dispatch: Dispatch) => {
		dispatch(loadSlideStartAction());

		getSlideBlocks(courseId, slideId)
			.then(slideBlocks => {
				dispatch(loadSlideSuccessAction(courseId, slideId, slideBlocks));
			})
			.catch(err => {
				dispatch(loadSlideFailAction(err));
			});
	};
};

export const setSlideReady = (isSlideReady: boolean) => {
	return (dispatch: Dispatch): void => {
		dispatch(slideReadyAction(isSlideReady));
	};
};
