import { RootState } from "src/redux/reducers";
import { MatchParams } from "src/models/router";
import { Dispatch } from "redux";
import { loadSlide } from "src/actions/slides";
import { connect } from "react-redux";
import Slide, { DispatchFromRedux, PropsFromRedux } from "./Slide";
import { RouteComponentProps, withRouter } from "react-router-dom";

const mapStateToProps = (state: RootState, { match, }: RouteComponentProps<MatchParams>) => {
	const { slides, instructor, } = state;
	const { slidesByCourses, slideLoading, slideError, } = slides;
	const slideId = match.params.slideSlugOrAction.split('_').pop()!;
	const courseId = match.params.courseId.toLowerCase();

	const props: PropsFromRedux = {
		slideId,
		courseId,
		slideLoading,
		slideBlocks: [],
		slideError,
		showHiddenBlocks: !instructor.isStudentMode,
	};

	const coursesSlides = slidesByCourses[courseId];

	if(coursesSlides) {
		props.slideBlocks = coursesSlides[slideId] || [];
	}

	return props;
};

const mapDispatchToProps = (dispatch: Dispatch): DispatchFromRedux => ({
	loadSlide: (courseId: string, slideId: string) => loadSlide(courseId, slideId)(dispatch),
});

export default withRouter(connect(mapStateToProps, mapDispatchToProps)(Slide));
