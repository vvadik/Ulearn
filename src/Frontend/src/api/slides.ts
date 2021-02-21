import api from "src/api/index";

import { slides } from "src/consts/routes";
import { Block, BlockTypes } from "src/models/slide";

export function getSlideBlocks(courseId: string, slideId: string): Promise<Block<BlockTypes>[]> {
	return api.get<{ blocks: Block<BlockTypes>[] }>(`${ slides }/${ courseId }/${ slideId }`)
		.then(r => r.blocks);
}
