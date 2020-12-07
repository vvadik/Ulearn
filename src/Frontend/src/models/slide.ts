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

export { ShortSlideInfo, SlideType }
