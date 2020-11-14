import CodeMirror from 'codemirror/lib/codemirror';
import CsCompleter from './CsCompleter';

const completer = new CsCompleter(Object.keys(CodeMirror.resolveMode("text/x-csharp").keywords));

function csharpHint(editor) {
	const cur = editor.getCursor(), token = editor.getTokenAt(cur);
	let from = CodeMirror.Pos(cur.line, token.start);
	let to = CodeMirror.Pos(cur.line, token.end);
	let completionList;
	if(token.string === ".") {
		from = to;
		const lastTokenPos = skipBrackets(editor, cur);
		const lastToken = editor.getTokenAt(lastTokenPos);
		completionList = completer.getCompletions(lastToken.string, "", true);
	} else if(!/^[\w@_]*$/.test(token.string)) {
		from = cur;
		to = cur;
		completionList = completer.getCompletions("", "", false);
	} else {
		let beforeDot = "";
		const lastDot = getPrevToken(editor, cur);
		if(lastDot.token.string === '.') {
			const beforeDotCur = skipBrackets(editor, lastDot.pos);
			beforeDot = editor.getTokenAt(beforeDotCur);
		}
		completionList = completer.getCompletions(beforeDot.string, token.string, false);
	}

	completionList = completionList.sort();

	return {
		list: completionList,
		from: from,
		to: to
	};
}

function skipBrackets(editor, pos) {
	let cursor = getPrevToken(editor, pos);
	if(cursor.token.string !== ')')
		return cursor.pos;
	let balance = 1;
	while (balance) {
		cursor = getPrevToken(editor, cursor.pos);
		if(cursor.token.string === ')')
			balance++;
		if(cursor.token.string === '(')
			balance--;
	}
	return getPrevToken(editor, cursor.pos).pos;
}

function getPrevToken(editor, pos) {
	let res = { ch: pos.ch, line: pos.line };
	let token = editor.getTokenAt(res);
	do {
		res.ch = token.start;
		if(res.ch === 0) {
			if(res.line === 0) return { pos: res, token: "" };
			res = CodeMirror.Pos(res.line - 1);
		}
		token = editor.getTokenAt(res);
	} while (token.string.length === 0 || !/\S/.test(token.string));
	return { pos: res, token: token };
}

CodeMirror.registerHelper("hint", "csharp", csharpHint);