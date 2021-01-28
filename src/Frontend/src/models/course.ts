import { ShortSlideInfo } from "./slide";

interface CourseInfo {
	id: string;
	title: string;
	description: string;
	units: UnitInfo[];
	nextUnitPublishTime: string | null;
	scoring: ScoringGroup;
	containsFlashcards: boolean;
	isTempCourse: boolean;
	tempCourseError: string;
}

interface ScoringGroup {
	id: string;
	name: string;
	abbr: string | null;
	description: string | null;
	weight: number; // decimal
}

interface UnitInfo {
	id: string;
	title: string;
	isNotPublished: boolean | undefined;
	publicationDate: string | undefined;
	slides: ShortSlideInfo[];
	additionalScores: UnitScoringGroupInfo[];
}

interface InfoByUnit {
	unitId: string,
	unitTitle: string,
	unlocked: boolean,
	flashcardsIds: string[],
	unratedFlashcardsCount: number,
	cardsCount: number,
	flashcardsSlideSlug: string,
}

interface AbstractScoringGroupInfo {
	id: string;
	name: string;
	abbreviation: string;
	description: string;
	weight: number; // decimal
}

interface UnitScoringGroupInfo extends AbstractScoringGroupInfo {
	canInstructorSetAdditionalScore: boolean;
	maxAdditionalScore: number;
}

export { CourseInfo, ScoringGroup, UnitInfo, UnitScoringGroupInfo, InfoByUnit };
