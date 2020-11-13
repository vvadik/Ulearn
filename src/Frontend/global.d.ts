declare module '*.js';

declare module '*.less' {
	const classes: {[key: string]: string};
	export default classes;
}

type EnumDictionary<T extends string | symbol | number, U> = {
	[K in T]: U;
};
