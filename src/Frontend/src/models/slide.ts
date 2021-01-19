import BlockTypes from "../components/course/Course/Slide/blockTypes";
import { ReactNode } from "react";
import { Language } from "../consts/languages";
import { SubmissionInfoRedux } from "./reduxState";
import { AttemptsStatistics } from "./exercise";

interface ShortSlideInfo {
	id: string;
	title: string;
	hide: boolean | undefined;
	slug: string; // Человекочитаемый фрагмент url для слайда
	maxScore: number;
	scoringGroup: string | null;
	type: SlideType;
	apiUrl: string;
	questionsCount: number; // Количество вопросов в quiz
	gitEditLink: string | undefined;
}

enum SlideType {
	Lesson = "lesson",
	Quiz = "quiz",
	Exercise = "exercise",
	Flashcards = "flashcards",
	CourseFlashcards = "courseFlashcards",
	PreviewFlashcards = "previewFlashcards",
}

interface Block<T extends BlockTypes> {
	$type: T,
	key: number,
	hide: boolean,
}

interface SpoilerBlock extends Block<BlockTypes.spoiler> {
	blocks: Block<BlockTypes>[],
	blocksId: string,
	isPreviousBlockHidden: boolean,
	renderedBlocks: ReactNode[],
}

interface TexBlock extends Block<BlockTypes.tex> {
	content: string,
	lines: string[],
}

interface VideoBlock extends Block<BlockTypes.video> {
	autoplay: boolean,
	openAnnotation: boolean,
	annotationWithoutBottomPaddings: boolean,
}

interface ExerciseBlock extends Block<BlockTypes.exercise> {
	slideId: string,
	courseId: string,
	forceInitialCode: boolean,
	maxScore: number,
}

interface ExerciseBlockProps {
	languages: Language[],
	languageNames: EnumDictionary<Language, string> | null,
	renderedHints: string[],
	exerciseInitialCode: string,
	hideSolutions: boolean,
	expectedOutput: string,
	submissions: SubmissionInfoRedux[],
	attemptsStatistics: AttemptsStatistics
}

export { ShortSlideInfo, SlideType, Block, SpoilerBlock, TexBlock, VideoBlock, ExerciseBlock, ExerciseBlockProps, };
