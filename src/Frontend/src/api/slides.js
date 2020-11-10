import { slides } from "src/consts/routes";
import api from "src/api/index";

export function getSlide(courseId, slideId) {
	return api.get(`${ slides }/${ courseId }/${ slideId }`)
		.then(r => r.blocks);
}

export function submitCode(courseId, slideId, code) {
	return api.exercise.submitCode(courseId, slideId, code);
}
