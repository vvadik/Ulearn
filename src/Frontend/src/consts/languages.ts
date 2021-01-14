export enum Language {
	CSharp = 'cSharp',
	Python2 = 'python2',
	Python3 = 'python3',
	Java = 'java',
	JavaScript = 'javaScript',
	Html = 'html',
	TypeScript = 'typeScript',
	Css = 'css',
	Haskell = 'haskell',
	Text = 'text',
}

export interface LanguageLaunchInfo {
	compiler: string;
	compileCommand: string;
	runCommand: string;
}
