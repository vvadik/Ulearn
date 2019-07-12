export const coursePath = "course";


export function constructPathToSlide(courseId, slideId) {
	return `/${coursePath}/${courseId}/${slideId}`;
}
