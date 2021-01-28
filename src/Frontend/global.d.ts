declare module '*.js';

declare module '*.less' {
	const classes: {[key: string]: string};
	export default classes;
}

type EnumDictionary<T extends string | symbol | number, U> = {
	[K in T]: U;
};

type DeepPartial<T> = {
	[P in keyof T]?: T[P] extends (infer U)[]
		? DeepPartial<U>[]
		: T[P] extends Readonly<infer U>[]
			? Readonly<DeepPartial<U>>[]
			: DeepPartial<T[P]>
};
