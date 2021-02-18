import { SlideType } from 'src/models/slide';

export interface MenuItem<T extends SlideType> {
	type: T;

	id: string;
	title: string;
	url: string;
	score: number;
	maxScore: number;

	isActive: boolean;
	visited: boolean;
	hide?: boolean;
}

export interface QuizMenuItem extends MenuItem<SlideType.Quiz> {
	questionsCount: number;
}

export interface Progress {
	current: number;
	max: number;
}

export interface CourseMenuItem {
	id: string;
	title: string;
	progress: Progress;
	isActive: boolean;
	isNotPublished?: boolean;
	publicationDate?: string;
	onClick?: (id: string) => void;
}

export interface CourseStatistics {
	courseProgress: Progress;
	byUnits: { [unitId: string]: Progress };
}
