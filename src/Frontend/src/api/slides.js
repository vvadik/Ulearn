import { slides } from "src/consts/routes";
import api from "src/api/index";

export function getSlide(courseId, slideId) {
	return api.get(`${ slides }/${ courseId }/${ slideId }`)
		.then(r => r.blocks);
}
