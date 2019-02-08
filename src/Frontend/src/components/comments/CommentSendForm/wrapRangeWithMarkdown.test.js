import CommentSendForm from "../CommentSendForm";

describe('CommentSendForm.wrapRangeWithMarkdown', () => {
	test.each([
		["", 0, 0, "bold", "****", 0, 4],
		["", 0, 0, "italic", "__", 0, 2],
		["", 0, 0, "code", "``````", 0, 6],
		["hello", 1, 2, "italic", "h_e_llo", 1, 4],
		["hello", 1, 2, "bold", "h**e**llo", 1, 6],
		["hello", 1, 2, "code", "h```e```llo", 1, 8],
	])('%s with selection, %i, %i %s -> %s with selection, %i, %i',
		(text, selectionStart, selectionEnd, markdown, expectedText, expectedSelectionStart, expectedSelectionEnd) => {
		const result = CommentSendForm.wrapRangeWithMarkdown(text, {start:selectionStart, end:selectionEnd}, markdown);
		expect(result).toEqual({finalText: expectedText, finalSelectionRange:{end: expectedSelectionEnd, start: expectedSelectionStart}});
	});
});


describe('CommentSendForm.wrapRangeWithMarkdown', () => {
	test.each([
		[4, 0, 1, "bold", TypeError],
		[undefined, 0, 0, TypeError],
		[null, 0, 0, "bold", TypeError],
		["", undefined, 0, "bold", TypeError],
		["", 0, null, "bold", TypeError],
		["", 0, '', "bold", TypeError],
		["hello", 2, 1, "bold", RangeError],
		["hello", -1, 0, "bold", RangeError],
		["hello", 0, 10, "bold", RangeError],
		["", 0, 0, undefined, TypeError],
	])('%s with selection, %i, %i %s -> %o with error',
		(text, selectionStart, selectionEnd, markdown) => {
			const result = CommentSendForm.wrapRangeWithMarkdown;
			expect(() => result(text, {start:selectionStart, end:selectionEnd}, markdown)).toThrowError(Error);
		});
});

