import { Comment } from "src/models/comments";

const compareComments = (a: Comment, b: Comment, timeByAscending?: boolean): number => {
	if(a.isPinnedToTop && !b.isPinnedToTop) {
		return -1;
	}

	if(!a.isPinnedToTop && b.isPinnedToTop) {
		return 1;
	}
	const aTime = new Date(a.publishTime).getTime();
	const bTime = new Date(b.publishTime).getTime();
	return timeByAscending ? bTime - aTime : aTime - bTime;
};

const sortComments = (comments: Comment[]): void => {
	comments.sort((a, b) => compareComments(a, b, true));
	comments.forEach(c => c.replies = c.replies.sort(compareComments));
};

export { compareComments, sortComments, };
